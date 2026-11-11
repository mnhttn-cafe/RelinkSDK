using System.Buffers;
using MessagePack;

namespace RelinkSDK.Msg;

/// <summary>
/// Wrapper class that builds on top of MessagePack to read/write
/// .msg files
///
/// Mostly exists because certain files have duplicate parameters,
/// which most JSON parsers would either throw on and/or probably
/// silently lose.
/// </summary>
public class MessagePackFile
{
    public List<MessagePackNode> Nodes { get; private set; }
    public MessagePackNode Root => Nodes[0];

    public MessagePackFile(byte[] bytes)
    {
        Nodes = new();

        var reader = new MessagePackReader(bytes);
        while (!reader.End)
        {
            Nodes.Add(MessagePackNode.Create(ref reader));
        }
    }

    public byte[] ToArray()
    {
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new MessagePackWriter(buffer);
        
        foreach (var root in Nodes)
        {
            root.Write(ref writer);
        }

        writer.Flush();
        return buffer.WrittenSpan.ToArray();
    }
}