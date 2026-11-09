using System.Runtime.InteropServices;
using RelinkSDK.Attributes;
using RelinkSDK.Bindings.Unknown;

namespace RelinkSDK.Bindings;

[Binding]
public unsafe partial struct Behavior
{
    [FieldOffset(0x28)] private UnknownA* _link;

    public Entity* Entity => _link->Entity;
}
