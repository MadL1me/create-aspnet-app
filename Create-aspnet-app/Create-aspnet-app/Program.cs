using CliFx;
using Sharprompt;

var name = Prompt.Input<string>("What's your name?");
Console.WriteLine($"Hello, {name}!");

// Password input
var secret = Prompt.Password("Type new password", validators: new[] { Validators.Required(), Validators.MinLength(8) });
Console.WriteLine("Password OK");

// Confirmation
var answer = Prompt.Confirm("Are you ready?", defaultValue: true);
Console.WriteLine($"Your answer is {answer}");

await new CliApplicationBuilder()
    .AddCommandsFromThisAssembly()
    .Build()
    .RunAsync();
