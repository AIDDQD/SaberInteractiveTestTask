using Newtonsoft.Json;
using SerializerTests.Nodes;

namespace SerializerTests.Implementations;

internal readonly struct NodeInfo
{
    public readonly ListNode Node;
    public readonly int? Previous;
    public readonly int? Next;
    public readonly int? Random;

    public NodeInfo(ListNode node, string data, int? previous, int? next, int? random)
    {
        Node = node;
        Node.Data = data;

        Previous = previous;
        Next = next;
        Random = random;
    }
}

internal static class ListDeserializer
{
    public static async Task<ListNode> Deserialize(Stream s)
    {
        using var streamReader = new StreamReader(s);
        await using var jsonReader = new JsonTextReader(streamReader);

        var canRead = await jsonReader.ReadAsync();
        if (!canRead || jsonReader.TokenType != JsonToken.StartArray)
            throw new ArgumentException("Конец потока или получен неожиданный токен", nameof(s));

        var nodeInfos = new List<NodeInfo>();
        try
        {
            while (await jsonReader.ReadAsync())
            {
                if (jsonReader.TokenType is not JsonToken.StartObject)
                    continue;

                var nodeInfo = await ParseNodeInfoAsync(jsonReader);
                nodeInfos.Add(nodeInfo);
            }
        }
        catch (JsonReaderException ex)
        {
            throw new ArgumentException("Ошибка чтения данных из потока", ex);
        }

        return ConnectNodes(nodeInfos);
    }

    private static ListNode ConnectNodes(IList<NodeInfo> nodeInfos)
    {
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

    private static async Task<NodeInfo> ParseNodeInfoAsync(JsonReader reader)
    {
        var fields = new Dictionary<string, string>(4);
        for (var i = 0; i < 4; i++)
        {
            if (reader.TokenType is not JsonToken.PropertyName)
                await reader.ReadAsync();

            var fieldName = reader.Value?.ToString();
            var value = await reader.ReadAsStringAsync();
            fields[fieldName] = value;
        }

        return new NodeInfo(
            new ListNode(),
            fields["Data"],
            fields["Previous"] == null ? null : int.Parse(fields["Previous"]),
            fields["Next"] == null ? null : int.Parse(fields["Next"]),
            fields["Random"] == null ? null : int.Parse(fields["Random"])
        );
    }
}