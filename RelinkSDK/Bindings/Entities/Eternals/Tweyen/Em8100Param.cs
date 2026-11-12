using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Entities.Eternals.Tweyen;

[Binding]
[Base(typeof(EmBossBaseParam))]
public unsafe partial struct Em8100Param
{
    [FieldOffset(0xA40)] public int EnableCombinationSpArtsLevelAI;
    [FieldOffset(0xA44)] public float CombiSpArtsWedgeHpPercent;
    [FieldOffset(0xA48)] public float CombiSpArtsBigWedgeHpPercent;
}
