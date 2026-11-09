using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings;

[Binding]
public unsafe partial struct ExEmLinkAttack
{
    [FieldOffset(0x8C)] public float Unknown8C;
    [FieldOffset(0x90)] public float Unknown90;
    [FieldOffset(0x98)] public float Unknown98;
    [FieldOffset(0x9C)] public float Unknown9C;
    [FieldOffset(0xF0)] public float LinkGauge;
    [FieldOffset(0xF4)] public float LinkGaugeMax;
    [FieldOffset(0xF8)] public float UnknownF8;
}
