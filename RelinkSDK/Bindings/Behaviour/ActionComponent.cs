using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Behaviour;

[Binding]
[Base(typeof(BehaviorTreeComponent))]
public unsafe partial struct ActionComponent
{
    [VirtualFunction(9)]
    public partial void Start();

    [VirtualFunction(10)]
    public partial void End();
}
