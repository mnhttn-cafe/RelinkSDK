using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Entities;

[Binding]
public unsafe partial struct Entity
{
    [FieldOffset(0x38)] public EntityHandle Handle;
    [FieldOffset(0x70)] public Behavior* Behavior;

    [RelativeVirtualAddress(0xBD0010)]
    public partial bool IsLocalAuthority();
}
