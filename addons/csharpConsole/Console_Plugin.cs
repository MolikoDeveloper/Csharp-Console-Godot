using Godot;

[Tool]
public partial class Console_Plugin : EditorPlugin
{
    public override void _EnterTree()
    {
        base._EnterTree();
        AddAutoloadSingleton("CommandConsole", "res://addons/csharpConsole/CommandConsole.cs");
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        RemoveAutoloadSingleton("CommandConsole");
    }
}
