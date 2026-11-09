using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings;

[Binding]
public unsafe partial struct ExEmGroupHp
{
    [FieldOffset(0x88)] public uint Share;

    [RelativeVirtualAddress(0x3AB8310)]
    public partial void SetHp(ulong hp);
}
