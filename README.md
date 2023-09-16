# Csharp-Console-Godot
original project:
[Godot Console](https://github.com/jitspoe/godot-console)

example:
```c#
using Godot;
public partial class Example : Node
{
    public override void _Ready()
    {
        CommandConsole.AddCommand("print", Print);
        CommandConsole.AddCommandDescription("print", "Prints the given text in the console.");
        CommandConsole.AddParameterDescription(CommandName: "print", param:"text", description:"The text to print.");

        CommandConsole.AddCommand("hi", HelloWorld);
        CommandConsole.AddCommandDescription("hi", "Prints 'Hola Mundo!' in the console.");
    }

    void Print(string text)
    {
        GD.Print(text);
    }

    void HelloWorld()
    {
        GD.PrintErr("Hola Mundo!");
    }
}

```

in game run with:
> `test` "testing the example" => set `testing the example` as param value

> `test` testing the example => set `testing` `the` `example` as params values

> `test` "testing the" example => set `testing the` `example` as params

all with a maximum of 16 params.
