using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Players;

[Binding]
[Base(typeof(Behavior), 0)]
[Base(typeof(ExHp), 0x150)]
[Base(typeof(ExStatus), 0xAE0)]
[Base(typeof(ExNetwork), 0x4350)]
public unsafe partial struct BehaviorPlayerBase
{
    [FieldOffset(0x14FA4)] public int PlayerSlot;
}
