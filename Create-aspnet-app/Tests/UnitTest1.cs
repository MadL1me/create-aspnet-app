using System.Threading.Tasks;
using CliFx.Infrastructure;
using Create_aspnet_app.Commands;
using NUnit.Framework;

namespace Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task ConcatCommand_executes_successfully()
    {
        using var console = new FakeInMemoryConsole();

        var command = new ConcatCommand
        {
            Left = "foo",
            Right = "bar"
        };

        // Act
        await command.ExecuteAsync(console);

        var stdOut = console.ReadOutputString();

        // Assert
        Assert.That(stdOut, Is.EqualTo("foo bar"));
    }
}

