using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Entities.Maglielle;

[Binding]
[Base(typeof(SuperiorWeapon))]
public unsafe partial struct We2111
{
    [RelativeVirtualAddress(0x2DE6E00)]
    public partial void Update();
}
