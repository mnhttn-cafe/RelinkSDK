using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings;

[Binding]
public unsafe partial struct StatusBase
{
    [FieldOffset(0x50)] public int Id;
    
    [RelativeVirtualAddress(0xBCD270)]
    public partial void ApplyStatus(void* result, int id, void* source, float duration);
}