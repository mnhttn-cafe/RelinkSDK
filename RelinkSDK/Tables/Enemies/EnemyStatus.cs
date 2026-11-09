namespace RelinkSDK.Tables.Enemies;

public class EnemyStatus : Table<EnemyStatus.EnemyStatusRow>
{
    public override string Path { get; } = "system/table/enemy_status.tbl";
    public override long RowSize { get; } = 0x18;

    public EnemyStatus(byte[] bytes)
        : base(bytes)
    {
    }

    public class EnemyStatusRow : Row
    {
        public long Health { get; set; }
        public long Attack { get; set; }
        public uint StatusKey { get; set; }
        public uint StatInterpolation { get; set; }


        public override void Deserialize(BinaryReader reader)
        {
            Health = reader.ReadInt64();
            Attack = reader.ReadInt64();
            StatusKey = reader.ReadUInt32();
            StatInterpolation = reader.ReadUInt32();
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Health);
            writer.Write(Attack);
            writer.Write(StatusKey);
            writer.Write(StatInterpolation);
        }
    }
}