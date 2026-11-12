namespace RelinkSDK.Tables.Enemies;

public class Enemy : Table<Enemy.EnemyRow>
{
    public override string Path { get; } = "system/table/enemy.tbl";
    public override long RowSize { get; } = 0x2D0;

    public Enemy(byte[] bytes)
        : base(bytes)
    {
    }

    public class EnemyRow : Row
    {
        public byte[] Name { get; set; } = new byte[8];
        // one per difficulty
        public uint[] LinkParamIds { get; set; } = new uint[5];

        public override void Deserialize(BinaryReader reader)
        {
            long start = reader.BaseStream.Position;
            Name = reader.ReadBytes(8);
            reader.BaseStream.Seek(0x210, SeekOrigin.Current);
            
            for (int i = 0; i < LinkParamIds.Length; i++)
            {
                LinkParamIds[i] = reader.ReadUInt32();
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Name);

            writer.BaseStream.Seek(0x210, SeekOrigin.Current);
            foreach (uint id in LinkParamIds)
            {
                writer.Write(id);
            }
        }
    }
}
