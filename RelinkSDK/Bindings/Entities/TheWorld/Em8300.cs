using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Entities.TheWorld;

/// <summary>
/// Enemy class for The World
/// </summary>
[Binding]
[Base(typeof(EmBossBase))]
public unsafe partial struct Em8300
{
    [FieldOffset(0x270C)] public uint CurrentMode;

    [RelativeVirtualAddress(0x30181C0)]
    public static partial Em8300* Create();

    [RelativeVirtualAddress(0x301A060)]
    public partial void Destructor();

    [RelativeVirtualAddress(0x301BFD0)]
    public partial void Update();
}
