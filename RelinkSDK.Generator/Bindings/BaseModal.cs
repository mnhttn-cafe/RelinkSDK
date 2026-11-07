using RelinkSDK.Generator.Utilities;

namespace RelinkSDK.Generator.Bindings;

internal sealed record BaseModel(
    string Type,
    string Name,
    uint Offset,
    EquatableArray<FieldModel> Fields);
