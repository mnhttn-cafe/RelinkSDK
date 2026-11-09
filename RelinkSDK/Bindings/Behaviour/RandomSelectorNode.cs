using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Behaviour;

[Binding]
public unsafe partial struct RandomSelectorNode
{
    [FieldOffset(0x38)] public int State;
    [FieldOffset(0xE8)] public uint RandomState;
    [FieldOffset(0xEC)] public int ChosenIndex;
    [FieldOffset(0xF0)] public int ChildCount;
    [FieldOffset(0x110)] public bool AnySuccessMode;

    [RelativeVirtualAddress(0x1F223D0)]
    public partial void Seed();

    [RelativeVirtualAddress(0x1F224C0)]
    public partial int Tick();
}
