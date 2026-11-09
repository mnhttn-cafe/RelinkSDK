using System.Runtime.InteropServices;
using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings;

[Binding]
public unsafe partial struct EntityHandleManager
{
    [FieldOffset(0x20)] public Entity** Slots;
    [FieldOffset(0x48)] public ulong* SlotStamps; // __rdtsc()

    [RelativeVirtualAddress(0x7020988)]
    public static partial EntityHandleManager* Instance { get; }

    public Entity* Resolve(in EntityHandle handle)
    {
        if (handle.Index == 0 || handle.Entity == null)
        {
            return null;
        }

        uint slot = handle.Index - 1;

        if (SlotStamps[slot] != handle.Stamp || Slots[slot] != handle.Entity)
        {
            return null;
        }

        return handle.Entity;
    }
    
    public T* Resolve<T>(in EntityHandle handle) where T : unmanaged
    {
        Entity* entity = Resolve(handle);

        if (entity == null)
        {
            return null;
        }

        return (T*)entity->Behavior;
    }
}
