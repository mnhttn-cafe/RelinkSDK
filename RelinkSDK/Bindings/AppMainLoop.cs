using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings;

[Binding]
public unsafe partial struct AppMainLoop
{
    [VirtualFunction(3)]
    public partial void Step();
}
