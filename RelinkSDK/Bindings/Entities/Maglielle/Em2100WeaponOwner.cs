using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Enemies.Maglielle;

[Binding]
public unsafe partial struct Em2100WeaponOwner
{
    [FieldOffset(0x10)] public EntityHandle Owner;
}
