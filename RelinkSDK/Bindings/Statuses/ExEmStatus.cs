using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings;

[Binding]
[Base(typeof(ExStatus))]
public unsafe partial struct ExEmStatus
{
    [FieldOffset(0x220)] public float RegenCycle;
    [FieldOffset(0x224)] public float RegenTimer;
    [FieldOffset(0x22C)] public float RegenPercent;

    [RelativeVirtualAddress(0x29198F0)]
    public partial void Update(float delta);
}
