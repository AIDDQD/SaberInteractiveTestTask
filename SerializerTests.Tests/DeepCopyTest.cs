using SerializerTests.Implementations;
using SerializerTests.Interfaces;
using SerializerTests.Nodes;

namespace SerializerTests.Tests; 

public class DeepCopyTest {
    private IListSerializer _serializer;

    public DeepCopyTest() {
        _serializer = new PseudoJsonListSerializer();
    }
    
    [Fact]
    public void DeepCopy_NullInput_ReturnsNull()
    {
        ListNode input = null;

        var result = _serializer.DeepCopy(input).Result;
        
        Assert.Null(result);
    }
    
    [Fact]
    public void DeepCopy_SingleNode_ReturnsCopy()
    {
        ListNode input = new ListNode { Data = "node1" };
        
        var result = _serializer.DeepCopy(input).Result;
        
        Assert.NotEqual(input, result);
        Assert.Equal(input.Data, result.Data);
        Assert.Null(result.Previous);
        Assert.Null(result.Next);
        Assert.Null(result.Random);
    }
    
    [Fact]
    public void DeepCopy_RandomNode_Copied() {
        var node1 = new ListNode {
            Data = "test-test",
        };
        var node2 = new ListNode() {
            Data = "super-test",
        };
        var node3 = new ListNode() {
            Data = "super-ultra-test",
        };
        node1.Next = node2;
        node1.Random = node2;
        node2.Previous = node1;
        node2.Next = node3;
        node3.Previous = node2;

        var result = _serializer.DeepCopy(node1).Result;
        Assert.NotEqual(node1, result);
        Assert.Equal(node1.Data, result.Data);
        Assert.NotEqual(node2, result.Next);
        Assert.Equal(node2.Data, result.Next.Data);
        Assert.NotEqual(node3, result.Next.Next);
        Assert.Equal(node3.Data, result.Next.Next.Data);
        
        Assert.NotEqual(node1.Random, result.Random);
        Assert.Equal(node1.Random.Data, result.Random.Data);
        
        Assert.Null(result.Previous);
        Assert.Null(result.Next.Random);
        Assert.Null(result.Next.Next.Next);
        Assert.Null(result.Next.Next.Random);
    }
}