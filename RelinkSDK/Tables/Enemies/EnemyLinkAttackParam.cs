namespace RelinkSDK.Tables.Enemies;

public class  EnemyLinkAttackParam : Table<EnemyLinkAttackParam.EnemyLinkAttackParamRow>
{
    public override string Path { get; } = "system/table/enemy_link_attack_param.tbl";
    public override long RowSize { get; } = 0x20;

    public EnemyLinkAttackParam(byte[] bytes)
        : base(bytes)
    {
    }

    public class EnemyLinkAttackParamRow : Row
    {
        public uint Id { get; set; }
        public float GaugeMax { get; set; }

        public override void Deserialize(BinaryReader reader)
        {
            Id = reader.ReadUInt32();
            GaugeMax = reader.ReadSingle();
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(GaugeMax);
        }
    }
}
