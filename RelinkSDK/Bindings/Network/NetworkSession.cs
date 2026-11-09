using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings;

[Binding]
public unsafe partial struct NetworkSession
{
    [FieldOffset(0x4)] public int State;

    [RelativeVirtualAddress(0x7C53D10)]
    public static partial NetworkSession* Instance { get; }

    [RelativeVirtualAddress(0x261150)]
    public partial int LocalSlot();
}
