using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RelinkSDK.Generator.Bindings;
using RelinkSDK.Generator.Utilities;

namespace RelinkSDK.Generator;

[Generator]
internal class BindingGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var bindings = context.SyntaxProvider.ForAttributeWithMetadataName(
            "RelinkSDK.Attributes.BindingAttribute",
            predicate: (node, _) => node is StructDeclarationSyntax,
            transform: (ctx, _) => TransformBinding(ctx)
        );
        
        context.RegisterSourceOutput(bindings, (spc, model) =>
        {
            spc.AddSource($"{model.Name}.g.cs", Emit(model));
        });
    }

    public BindingModel TransformBinding(GeneratorAttributeSyntaxContext context)
    {
        var symbol = (INamedTypeSymbol)context.TargetSymbol;
        var methods = new List<MethodModel>();
        var inheritors = new List<BaseModel>();
        var globals = new List<GlobalModel>();
        var inheritedMethods = new List<MethodModel>();

        foreach (var attribute in symbol.GetAttributes())
        {
            var inheritedFields = new List<FieldModel>();

            if (attribute.AttributeClass?.ToDisplayString() != "RelinkSDK.Attributes.BaseAttribute")
            {
                continue;
            }

            if (attribute.ConstructorArguments[0].Value is not INamedTypeSymbol baseType
                || attribute.ConstructorArguments[1].Value is not uint baseOffset)
            {
                continue;
            }

            CollectBases(baseType, baseOffset, inheritedFields, inheritedMethods);

            inheritors.Add(
                new BaseModel(
                    baseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    baseType.Name,
                    baseOffset,
                    new EquatableArray<FieldModel>(inheritedFields.ToArray())
                )
            );
        }

        foreach (var property in symbol.GetMembers().OfType<IPropertySymbol>())
        {
            var attribute = property.GetAttributes().FirstOrDefault(a =>
                a.AttributeClass?.ToDisplayString() == "RelinkSDK.Attributes.RelativeVirtualAddressAttribute");

            // only statics are globals, an instance one would need a this to be relative to
            if (attribute == null || !property.IsStatic)
                continue;

            var length = property.GetAttributes().FirstOrDefault(a =>
                a.AttributeClass?.ToDisplayString() == "RelinkSDK.Attributes.ArrayLengthAttribute");

            globals.Add(new GlobalModel(
                property.Name,
                property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                attribute.ConstructorArguments[0].Value is uint address ? address : 0,
                property.SetMethod != null,
                property.ReturnsByRef,
                length?.ConstructorArguments[0].Value as int?));
        }

        var ownNames = new HashSet<string>(symbol.GetMembers().Select(m => m.Name));
        var nameCounts = inheritors
            .SelectMany(b => b.Fields.Select(f => f.Name).Append(b.Name))
            .GroupBy(name => name)
            .ToDictionary(g => g.Key, g => g.Count());

        for (int i = 0; i < inheritors.Count; i++)
        {
            var kept = inheritors[i].Fields
                .Where(f => nameCounts[f.Name] == 1 && !ownNames.Contains(f.Name))
                .ToArray();

            inheritors[i] = inheritors[i] with { Fields = new EquatableArray<FieldModel>(kept) };
        }

        foreach (var method in symbol.GetMembers().OfType<IMethodSymbol>())
        {
            var model = ToMethodModel(method, false);

            if (model != null)
            {
                methods.Add(model);
            }
        }

        var fieldNames = new HashSet<string>(inheritors.SelectMany(b => b.Fields.Select(f => f.Name).Append(b.Name)));
        var methodNames = new HashSet<string>();

        foreach (var method in inheritedMethods)
        {
            if (ownNames.Contains(method.Name) || fieldNames.Contains(method.Name) || !methodNames.Add(method.Name))
                continue;

            methods.Add(method);
        }

        return new BindingModel(
            symbol.ContainingNamespace.ToDisplayString(),
            symbol.Name,
            new EquatableArray<BaseModel>(inheritors.ToArray()),
            new EquatableArray<GlobalModel>(globals.ToArray()),
            new EquatableArray<MethodModel>(methods.ToArray())
        );
    }

    private MethodModel? ToMethodModel(IMethodSymbol method, bool inherited)
    {
        var attributes = method.GetAttributes();
        var address = attributes.FirstOrDefault(a =>
            a.AttributeClass?.ToDisplayString() == "RelinkSDK.Attributes.RelativeVirtualAddressAttribute");
        var slot = attributes.FirstOrDefault(a =>
            a.AttributeClass?.ToDisplayString() == "RelinkSDK.Attributes.VirtualFunctionAttribute");

        if (address == null && slot == null)
        {
            return null;
        }

        var parameters = method.Parameters.Select(p => (
            p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            p.Name
        ));

        return new MethodModel(
            method.Name,
            method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            method.IsStatic,
            address?.ConstructorArguments[0].Value as uint? ?? 0x0,
            slot?.ConstructorArguments[0].Value as int?,
            inherited,
            0,
            "",
            new EquatableArray<(string Type, string Name)>(parameters.ToArray())
        );
    }

    private void CollectBases(INamedTypeSymbol type, uint offset, List<FieldModel> inheritedFields, List<MethodModel> inheritedMethods)
    {
        foreach (var method in type.GetMembers().OfType<IMethodSymbol>())
        {
            var model = ToMethodModel(method, true);

            if (model is not { Slot: not null, IsStatic: false })
                continue;

            // a base past offset 0 has its own vtable inside the object,
            // so the call has to go through that one with this moved there
            inheritedMethods.Add(offset == 0
                ? model
                : model with
                {
                    BaseOffset = offset,
                    BaseType = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                });
        }

        foreach (var fieldToInherit in type.GetMembers().OfType<IFieldSymbol>())
        {
            // declared/generated statics & constants stay on their
            // defined class
            if (fieldToInherit.IsStatic || fieldToInherit.IsConst)
            {
                continue;
            }

            var fieldOffset = fieldToInherit.GetAttributes()
                .FirstOrDefault(a =>
                    a.AttributeClass?.ToDisplayString() == "System.Runtime.InteropServices.FieldOffsetAttribute")
                ?.ConstructorArguments[0].Value as int?;

            if (fieldOffset == null)
            {
                continue;
            }

            inheritedFields.Add(new FieldModel(
                fieldToInherit.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                fieldToInherit.Name,
                offset + (uint)fieldOffset.Value
            ));
        }
            
        // GetMembers() can only see base fields that already exist pre-generation
        // but deeper level nested ones have to be recursively inherited
        foreach (var attribute in type.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() != "RelinkSDK.Attributes.BaseAttribute")
            {
                continue;
            }

            if (attribute.ConstructorArguments[0].Value is not INamedTypeSymbol baseType
                || attribute.ConstructorArguments[1].Value is not uint baseOffset)
            {
                continue;
            }

            // the embedded field itself, e.g. ExEmStatus.@ExStatus
            inheritedFields.Add(new FieldModel(
                baseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                baseType.Name,
                offset + baseOffset
            ));

            CollectBases(baseType, offset + baseOffset, inheritedFields, inheritedMethods);
        }
    }
    
    private string Emit(BindingModel model)
    {
        StringBuilder code = new();

        code.AppendLine("// <auto-generated/>");
        code.AppendLine("using System.Runtime.InteropServices;");
        code.AppendLine("using System.Runtime.CompilerServices;");
        code.AppendLine("using Reloaded.Hooks.Definitions;");
        code.AppendLine("using Reloaded.Hooks.Definitions.X64;");
        code.AppendLine();
        code.AppendLine($"namespace {model.Namespace};");

        code.AppendLine();
        code.AppendLine("[StructLayout(LayoutKind.Explicit)]");
        code.AppendLine($"public unsafe partial struct {model.Name}");
        code.AppendLine("{");
        
        foreach (BaseModel baseModel in model.Bases)
        {
            EmitBase(code, baseModel);
        }

        foreach (GlobalModel globalModel in model.Globals)
        {
            EmitGlobal(code, globalModel);
        }

        foreach (MethodModel methodModel in model.Methods)
        {
            EmitMethod(code, model.Name, methodModel);
            EmitHook(code, model.Name, methodModel);
        }
        
        code.AppendLine("}");
        return code.ToString();
    }

    private void EmitGlobal(StringBuilder code, GlobalModel model)
    {
        code.AppendLine();
        code.AppendLine($"public const nint {model.Name}_RVA = 0x{model.Address:X};");
        code.AppendLine();

        if (model.Length != null)
        {
            code.AppendLine($"public const int {model.Name}_LENGTH = {model.Length};");
            code.AppendLine();
            code.AppendLine($"public static partial {model.Type} {model.Name}");
            code.AppendLine("{");
            code.AppendLine($"    get => ({model.Type})(global::RelinkSDK.Game.Instance.BaseAddress + {model.Name}_RVA);");
            code.AppendLine("}");
        }
        else if (model.ReturnsByReference)
        {

            code.AppendLine($"public static partial ref {model.Type} {model.Name}");
            code.AppendLine("{");
            code.AppendLine($"    get => ref *({model.Type}*)(global::RelinkSDK.Game.Instance.BaseAddress + {model.Name}_RVA);");
            code.AppendLine("}");
        }
        else
        {
            code.AppendLine($"public static partial {model.Type} {model.Name}");
            code.AppendLine("{");
            code.AppendLine($"    get => *({model.Type}*)(global::RelinkSDK.Game.Instance.BaseAddress + {model.Name}_RVA);");
            
            if (model.HasSetter)
            {
                code.AppendLine($"    set => *({model.Type}*)(global::RelinkSDK.Game.Instance.BaseAddress + {model.Name}_RVA) = value;");
            }
            
            code.AppendLine("}");
        }
    }

    private void EmitBase(StringBuilder code, BaseModel model)
    {
        code.AppendLine($"[FieldOffset(0x{model.Offset:X})] public {model.Type} @{model.Name};");
        code.AppendLine();
        code.AppendLine($"// fields inherited from {model.Name}");
        
        foreach (FieldModel fieldModel in model.Fields)
        {
            code.AppendLine($"[FieldOffset(0x{fieldModel.Offset:X})] public {fieldModel.Type} {fieldModel.Name};");
        }
    }

    private void EmitMethod(StringBuilder code, string structName, MethodModel model)
    {
        var types = new List<string>();
        var parameters = new List<string>();
        var declaration = new List<string>();
        
        code.AppendLine();
        code.AppendLine(model.Slot != null
            ? $"public const int {model.Name}_SLOT = {model.Slot};"
            : $"public const nint {model.Name}_RVA = 0x{model.Address:X};");

        var selfType = model.BaseOffset == 0 ? structName : model.BaseType;
        var self = model.BaseOffset == 0
            ? $"({structName}*)Unsafe.AsPointer(ref this)"
            : $"({selfType}*)((byte*)Unsafe.AsPointer(ref this) + 0x{model.BaseOffset:X})";

        if (!model.IsStatic)
        {
            types.Add($"{selfType}*");
            parameters.Add(self);
        }

        foreach (var (type, name) in model.Parameters)
        {
            declaration.Add($"{type} @{name}");

            if (type == "bool")
            {
                types.Add("byte");
                parameters.Add($"(byte)(@{name} ? 1 : 0)");
            }
            else
            {
                types.Add(type);
                parameters.Add($"@{name}");
            }
        }

        bool returnsBool = model.ReturnType == "bool";
        types.Add(returnsBool ? "byte" : model.ReturnType);

        var target = model.Slot != null
            ? $"(*(nint**){self})[{model.Name}_SLOT]"
            : $"global::RelinkSDK.Game.Instance.BaseAddress + {model.Name}_RVA";

        var call = $"((delegate* unmanaged<{string.Join(", ", types)}>)({target}))({string.Join(", ", parameters)})";
        if (returnsBool)
        {
            call += " != 0";
        }

        var modifiers = model.IsStatic ? "public static" : "public";
        var partial = model.IsInherited ? "" : " partial";
        code.AppendLine();
        code.AppendLine($"{modifiers}{partial} {model.ReturnType} {model.Name}({string.Join(", ", declaration)})");
        code.AppendLine($"    => {call};");
    }

    private void EmitHook(StringBuilder code, string structName, MethodModel model)
    {
        var delegateParameters = new List<string>();

        if (!model.IsStatic)
        {
            delegateParameters.Add($"{(model.BaseOffset == 0 ? structName : model.BaseType)}* self");
        }
        
        // managed delegate shits are marshalled
        // into function pointers if they're called
        // by native code
        //
        // but booleans are marshalled into 4 bytes by C#
        // even though the game has them as 1
        // https://stackoverflow.com/questions/28514373/what-is-the-size-of-a-boolean-in-c-does-it-really-take-4-bytes
        foreach (var (type, name) in model.Parameters)
        {
            delegateParameters.Add(type == "bool"
                ? $"[MarshalAs(UnmanagedType.U1)] bool @{name}"
                : $"{type} @{name}");
        }
        
        
        code.AppendLine();
        code.AppendLine("[Function(CallingConventions.Microsoft)]");
        if (model.ReturnType == "bool")
        {
            code.AppendLine("[return: MarshalAs(UnmanagedType.U1)]");
        }
        code.AppendLine($"public delegate {model.ReturnType} {model.Name}Fn({string.Join(", ", delegateParameters)});");
        code.AppendLine();
        var address = model.Slot != null
            ? $"global::RelinkSDK.Game.Instance.VirtualFunction(\"{structName}\", {model.Name}_SLOT, 0x{model.BaseOffset:X})"
            : $"global::RelinkSDK.Game.Instance.BaseAddress + {model.Name}_RVA";

        code.AppendLine($"public static IHook<{model.Name}Fn> Hook{model.Name}(IReloadedHooks hooks, {model.Name}Fn handler)");
        code.AppendLine($"    => hooks.CreateHook<{model.Name}Fn>(handler, {address}).Activate();");
    }
}