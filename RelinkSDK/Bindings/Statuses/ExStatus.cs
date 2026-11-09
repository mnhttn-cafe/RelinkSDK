using System.Runtime.InteropServices;
using RelinkSDK.Attributes;
using RelinkSDK.Standard;
using RelinkSDK.Statuses;

namespace RelinkSDK.Bindings.Statuses;

[Binding]
public unsafe partial struct ExStatus
{
    [FieldOffset(0x8)] public void* Owner;

    [FieldOffset(0x18)] public Vector<nint> Statuses;

    [RelativeVirtualAddress(0xBCD270)]
    public partial void ApplyStatus(void* result, int id, Entities.Entity* source, float duration, int sourceTag,
        int unknownParameter, byte* paramsBegin, byte* paramsEnd, bool clearProtected, bool suppressBroadcast,
        bool unknownC, bool unknownD, bool skipSourceAdjustment, bool force);

    [RelativeVirtualAddress(0xBD00D0)]
    public static partial void BroadcastStatus(void* owner, Entities.Entity* source, int id, float duration, int sourceTag,
        int unknownParameter, bool clearProtected, bool unknownC, byte* paramsBegin, byte* paramsEnd, bool ownerOnly,
        bool createIfMissing);
    
    public StatusBase* FindStatus(StatusId id)
    {
        foreach (nint status in Statuses.AsSpan())
        {
            var s = (StatusBase*)status;
            if (s != null && s->Id == id) return s;
        }
        
        return null;
    }

    public bool HasStatus(StatusId id) => FindStatus(id) != null;
}
