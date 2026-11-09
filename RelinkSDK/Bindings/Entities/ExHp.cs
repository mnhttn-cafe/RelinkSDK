using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Entities;

[Binding]
public unsafe partial struct ExHp
{
    [FieldOffset(0x10)] public ulong CurrentHp;
    [FieldOffset(0x18)] public ulong MaxHp;

    [RelativeVirtualAddress(0x1F75EA0)]
    public partial void SetMaxHp(ulong val);
    
    [RelativeVirtualAddress(0x1F75EB0)]
    public partial void SetCurrentHp(ulong val);
}