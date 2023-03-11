using System.Text;
using SerializerTests.Implementations;
using SerializerTests.Interfaces;
using SerializerTests.Nodes;

namespace SerializerTests.Tests;

public class SerializeTest
{
    private readonly IListSerializer _serializer;

    public SerializeTest()
    {
        _serializer = new PseudoJsonListSerializer();
    }

    [Fact]
    public void Serialize_SingleNode()
    {
        var list = new ListNode();
        using var ms = new MemoryStream();

        _serializer.Serialize(list, ms);
        var result = Encoding.UTF8.GetString(ms.ToArray());
        Assert.Equal(@"[{""Data"":null,""Previous"":null,""Next"":null,""Random"":null}]", result);
    }

    [Fact]
    public void Serialize_WithNext()
    {
        var list = new ListNode
        {
            Data = "test-test"
        };
        var node1 = new ListNode
        {
            Data = "super-test"
        };
        list.Next = node1;
        node1.Previous = list;

        using var ms = new MemoryStream();
        _serializer.Serialize(list, ms);
        var result = Encoding.UTF8.GetString(ms.ToArray());
        Assert.Equal(
            @"[{""Data"":""test-test"",""Previous"":null,""Next"":1,""Random"":null}," +
            @"{""Data"":""super-test"",""Previous"":0,""Next"":null,""Random"":null}]",
            result);
    }

    [Fact]
    public void Serialize_WithRandom()
    {
        var node1 = new ListNode
        {
            Data = "test-test"
        };
        var node2 = new ListNode
        {
            Data = "super-test"
        };
        var node3 = new ListNode
        {
            Data = "super-ultra-test"
        };
        node1.Next = node2;
        node1.Random = node2;
        node2.Previous = node1;
        node2.Next = node3;
        node3.Previous = node2;

        using var ms = new MemoryStream();
        _serializer.Serialize(node1, ms);
        var result = Encoding.UTF8.GetString(ms.ToArray());
        Assert.Equal(
            @"[{""Data"":""test-test"",""Previous"":null,""Next"":1,""Random"":1}," +
            @"{""Data"":""super-test"",""Previous"":0,""Next"":2,""Random"":null}," +
            @"{""Data"":""super-ultra-test"",""Previous"":1,""Next"":null,""Random"":null}]",
            result);
    }
}