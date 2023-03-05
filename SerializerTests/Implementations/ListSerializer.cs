using System.Text;
using SerializerTests.Nodes;

namespace SerializerTests.Implementations; 

internal static class ListSerializer {
    private static Dictionary<ListNode, int> CreateNodeIndexDict(ListNode list) {
        var nodeIndices = new Dictionary<ListNode, int>();
        var node = list;
        var index = 0;
        while (node != null && !nodeIndices.ContainsKey(node)) {
            nodeIndices.Add(node, index);
            index++;
            node = node.Next;
        }
        
        return nodeIndices;
    }

    private static void WriteNode(ListNode node, IDictionary<ListNode, int> nodeIndices, TextWriter writer) {
        writer.Write('{');
        writer.Write($"Data=\"{node.Data}\",");
        writer.Write($"Previous={(node.Previous == null ? "null" : nodeIndices[node.Previous].ToString())},");
        writer.Write($"Next={(node.Next == null ? "null" : nodeIndices[node.Next].ToString())},");
        writer.Write($"Random={(node.Random == null ? "null" : nodeIndices[node.Random].ToString())}");
        writer.Write("}");
    }
    
    public static void Serialize(ListNode head, Stream s) {
        var indices = CreateNodeIndexDict(head);

        var writer = new StreamWriter(s); 
        writer.Write('[');
        
        var next = head; 
        while (next != null) {
            WriteNode(next, indices, writer);
            next = next.Next;
            writer.Write(',');
            writer.Flush();
        }
        
        writer.Write(']');
        writer.Flush();
    }
}