namespace Json.MSBuild;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

public class ValidationTests
{
    private Mock<IBuildEngine> buildEngineMock = new ();
    private List<BuildErrorEventArgs> errors = new ();

    public ValidationTests()
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
        var task = new JsonValidator
        {
            BuildEngine = this.buildEngineMock.Object,
            Files = new[]
            {
                new TaskItem("json/no-schema-valid.json"),
            },
        };

        var result = task.Execute();

        Assert.True(result);
        Assert.Empty(this.errors);
    }

    [Fact]
    public void Execute_InValidJsonWithNoSchema_Fail()
    {
        var task = new JsonValidator
        {
            BuildEngine = this.buildEngineMock.Object,
            Files = new[]
            {
                new TaskItem("json/no-schema-invalid.json"),
            },
        };

        var result = task.Execute();

        Assert.False(result);
        Assert.NotEmpty(this.errors);
    }

    [Fact]
    public void Execute_ValidJsonWithSchema_Success()
    {
        var task = new JsonValidator
        {
            BuildEngine = this.buildEngineMock.Object,
            Files = new[]
            {
                new TaskItem("json/with-schema-valid.json"),
            },
        };

        var result = task.Execute();

        Assert.True(result);
        Assert.Empty(this.errors);
    }

    [Fact]
    public void Execute_InValidJsonWithSchema_Fail()
    {
        var task = new JsonValidator
        {
            BuildEngine = this.buildEngineMock.Object,
            Files = new[]
            {
                new TaskItem("json/with-schema-invalid.json"),
            },
        };

        var result = task.Execute();

        Assert.True(result);
        Assert.Empty(this.errors);
    }
}
