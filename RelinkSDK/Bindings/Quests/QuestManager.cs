using System.Runtime.InteropServices;
using RelinkSDK.Attributes;
using RelinkSDK.Quests;

namespace RelinkSDK.Bindings.Quests;

[Binding]
public unsafe partial struct QuestManager
{
    [FieldOffset(0x210)] public ReceptionQuestFlowBase* Flow;

    [RelativeVirtualAddress(0x7C53D30)]
    public static partial QuestManager* Instance { get; }

    [RelativeVirtualAddress(0x61DFC0)]
    public partial void GetCurrentQuestId(uint* result);
    
    public uint CurrentQuestId
    {
        get
        {
            uint id = 0;
            GetCurrentQuestId(&id);
            
            return id;
        }
    }
    
    public bool IsCurrentlyPlaying(QuestId id) => Flow->QuestId == id;
}