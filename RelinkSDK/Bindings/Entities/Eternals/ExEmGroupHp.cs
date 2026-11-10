using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Entities.Eternals;

[Binding]
public unsafe partial struct ExEmGroupHp
{
    [FieldOffset(0x10)] public EntityHandle Holder;
    [FieldOffset(0x78)] public EmGroupHpManager* Manager;
    [FieldOffset(0x88)] public uint Share;

    [RelativeVirtualAddress(0x3AB7850)]
    public partial void Damage(uint damage, uint flag);
}
