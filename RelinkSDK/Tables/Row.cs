namespace RelinkSDK.Tables;

public abstract class Row
{
    public int RowIndex { get; set; }
    
    public abstract void Deserialize(BinaryReader reader);
    public abstract void Serialize(BinaryWriter writer);
}