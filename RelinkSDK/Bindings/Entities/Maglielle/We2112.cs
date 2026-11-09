using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Entities.Maglielle;

[Binding]
[Base(typeof(SuperiorWeapon))]
public unsafe partial struct We2112
{
    [RelativeVirtualAddress(0x2DEB700)]
    public partial void Update();
}
