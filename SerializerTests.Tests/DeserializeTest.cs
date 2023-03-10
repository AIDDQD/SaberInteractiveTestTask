using System.Text;
using SerializerTests.Implementations;
using SerializerTests.Interfaces;

namespace SerializerTests.Tests;

public class DeserializeTest
{
    private readonly IListSerializer _serializer;

    public DeserializeTest()
    {
        _serializer = new JsonListSerializer();
    }

    [Fact]
    public async Task Deserialize_InvalidList()
    {
        var raw = @"[]";

        using var ms = new MemoryStream();
        ms.Write(Encoding.UTF8.GetBytes(raw));
        ms.Seek(0, SeekOrigin.Begin);

        var result = await _serializer.Deserialize(ms);
        Assert.Null(result);
    }

    [Fact]
    public async Task Deserialize_SingleItem()
    {
        var raw = @"[{""Data"":""test-test"",""Previous"":null,""Next"":null,""Random"":null}]";

        using var ms = new MemoryStream();
        ms.Write(Encoding.UTF8.GetBytes(raw));
        ms.Seek(0, SeekOrigin.Begin);

        var result = await _serializer.Deserialize(ms);
        Assert.Equal("test-test", result.Data);
        Assert.Null(result.Next);
        Assert.Null(result.Previous);
        Assert.Null(result.Random);
    }

    [Fact]
    public async Task Deserialize_TwoItems()
    {
        var raw =
            @"[{""Data"":""test-test"",""Previous"":null,""Next"":1,""Random"":null},{""Data"":""super-test"",""Previous"":0,""Next"":null,""Random"":null},]";

        using var ms = new MemoryStream();
        ms.Write(Encoding.UTF8.GetBytes(raw));
        ms.Seek(0, SeekOrigin.Begin);

        var result = await _serializer.Deserialize(ms);
        Assert.Equal("test-test", result.Data);
        Assert.NotNull(result.Next);
        Assert.Null(result.Previous);
        Assert.Null(result.Random);

        Assert.Equal("super-test", result.Next.Data);
        Assert.Equal(result.Data, result.Next.Previous.Data);
        Assert.Null(result.Next.Next);
        Assert.Null(result.Next.Random);
    }

    [Fact]
    public async Task Deserialize_Random1()
    {
        var raw =
            @"[{""Data"":""test-test"",""Previous"":null,""Next"":1,""Random"":1},{""Data"":""super-test"",""Previous"":0,""Next"":null,""Random"":null},]";

        using var ms = new MemoryStream();
        ms.Write(Encoding.UTF8.GetBytes(raw));
        ms.Seek(0, SeekOrigin.Begin);

        var result = await _serializer.Deserialize(ms);
        Assert.Equal(result.Next.Data, result.Random.Data);
        Assert.Null(result.Random.Next);
        Assert.Null(result.Random.Random);
    }

    /*
    // ???????? ???????? ???? ?????????? ????????????????, ?????????????????? ?? Data ?????????????????? ???????????????????????????????? ?????????????? ?? ???????????? ?????????? ????????????????.
    // ???? ?? ?????? ?????????????? ?? ??????????????????????, ???????? ???????????? ???? ???????? ????????????????????.
    [Fact]
    public void Deserialize_Random2()
    {
        var raw =
            @"[{""Data"":"""""",""Previous"":null,""Next"":1,""Random"":1},{""Data"":""super-test"",""Previous"":0,""Next"":null,""Random"":null},]";

        using var ms = new MemoryStream();
        ms.Write(Encoding.UTF8.GetBytes(raw));
        ms.Seek(0, SeekOrigin.Begin);

        var result = _serializer.Deserialize(ms).Result;
        Assert.Equal(result.Next.Data, result.Random.Data);
        Assert.Null(result.Random.Next);
        Assert.Null(result.Random.Random);
    }*/
}