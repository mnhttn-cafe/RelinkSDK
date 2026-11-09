using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Entities;

[Binding]
public unsafe partial struct ExEmSummoned
{
    [FieldOffset(0x10)] public EntityHandle Owner;
    [FieldOffset(0x28)] public bool HasOwner;
}
