using RelinkSDK.Generator.Utilities;

namespace RelinkSDK.Generator.Bindings;

internal sealed record BindingModel(
    string Namespace, 
    string Name,
    EquatableArray<BaseModel> Bases,
    EquatableArray<GlobalModel> Globals,
    EquatableArray<MethodModel> Methods);