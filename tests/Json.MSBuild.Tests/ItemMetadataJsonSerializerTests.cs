namespace Json.MSBuild;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

public class ItemMetadataJsonSerializerTests
{
    private Mock<IBuildEngine> buildEngineMock = new ();
    private List<BuildErrorEventArgs> errors = new ();

    public ItemMetadataJsonSerializerTests()
    {
        this.buildEngineMock.Setup(x => x.LogErrorEvent(It.IsAny<BuildErrorEventArgs>()))
            .Callback<BuildErrorEventArgs>(x => this.errors.Add(x));
    }

    public void Dispose()
    {
        this.buildEngineMock.VerifyAll();
    }

    [Fact]
    public void Execute_ValidJsonWithNoSchema_Success()
    {
        var task = new ItemMetadataJsonSerializer
        {
            BuildEngine = this.buildEngineMock.Object,
            Items = new[]
            {
                new TaskItem("1", new Dictionary<string, string>
                {
                    { "a", "1" },
                    { "b", "2" },
                }),
            },
            MetadataToSerialize = new string[] { "a", "b" },
        };

        var result = task.Execute();

        Assert.True(result);
        Assert.Empty(this.errors);
        Assert.Equal("[{\"b\":\"2\"},{\"a\":\"1\"}]", task.Json);
    }
}
