using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings;

[Binding]
[Base(typeof(Behavior))]
[Base(typeof(Entities.ExHp), 0x150)]
public unsafe partial struct BehaviorAppBase
{
}
