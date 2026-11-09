using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Enemies.Maglielle;

[Binding]
[Base(typeof(EmBehaviorBase))]
public unsafe partial struct We2110
{
    [RelativeVirtualAddress(0x2DD9910)]
    public partial void Update();
}
