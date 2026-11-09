using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings;

[Binding]
public unsafe partial struct Entity
{
    [FieldOffset(0x38)] public EntityHandle Handle;
    [FieldOffset(0x70)] public Behavior* Behavior;
}
