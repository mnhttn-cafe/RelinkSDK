using System.Runtime.InteropServices;
using RelinkSDK.Attributes;
using RelinkSDK.Quests;

namespace RelinkSDK.Bindings.Quests;

[Binding]
public unsafe partial struct ReceptionQuestFlowBase
{
    [FieldOffset(0x40)] public QuestId QuestId;
}
