using RelinkSDK.Generator.Utilities;

namespace RelinkSDK.Generator.Bindings;

internal sealed record MethodModel(
    string Name,
    string ReturnType,
    bool IsStatic,
    uint Address,
    int? Slot,
    bool IsInherited,
    uint BaseOffset,
    string BaseType,
    EquatableArray<(string Type, string Name)> Parameters);
