using API.Entities;
using API.Utilities;

namespace APITests.Utilities;

public class RateLimitOverridesSerializerTests
{
    [Fact]
    public void Serializer_Produces_EmptyString_For_Empty_Input()
    {
        var data = new RateLimitOverrides();
        var output = RateLimitOverridesSerializer.Serialize(data);

        Assert.Equal(string.Empty, output);
    }

    [Fact]
    public void Serializer_Produces_CamelCase()
    {
        var data = new RateLimitOverrides() { PermitLimit = 30, Window = 60 };
        var output = RateLimitOverridesSerializer.Serialize(data);

        Assert.Contains("permitLimit", output);
        Assert.Contains("window", output);
    }

    [Fact]
    public void Serializer_Ignores_Null_Fields()
    {
        var data = new RateLimitOverrides() { PermitLimit = 30 };
        var output = RateLimitOverridesSerializer.Serialize(data);

        Assert.Null(data.Window);
        Assert.DoesNotContain("window", output);
    }

    [Fact]
    public void Serializer_Produces_Expected()
    {
        var data = new RateLimitOverrides() { PermitLimit = 30, Window = 60 };
        var output = RateLimitOverridesSerializer.Serialize(data);

        Assert.Equal("{\"permitLimit\":30,\"window\":60}", output);
    }

    [Theory]
    [InlineData("{\"permitLimit\":30}", 30, null)]
    [InlineData("{\"window\":60}", null, 60)]
    [InlineData("{\"permitLimit\":30,\"window\":60}", 30, 60)]
    public void Deserializer_Produces_Expected(string input, int? permitLimit, int? window)
    {
        RateLimitOverrides? output = RateLimitOverridesSerializer.Deserialize(input);

        Assert.NotNull(output);
        Assert.Equal(permitLimit, output.PermitLimit);
        Assert.Equal(window, output.Window);
    }

    [Fact]
    public void Round_Trip_Data_Is_Equal()
    {
        var origOverrides = new RateLimitOverrides() { PermitLimit = 30, Window = 60 };

        var serializedData = RateLimitOverridesSerializer.Serialize(origOverrides);
        RateLimitOverrides? deserializedData = RateLimitOverridesSerializer.Deserialize(serializedData);

        Assert.Equivalent(origOverrides, deserializedData, true);
    }
}
