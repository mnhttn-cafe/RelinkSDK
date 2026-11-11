using System.Buffers;
using System.Collections;
using System.Globalization;
using MessagePack;

namespace RelinkSDK.Msg;

/// <summary>
/// 
/// </summary>
public class MessagePackNode : IEnumerable<MessagePackNode>
{
    public MessagePackType Type { get; private set; }
    public byte[] Bytes { get; private set; }
    public List<(MessagePackNode? Key, MessagePackNode Value)> Children { get; private set; }
    public string? Name => Type == MessagePackType.String ? MessagePackSerializer.Deserialize<string>(Bytes) : null;
    
    private delegate void Encoder(ref MessagePackWriter writer);

    private readonly int _count;

    private MessagePackNode(MessagePackType type, byte[] bytes, int count)
    {
        Type = type;
        Bytes = bytes;
        Children = new();
        _count = count;
    }

    public MessagePackNode Set(string value)
    {
        if (Type != MessagePackType.String)
        {
            throw new InvalidOperationException($"Can't set a string on {Type}");
        }

        Bytes = MessagePackSerializer.Serialize(value);
        return this;
    }

    public MessagePackNode Set(double value)
    {
        if (Type == MessagePackType.String)
        {
            Bytes = MessagePackSerializer.Serialize(value.ToString(CultureInfo.InvariantCulture));
        }
        else if (Type == MessagePackType.Integer && double.IsInteger(value))
        {
            Bytes = MessagePackSerializer.Serialize((long)value);
        }
        else if (Type is MessagePackType.Integer or MessagePackType.Float)
        {
            Type = MessagePackType.Float;
            Bytes = MessagePackSerializer.Serialize((float)value);
        }
        else
        {
            throw new InvalidOperationException($"Can't set a number on {Type}");
        }

        return this;
    }

    public MessagePackNode Set(long value)
    {
        if (Type == MessagePackType.String)
        {
            Bytes = MessagePackSerializer.Serialize(value.ToString(CultureInfo.InvariantCulture));
        }
        else if (Type == MessagePackType.Integer)
        {
            Bytes = MessagePackSerializer.Serialize(value);
        }
        else if (Type == MessagePackType.Float)
        {
            Bytes = MessagePackSerializer.Serialize((float)value);
        }
        else
        {
            throw new InvalidOperationException($"Can't set a number on {Type}");
        }

        return this;
    }

    public MessagePackNode Set(bool value)
    {
        if (Type == MessagePackType.String)
        {
            Bytes = MessagePackSerializer.Serialize(value ? "true" : "false");
        }
        else if (Type == MessagePackType.Boolean)
        {
            Bytes = MessagePackSerializer.Serialize(value);
        }
        else
        {
            throw new InvalidOperationException($"Can't set a bool on {Type}");
        }

        return this;
    }

    public MessagePackNode Set(string key, string value)
    {
        foreach (var node in Each(key))
        {
            node.Set(value);
        }

        return this;
    }

    public MessagePackNode Set(string key, double value)
    {
        foreach (var node in Each(key))
        {
            node.Set(value);
        }

        return this;
    }

    public MessagePackNode Set(string key, long value)
    {
        foreach (var node in Each(key))
        {
            node.Set(value);
        }

        return this;
    }

    public MessagePackNode Set(string key, bool value)
    {
        foreach (var node in Each(key))
        {
            node.Set(value);
        }

        return this;
    }

    public MessagePackNode With(string key, Action<MessagePackNode> edit)
    {
        foreach (var node in Each(key))
        {
            edit(node);
        }

        return this;
    }

    public IEnumerable<MessagePackNode> All(string key)
    {
        return Children.Where(c => c.Key?.Name == key).Select(c => c.Value);
    }

    private List<MessagePackNode> Each(string key)
    {
        var nodes = All(key).ToList();
        if (nodes.Count == 0)
        {
            throw new KeyNotFoundException($"No {key} found on this node");
        }

        return nodes;
    }

    public IEnumerator<MessagePackNode> GetEnumerator()
    {
        return Children.Select(c => c.Value).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static MessagePackNode Create(ref MessagePackReader reader)
    {
        var start = reader.Position;
        var type = reader.NextMessagePackType;

        switch (type)
        {
            case MessagePackType.Map:
            {
                var count = reader.ReadMapHeader();
                var node = new MessagePackNode(type, reader.Sequence.Slice(start, reader.Position).ToArray(), count);
                for (var i = 0; i < count; i++)
                {
                    var key = Create(ref reader);
                    node.Children.Add((key, Create(ref reader)));
                }
                return node;
            }
            
            case MessagePackType.Array:
            {
                var count = reader.ReadArrayHeader();
                var node = new MessagePackNode(type, reader.Sequence.Slice(start, reader.Position).ToArray(), count);
                for (var i = 0; i < count; i++)
                {
                    node.Children.Add((null, Create(ref reader)));
                }
                return node;
            }
            
            default:
                return new MessagePackNode(type, reader.ReadRaw().ToArray(), 0);
        }
    }

    public void Write(ref MessagePackWriter writer)
    {
        if (Children.Count == _count)
        {
            writer.WriteRaw(Bytes);
        }
        else if (Type == MessagePackType.Map)
        {
            writer.WriteMapHeader(Children.Count);
        }
        else if (Type == MessagePackType.Array)
        {
            writer.WriteArrayHeader(Children.Count);
        }
        else
        {
            throw new InvalidOperationException($"Can't write children on {Type}");
        }

        foreach (var (key, value) in Children)
        {
            key?.Write(ref writer);
            value.Write(ref writer);
        }
    }
}