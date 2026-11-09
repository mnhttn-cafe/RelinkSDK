using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Entities;

[Binding]
[Base(typeof(BehaviorAppBase))]
[Base(typeof(Network.ExNetwork), 0x9E0)]
[Base(typeof(ExEmLinkAttack), 0xAA0)]
[Base(typeof(Statuses.ExEmStatus), 0xCF8)]
public unsafe partial struct EmBehaviorBase
{
    [FieldOffset(0x15A8)] public ulong BaseHp;
    [FieldOffset(0x15B0)] public uint BaseAttack;
    [FieldOffset(0x15D0)] public uint Unknown15D0;
    [FieldOffset(0x15D8)] public uint Level;

    [RelativeVirtualAddress(0xB40B20)]
    public partial void InitStatus(int enemyParam);

    [RelativeVirtualAddress(0xB40CF0)]
    public static partial void MakeStatusKey(uint unknown4A, uint* key, int enemyParam);
}
