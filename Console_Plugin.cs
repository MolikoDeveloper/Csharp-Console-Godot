using Godot;

[Tool]
public partial class Console_Plugin : EditorPlugin
{
    public override void _EnterTree()
    {
        base._EnterTree();
        AddAutoloadSingleton("CommandConsole", "res://addons/csharpConsole/CommandConsole.cs");
    }
}
