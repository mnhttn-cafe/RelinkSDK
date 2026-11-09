using System.Runtime.InteropServices;
using RelinkSDK.Attributes;
using RelinkSDK.Standard;

namespace RelinkSDK.Bindings;

[Binding]
public unsafe partial struct EmGroupHpManager
{
    [FieldOffset(0x8)] public Vector<EntityHandle> Members;
    [FieldOffset(0x20)] public bool Dirty;

    [RelativeVirtualAddress(0x3729440)]
    public partial void Update();

    [RelativeVirtualAddress(0x37296C0)]
    public static partial ExEmGroupHp* FindMember(EntityHandle* member);
}
