using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Enemies.Maglielle;

[Binding]
[Base(typeof(EmBehaviorBase))]
public unsafe partial struct We2112
{
    [RelativeVirtualAddress(0x2DEB700)]
    public partial void Update();
}
