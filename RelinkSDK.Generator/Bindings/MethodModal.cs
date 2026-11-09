using RelinkSDK.Generator.Utilities;

namespace RelinkSDK.Generator.Bindings;

internal sealed record MethodModel(
    string Name,
    string ReturnType,
    bool IsStatic,
    uint Address,
    int? Slot,
    bool IsInherited,
    EquatableArray<(string Type, string Name)> Parameters);
