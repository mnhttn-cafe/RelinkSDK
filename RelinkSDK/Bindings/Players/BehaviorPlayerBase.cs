using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Players;

[Binding]
[Base(typeof(BehaviorAppBase))]
[Base(typeof(Statuses.ExStatus), 0xAE0)]
[Base(typeof(Network.ExNetwork), 0x4350)]
public unsafe partial struct BehaviorPlayerBase
{
    [FieldOffset(0x14FA4)] public int PlayerSlot;
}
