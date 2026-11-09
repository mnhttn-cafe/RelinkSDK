using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Entities.Gallanza;

[Binding]
[Base(typeof(EmBossBase))]
public unsafe partial struct Em2000
{
    [RelativeVirtualAddress(0x2708580)]
    public partial void Update();
}
