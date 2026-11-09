using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Entities.Maglielle;

[Binding]
[Base(typeof(SuperiorWeapon))]
public unsafe partial struct We2113
{
    [RelativeVirtualAddress(0x2DEF090)]
    public partial void Update();
}
