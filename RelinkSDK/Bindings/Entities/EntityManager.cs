using System.Runtime.InteropServices;
using RelinkSDK.Attributes;
using RelinkSDK.Interop;
using RelinkSDK.Standard;

namespace RelinkSDK.Bindings.Entities;

[Binding]
public unsafe partial struct EntityManager
{
    [FieldOffset(0x18)] public Vector<Pointer<Entity>> Entities;

    [RelativeVirtualAddress(0x7C276D0)]
    public static partial ref EntityManager Instance { get; }

    [RelativeVirtualAddress(0x237480)]
    public partial Entity* Create(void* name, void* description, void* unknown);

    [RelativeVirtualAddress(0x235890)]
    public static partial void Destroy(Entity* entity);
}
