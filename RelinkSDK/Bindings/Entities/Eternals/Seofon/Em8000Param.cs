using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Entities.Eternals.Seofon;

[Binding]
[Base(typeof(EmBossBaseParam))]
public unsafe partial struct Em8000Param
{
    [FieldOffset(0xCE0)] public int EnableCombinationSpArtsLevelAI;
}
