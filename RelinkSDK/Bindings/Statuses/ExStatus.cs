using System.Runtime.InteropServices;
using RelinkSDK.Attributes;
using RelinkSDK.Standard;

namespace RelinkSDK.Bindings;

[Binding]
public unsafe partial struct ExStatus
{
    [FieldOffset(0x8)] public void* Owner;

    [FieldOffset(0x18)] public Vector<nint> Statuses;

    [RelativeVirtualAddress(0xBCD270)]
    public partial void ApplyStatus(void* result, int id, Entity* source, float duration, int sourceTag,
        int unknownParameter, byte* paramsBegin, byte* paramsEnd, bool clearProtected, bool suppressBroadcast,
        bool unknownC, bool unknownD, bool skipSourceAdjustment, bool force);

    [RelativeVirtualAddress(0xBD00D0)]
    public static partial void BroadcastStatus(void* owner, Entity* source, int id, float duration, int sourceTag,
        int unknownParameter, bool clearProtected, bool unknownC, byte* paramsBegin, byte* paramsEnd, bool ownerOnly,
        bool createIfMissing);
}
