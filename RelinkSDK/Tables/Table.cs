namespace RelinkSDK.Tables;

/// <summary>
/// Managed representation of .tbl files in Granblue Fantasy Relink
/// </summary>
public abstract class Table<T> where T : Row, new()
{
    private readonly byte[] _bytes;
    private long _rowCount;
    
    public abstract string Name { get; }
    public abstract long RowSize { get; }
    
    public List<T> Rows { get; private set; }
    
    public Table(byte[] bytes)
    {
        _bytes = bytes;

        Read();
    }

    public void Read()
    {
        using (BinaryReader reader = new BinaryReader(new MemoryStream(_bytes)))
        {
            var rowList = new List<T>();
            _rowCount = reader.ReadInt64();

            if (sizeof(long) + (RowSize * _rowCount) != _bytes.Length)
            {
                throw new InvalidDataException("The number of bytes does not match the number of rows");
            }

            foreach (T row in IterateRow(reader))
            {
                rowList.Add(row);
            }

            Rows = rowList;
        }
    }

    public byte[] Write()
    {
        // needs to be copied because if we don't,
        // then we lose fields if everything isn't read
        byte[] copyOfRows = (byte[])_bytes.Clone();
        
        using (MemoryStream stream = new MemoryStream(copyOfRows))
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            foreach (T row in Rows)
            {
                writer.BaseStream.Position = sizeof(long) + row.RowIndex * RowSize;
                row.Serialize(writer);
            }

            return copyOfRows;
        }
    }

    private IEnumerable<T> IterateRow(BinaryReader reader)
    {
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            long start = reader.BaseStream.Position;

            T row = new();
            row.RowIndex = (int)((start - sizeof(long)) / RowSize);
            row.Deserialize(reader);

            // a row may not read all of its bytes
            //
            // so we just affix the stream's position
            // to where it would be as if it did
            reader.BaseStream.Position = start + RowSize;

            yield return row;
        }
    }
}