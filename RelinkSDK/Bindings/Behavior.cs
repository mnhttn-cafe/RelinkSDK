using System.Runtime.InteropServices;
using RelinkSDK.Attributes;
using RelinkSDK.Bindings.Unknown;
using RelinkSDK.Interop;

namespace RelinkSDK.Bindings;

[Binding]
public unsafe partial struct Behavior
{
    [FieldOffset(0x28)] private UnknownA* _link;

    public Entities.Entity* Entity => _link->Entity;

    [VirtualFunction(9)]
    public partial CString* GetTypeName(CString* result);

    [VirtualFunction(10)]
    public partial CString* GetBaseTypeName(CString* result);

    [VirtualFunction(11)]
    public partial uint* GetTypeId(uint* result);
}
