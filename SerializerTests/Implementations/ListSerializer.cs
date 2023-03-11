using Newtonsoft.Json;
using SerializerTests.Nodes;

namespace SerializerTests.Implementations;

internal static class ListSerializer
{
    private static Dictionary<ListNode, int> CreateNodeIndexDict(ListNode list)
    {
        var nodeIndices = new Dictionary<ListNode, int>();
        var node = list;
        var index = 0;
        while (node != null && !nodeIndices.ContainsKey(node))
        {
            nodeIndices.Add(node, index);
            index++;
            node = node.Next;
        }

        return nodeIndices;
    }

    private static void WriteNode(ListNode node, IDictionary<ListNode, int> nodeIndices, JsonWriter writer)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Data");
        writer.WriteValue(node.Data);

        writer.WritePropertyName("Previous");
        writer.WriteValue(node.Previous == null ? null : nodeIndices[node.Previous]);

        writer.WritePropertyName("Next");
        writer.WriteValue(node.Next == null ? null : nodeIndices[node.Next]);

        writer.WritePropertyName("Random");
        writer.WriteValue(node.Random == null ? null : nodeIndices[node.Random]);

        writer.WriteEndObject();
    }

    public static void Serialize(ListNode head, Stream s)
    {
        var indices = CreateNodeIndexDict(head);

        var writer = new StreamWriter(s);
        using var jsonWriter = new JsonTextWriter(writer);
        jsonWriter.WriteStartArray();

        var next = head;
        while (next != null)
        {
            WriteNode(next, indices, jsonWriter);
            writer.Flush();

            next = next.Next;
        }

        jsonWriter.WriteEndArray();
        writer.Flush();
    }
}