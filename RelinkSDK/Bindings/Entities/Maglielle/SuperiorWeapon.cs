using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Entities.Maglielle;

[Binding]
[Base(typeof(EmBehaviorBase))]
[Base(typeof(ExParent), 0x1C48)]
[Base(typeof(Em2100WeaponOwner), 0x22D0)]
public unsafe partial struct SuperiorWeapon
{
}
