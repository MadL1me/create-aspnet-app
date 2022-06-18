using System.Threading.Tasks;
using CliFx.Infrastructure;
using CreateAspnetApp.Commands;
using Moq;
using NUnit.Framework;
using Sharprompt;
using Sharprompt.Prompts;

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

        var mock = new Mock<IPrompt>();
        mock.Setup(p => p.Input<string>("What's your name?", 
            null, null, null)).Returns("MadL1me");
        
        Prompt.PromptRealisation = mock.Object;
        
        var command = new CreateWebAppCommand();

        // Act
        await command.ExecuteAsync(console);

        var stdOut = console.ReadOutputString();

        // Assert
        Assert.That(stdOut, Is.EqualTo("Hello, MadL1me!\r\n"));
    }
}

