using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Entities.Maglielle;

[Binding]
[Base(typeof(SuperiorWeapon))]
public unsafe partial struct We2110
{
    [RelativeVirtualAddress(0x2DD9910)]
    public partial void Update();
}
