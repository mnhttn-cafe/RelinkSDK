using System.Runtime.InteropServices;
using RelinkSDK.Attributes;
using RelinkSDK.Bindings.Entities;

namespace RelinkSDK.Bindings.Behaviour;

[Binding]
public unsafe partial struct BehaviorTreeComponent
{
    [FieldOffset(0x10)] public EntityHandle Owner;
}
