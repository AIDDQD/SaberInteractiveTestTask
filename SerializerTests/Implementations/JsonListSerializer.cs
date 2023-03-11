using SerializerTests.Interfaces;
using SerializerTests.Nodes;

namespace SerializerTests.Implementations;

public class JsonListSerializer : IListSerializer
{
    public Task Serialize(ListNode head, Stream s)
    {
        return ListSerializer.Serialize(head, s);
    }

    public Task<ListNode> Deserialize(Stream s)
    {
        return ListDeserializer.Deserialize(s);
    }

    public Task<ListNode> DeepCopy(ListNode head)
    {
        return Task.FromResult(DeepCopier.DeepCopy(head));
    }
}