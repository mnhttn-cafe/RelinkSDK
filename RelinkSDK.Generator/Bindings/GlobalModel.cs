namespace RelinkSDK.Generator.Bindings;

internal sealed record GlobalModel(
    string Name,
    string Type,
    uint Address,
    bool HasSetter,
    bool ReturnsByReference,
    int? Length);