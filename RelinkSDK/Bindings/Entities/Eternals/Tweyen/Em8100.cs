using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Entities.Eternals.Tweyen;

[Binding]
[Base(typeof(EmBossBase))]
[Base(typeof(ExEmGroupHp), 0x2870)]
public unsafe partial struct Em8100
{
    [FieldOffset(0x1598)] public Em8100Param* Param;
}
