using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Entities.Maglielle;

/// <summary>
/// Enemy class for Maglielle
/// </summary>
[Binding]
[Base(typeof(EmBossBase))]
public unsafe partial struct Em2100
{
    [RelativeVirtualAddress(0x270FEF0)]
    public partial void Update();
}
