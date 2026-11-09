using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings;

[Binding]
public unsafe partial struct ExParent
{
    [FieldOffset(0x38)] public EntityHandle Parent;
    [FieldOffset(0x50)] public int Attached;
}
