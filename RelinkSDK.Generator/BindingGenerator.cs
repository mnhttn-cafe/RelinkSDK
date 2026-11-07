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

        foreach (var attribute in symbol.GetAttributes())
        {
            var inheritedFields = new List<FieldModel>();
            
            // no other attribute on the entire struct really
            // matters here atm
            if (attribute.AttributeClass?.ToDisplayString() != "RelinkSDK.Attributes.BaseAttribute")
            {
                continue;
            }

            if (attribute.ConstructorArguments[0].Value is not INamedTypeSymbol baseType
                || attribute.ConstructorArguments[1].Value is not uint baseOffset)
            {
                continue;
            }

            foreach (var fieldToInherit in baseType.GetMembers().OfType<IFieldSymbol>())
            {
                // declared/generated statics & constants stay on their
                // defined class
                if (fieldToInherit.IsStatic || fieldToInherit.IsConst)
                {
                    continue;
                }
                
                var fieldOffset = fieldToInherit.GetAttributes()
                    .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == "System.Runtime.InteropServices.FieldOffsetAttribute")
                    ?.ConstructorArguments[0].Value as int?;

                if (fieldOffset == null)
                {
                    continue;
                }
                
                inheritedFields.Add(new FieldModel(
                    fieldToInherit.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    fieldToInherit.Name,
                    baseOffset + (uint)fieldOffset.Value
                ));
            }
            
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

            globals.Add(new GlobalModel(
                property.Name,
                property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                attribute.ConstructorArguments[0].Value is uint address ? address : 0,
                property.SetMethod != null,
                property.ReturnsByRef));
        }

        foreach (var method in symbol.GetMembers().OfType<IMethodSymbol>())
        {
            var attribute = method.GetAttributes().FirstOrDefault(a =>
                a.AttributeClass?.ToDisplayString() == "RelinkSDK.Attributes.RelativeVirtualAddressAttribute");

            if (attribute == null)
                continue;

            var parameters =
                method.Parameters.Select(p =>
                {
                    return (
                        p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                        p.Name
                    );
                });
            
            methods.Add(new MethodModel(
                method.Name,
                method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                method.IsStatic,
                (uint?)attribute.ConstructorArguments[0].Value ?? 0x0,
                new EquatableArray<(string Type, string Name)>(parameters.ToArray())
            ));
        }

        return new BindingModel(
            symbol.ContainingNamespace.ToDisplayString(),
            symbol.Name,
            new EquatableArray<BaseModel>(inheritors.ToArray()),
            new EquatableArray<GlobalModel>(globals.ToArray()),
            new EquatableArray<MethodModel>(methods.ToArray())
        );
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

        if (model.ReturnsByReference)
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
        code.AppendLine($"public const nint {model.Name}_RVA = 0x{model.Address:X};");

        if (!model.IsStatic)
        {
            types.Add($"{structName}*");
            parameters.Add($"({structName}*)Unsafe.AsPointer(ref this)");
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

        var call = $"((delegate* unmanaged<{string.Join(", ", types)}>)(global::RelinkSDK.Game.Instance.BaseAddress + {model.Name}_RVA))({string.Join(", ", parameters)})";
        if (returnsBool)
        {
            call += " != 0";
        }

        var modifiers = model.IsStatic ? "public static" : "public";
        code.AppendLine();
        code.AppendLine($"{modifiers} partial {model.ReturnType} {model.Name}({string.Join(", ", declaration)})");
        code.AppendLine($"    => {call};");
    }

    private void EmitHook(StringBuilder code, string structName, MethodModel model)
    {
        var delegateParameters = new List<string>();

        if (!model.IsStatic)
        {
            delegateParameters.Add($"{structName}* self");
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
        code.AppendLine($"public static IHook<{model.Name}Fn> Hook{model.Name}(IReloadedHooks hooks, {model.Name}Fn handler)");
        code.AppendLine($"    => hooks.CreateHook<{model.Name}Fn>(handler, global::RelinkSDK.Game.Instance.BaseAddress + {model.Name}_RVA).Activate();");
    }
}