using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Behaviour;

[Binding]
public unsafe partial struct RandomSelectorEmNode
{
    [FieldOffset(0x38)] public int State;
    [FieldOffset(0xE8)] public uint RandomState;
    [FieldOffset(0xEC)] public int ChosenIndex;
    [FieldOffset(0x150)] public int LastPick;
    [FieldOffset(0x154)] public int RepeatCount;

    [RelativeVirtualAddress(0x1F14AF0)]
    public static partial RandomSelectorEmNode* Create();

    [RelativeVirtualAddress(0x1F15D40)]
    public partial int Tick();
}
