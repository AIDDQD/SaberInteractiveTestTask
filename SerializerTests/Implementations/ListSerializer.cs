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

    private static async Task WriteNodeAsync(ListNode node, IDictionary<ListNode, int> nodeIndices, JsonWriter writer)
    {
        await writer.WriteStartObjectAsync();

        await writer.WritePropertyNameAsync("Data");
        await writer.WriteValueAsync(node.Data);

        await writer.WritePropertyNameAsync("Previous");
        await writer.WriteValueAsync(node.Previous == null ? null : nodeIndices[node.Previous]);

        await writer.WritePropertyNameAsync("Next");
        await writer.WriteValueAsync(node.Next == null ? null : nodeIndices[node.Next]);

        await writer.WritePropertyNameAsync("Random");
        await writer.WriteValueAsync(node.Random == null ? null : nodeIndices[node.Random]);

        await writer.WriteEndObjectAsync();
    }

    public static async Task Serialize(ListNode head, Stream s)
    {
        var indices = CreateNodeIndexDict(head);

        var writer = new StreamWriter(s);
        await using var jsonWriter = new JsonTextWriter(writer);
        await jsonWriter.WriteStartArrayAsync();

        var next = head;
        while (next != null)
        {
            await WriteNodeAsync(next, indices, jsonWriter);
            await writer.FlushAsync();

            next = next.Next;
        }

        await jsonWriter.WriteEndArrayAsync();
        await writer.FlushAsync();
    }
}