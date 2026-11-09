using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings;

[Binding]
public unsafe partial struct ExNetwork
{
    [FieldOffset(0x8)] public void* Host;
    [FieldOffset(0x10)] public ulong NetworkId;
    [FieldOffset(0x18)] public int NetworkSlot;

    [RelativeVirtualAddress(0xA673E0)]
    public partial bool Setup();

    [RelativeVirtualAddress(0xA67D00)]
    public partial void SendHp(ulong hp);

    [RelativeVirtualAddress(0xA67E70)]
    public partial void SendMaxHp(ulong maxHp);

    [RelativeVirtualAddress(0xA67FE0)]
    public partial void SendCurrentHp();

    [RelativeVirtualAddress(0xB1FE40)]
    public partial void SyncCurrentHp();
}
