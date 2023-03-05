using SerializerTests.Interfaces;
using SerializerTests.Nodes;

namespace SerializerTests.Implementations;

public class PseudoJsonListSerializer : IListSerializer
{
    public PseudoJsonListSerializer()
    {
    }

    public Task Serialize(ListNode head, Stream s) {
        ListSerializer.Serialize(head, s);
        return Task.CompletedTask;
    }

    public Task<ListNode> Deserialize(Stream s) {
        return Task.FromResult(ListDeserializer.Deserialize(s));
    }

    public Task<ListNode> DeepCopy(ListNode head) {
        return Task.FromResult(DeepCopier.DeepCopy(head));
    }
}