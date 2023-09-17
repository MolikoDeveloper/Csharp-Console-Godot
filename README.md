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

        CommandConsole.AddCommand("heloworld", HelloWorld);
        CommandConsole.AddCommandDescription("heloworld", "Prints 'Hola Mundo!' in the console.");
    }

    void Print(string text)
    {
        GD.Print(text);
    }

    void HelloWorld()
    {
        GD.PrintErr("Hola Mundo!");
    }

    //also you can add the Attribute
    [AddCommand("testing"), AddCommandDescription("[color=red]Prints on GD Console[/color]")]
    public void testing(string text)
    {
        GD.Print(text);
    }
}

```

in game run with:
> `test` "testing the example" => set `testing the example` as param value

> `test` testing the example => set `testing` `the` `example` as params values

> `test` "testing the" example => set `testing the` `example` as params

![image](https://github.com/MolikoDeveloper/Csharp-Console-Godot/assets/58595683/d17ee243-80b2-47dc-9acf-477ce4562e2c)

command_list command:

![image](https://github.com/MolikoDeveloper/Csharp-Console-Godot/assets/58595683/5810666b-d237-406f-96fd-f655bd0f2feb)



print example:

![image](https://github.com/MolikoDeveloper/Csharp-Console-Godot/assets/58595683/4ebf6452-bbb5-4651-a0b0-a48eeb8148ae)


all with a maximum of 16 params.
