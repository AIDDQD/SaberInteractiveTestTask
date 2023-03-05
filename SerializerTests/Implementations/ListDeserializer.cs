using System.Text;
using SerializerTests.Nodes;

namespace SerializerTests.Implementations;

internal readonly struct NodeInfo {
    public readonly ListNode Node;
    public readonly int? Previous;
    public readonly int? Next;
    public readonly int? Random;

    public NodeInfo(ListNode node, string data, int? previous, int? next, int? random) {
        Node = node;
        Node.Data = data;
        
        Previous = previous;
        Next = next;
        Random = random;
    }
}

internal static class ListDeserializer {
    public static ListNode Deserialize(Stream s) {
        int firstByte = s.ReadByte();
        if (firstByte is -1 or not '[')
        {
            throw new ArgumentException("Поток путой или начинается не с '['");
        }
        
        var nodeInfos = new List<NodeInfo>();
        var nodeRead = false;
        while (true)
        {
            var b = s.ReadByte();

            if (b == '{' && !nodeRead)
            {
                nodeRead = true;
                nodeInfos.Add(ParseNodeInfo(s));
            }
            else if (b == ',' && nodeRead)
            {
                nodeRead = false;
            }
            else if (b == -1)
            {
                throw new ArgumentException($"Неожиданный конец потока", nameof(s));
            }
            else if (b == ']')
            {
                break;
            }
            else {
                throw new ArgumentException($"Неожиданный символ '{(char) b}'", nameof(s));
            }
        }

        return FillNodes(nodeInfos);
    }

    private static ListNode FillNodes(IList<NodeInfo> nodeInfos) {
        if (!nodeInfos.Any())
            return null;
        
        foreach (var nodeInfo in nodeInfos)
        {
            if (nodeInfo.Next != null)
                nodeInfo.Node.Next = nodeInfos[nodeInfo.Next.Value].Node;
            if (nodeInfo.Previous != null)
                nodeInfo.Node.Previous = nodeInfos[nodeInfo.Previous.Value].Node;
            if (nodeInfo.Random != null)
                nodeInfo.Node.Random = nodeInfos[nodeInfo.Random.Value].Node;
        }
        
        return nodeInfos[0].Node;
    }

    private static NodeInfo ParseNodeInfo(Stream s) {
        var fields = new Dictionary<string, string>(4);
        string fieldName, value;
        for (int i = 0; i < 3; i++) {
            (fieldName, value) = ReadField(',', s);
            fields[fieldName] = value;
        }
        
        (fieldName, value) = ReadField('}', s);
        fields[fieldName] = value;

        return new NodeInfo(
            new ListNode(),
            fields["Data"], 
            fields["Previous"] == "null" ? null : int.Parse(fields["Previous"]), 
            fields["Next"] == "null" ? null : int.Parse(fields["Next"]),
            fields["Random"] == "null" ? null : int.Parse(fields["Random"])
            );
    }
    
    private static (string FieldName, string Value) ReadField(char stopChar, Stream s)
    {
        var line = ReadUntil(s, stopChar);
        var parts = line.Split('=', 2);
        return (parts[0], parts[1]);
    }
    
    private static string ReadUntil(Stream s, char stopChar)
    {
        var sb = new StringBuilder();
        while (true)
        {
            var b = s.ReadByte();
            
            if ((char) b == stopChar)
                break;

            switch (b) {
                case '\'':
                    sb.Append(ReadUntil(s, '\''));
                    break;
                case '"':
                    sb.Append(ReadUntil(s, '"'));
                    break;
                default:
                    sb.Append((char) b);
                    break;
            }
        }

        return sb.ToString();
    }
    
}