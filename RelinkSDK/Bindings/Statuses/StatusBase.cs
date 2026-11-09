using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings;

[Binding]
public unsafe partial struct StatusBase
{
    [FieldOffset(0x28)] public EntityHandle Source;
    [FieldOffset(0x4C)] public int SourceTag;
    [FieldOffset(0x50)] public int Id;
    [FieldOffset(0x54)] public int Parameter;
    [FieldOffset(0x79)] public bool Permanent;
    [FieldOffset(0x7C)] public float Duration;
    [FieldOffset(0x80)] public float TimeLeft;
}
