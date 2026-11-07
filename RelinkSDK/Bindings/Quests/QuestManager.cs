using RelinkSDK.Attributes;

namespace RelinkSDK.Bindings.Quests;

[Binding]
public unsafe partial struct QuestManager
{
    [RelativeVirtualAddress(0x7C53D30)]
    public static partial QuestManager* Instance { get; }

    [RelativeVirtualAddress(0x61DFC0)]
    public partial void GetCurrentQuestId(uint* result);
}