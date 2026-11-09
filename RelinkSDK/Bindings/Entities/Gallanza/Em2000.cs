using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Enemies.Gallanza;

[Binding]
[Base(typeof(EmBehaviorBase))]
public unsafe partial struct Em2000
{
    [RelativeVirtualAddress(0x2708580)]
    public partial void Update();
}
