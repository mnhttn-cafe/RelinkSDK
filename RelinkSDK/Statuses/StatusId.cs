namespace RelinkSDK.Statuses;

/// <summary>
/// All the status IDs for Granblue Fantasy Relink
/// </summary>
public enum StatusId : uint 
{
    AttackUp = 0x0,
    DefenseUp = 0x1,
    AttackDown = 0x2,
    DefenseDown = 0x3,
    DamageCut = 0x4,
    Regen = 0x5,
    
    SkillSealed = 0x3F8, 
    SkyboundArtsSealed = 0x3F9
}