using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Behaviour;

[Binding]
public unsafe partial struct BtGlobalState
{
    [FieldOffset(0x0)] public uint RandomState;

    [RelativeVirtualAddress(0x7ADBF28)]
    public static partial BtGlobalState* Instance { get; }
}
