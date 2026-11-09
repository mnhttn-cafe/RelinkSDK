using RelinkSDK.Attributes;
using RelinkSDK.Bindings.Players;

namespace RelinkSDK.Bindings;

[Binding]
public unsafe partial struct Globals
{
    [RelativeVirtualAddress(0x7035D00), ArrayLength(4)]
    public static partial BehaviorPlayerBase** PlayerSlots { get; }

    [RelativeVirtualAddress(0x7035CA0), ArrayLength(4)]
    public static partial EntityHandle* PlayerEntities { get; }
}