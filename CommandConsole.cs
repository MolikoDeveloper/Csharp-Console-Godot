using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CommandConsole : Node
{
	public event Action console_opened;
	public event Action console_closed;
	public event Action console_unknown_command;


	class ConsoleCommand
	{
		public Callable function;
		public int param_count;
		public string Description { get; set; }

		public ConsoleCommand(Callable in_function, int in_param_count)
		{
			this.function = in_function;
			this.param_count = in_param_count;
		}
	}

	Control control;
	RichTextLabel rich_label;
	LineEdit line_edit;

	Dictionary<string, ConsoleCommand> ConsoleCommands = new Dictionary<string, ConsoleCommand>();
	List<string> console_history = new List<string>();
	int console_history_index = 0;


	List<string> Suggestions = new List<string>();
	int CurrentSuggestion = 0;
	bool Suggesting = false;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.control = new Control();
		this.rich_label = new RichTextLabel();
		this.line_edit = new LineEdit();

		CanvasLayer canvas = new CanvasLayer();
		canvas.Layer = 3;
		AddChild(canvas);
		control.AnchorBottom = 1;
		control.AnchorRight = 1;
		canvas.AddChild(control);

		rich_label.ScrollFollowing = true;
		rich_label.AnchorRight = 1;
		rich_label.AnchorBottom = 1;
		rich_label.AddThemeStyleboxOverride("normal", (StyleBox)ResourceLoader.Load("res://addons/console/console_background.tres"));
		control.AddChild(rich_label);

		rich_label.Text = "Development Console. \n";
		line_edit.AnchorTop = 0.5f;
		line_edit.AnchorRight = 1;
		line_edit.AnchorBottom = 0.5f;

		control.AddChild(line_edit);
		line_edit.TextSubmitted += OnTextSubmited;
		line_edit.TextChanged += OnTextChanged;
		control.Visible = false;
		this.ProcessMode = ProcessModeEnum.Always;

		AddCommand("quit", Quit, 0);
		AddCommand("exit", Quit, 0);
		AddCommand("help", Help, 0);
		AddCommand("clear", clear, 0);
		AddCommand("delete_history", DeleteHistory, 0);
		AddCommand("comandos", CommandList, 0);

	}

	private void OnTextChanged(string newText)
	{
		ResetAutoComplete();
	}

	void OnTextSubmited(string text)
	{
		ScrollToBottom();
		ResetAutoComplete();
		line_edit.Clear();
		AddInputHistory(text);
		PrintLine(text);

		var splitText = text.Split(' ');
		if (splitText.Length > 0)
		{
			string commandString = splitText[0].ToLower();
			if (ConsoleCommands.ContainsKey(commandString))
			{
				ConsoleCommand commandEntry = ConsoleCommands[commandString];

				switch (commandEntry.param_count)
				{
					case 0:
						commandEntry.function.Call();
						break;
					case > 0:
						for (int i = 1; i < splitText.Length; i++)
						{
							commandEntry.function.Call(splitText[i]);
						}
						break;
				}
			}
			else
			{
				console_unknown_command?.Invoke();
				PrintLine("Command not found.");

			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			// ~ key
			if (eventKey.Keycode == Key.Quoteleft)
			{
				if (eventKey.Pressed)
				{
					ToggleConsole();
				}
				GetTree().Root.SetInputAsHandled();
			}
			//toggle console size
			else if (eventKey.Keycode == Key.Quoteleft && eventKey.IsCommandOrControlPressed())
			{
				if (eventKey.Pressed)
				{
					if (control.Visible)
						ToggleSize();
					else
					{
						ToggleConsole();
						ToggleSize();
					}
					GetTree().Root.SetInputAsHandled();
				}
			}
			//disable control on ESC
			else if (eventKey.Keycode == Key.Escape && control.Visible)
			{
				if (eventKey.Pressed)
					ToggleConsole();
				GetTree().Root.SetInputAsHandled();
			}

			if (control.Visible && eventKey.Pressed)
			{
				if (eventKey.Keycode == Key.Up)
				{
					GetTree().Root.SetInputAsHandled();
					if (console_history_index > 0)
					{
						console_history_index--;
						line_edit.Text = console_history[console_history_index];
						line_edit.CaretColumn = line_edit.Text.Length;
					}
				}
				if (eventKey.Keycode == Key.Down)
				{
					GetTree().Root.SetInputAsHandled();
					if (console_history_index < console_history.Count)
					{
						console_history_index++;
						if (console_history_index == console_history.Count)
						{
							line_edit.Text = console_history[console_history_index];
							line_edit.CaretColumn = line_edit.Text.Length;
							ResetAutoComplete();
						}
						else
						{
							line_edit.Text = string.Empty;
							ResetAutoComplete();
						}
					}
				}
				if (eventKey.Keycode == Key.Pageup)
				{
					VScrollBar scroll = rich_label.GetVScrollBar();
					scroll.Value -= scroll.Page - scroll.Page * 0.1;
					GetTree().Root.SetInputAsHandled();
				}
				if (eventKey.Keycode == Key.Pagedown)
				{
					VScrollBar scroll = rich_label.GetVScrollBar();
					scroll.Value += scroll.Page - scroll.Page * 0.1;
					GetTree().Root.SetInputAsHandled();
				}
				if (eventKey.Keycode == Key.Tab)
				{
					AutoComplete();
					GetTree().Root.SetInputAsHandled();
				}
			}
		}
	}

	void ToggleConsole()
	{
		control.Visible = !control.Visible;
		if (control.Visible)
		{
			GetTree().Paused = true;
			line_edit.GrabFocus();
			console_opened?.Invoke();
		}
		else
		{
			control.AnchorBottom = 1f;
			GetTree().Paused = false;
			ScrollToBottom();
			ResetAutoComplete();
			console_closed?.Invoke();
		}
	}
	void ToggleSize()
	{
		if (control.AnchorBottom == 1.0f)
		{
			control.AnchorBottom = 1.9f;
		}
		else
		{
			control.AnchorBottom = 1.0f;
		}
	}
	void AutoComplete()
	{
		if (Suggesting)
		{
			for (int i = 0; i < Suggestions.Count - 1; i++)
			{
				if (CurrentSuggestion == i)
				{
					line_edit.Text = Suggestions[i];
					line_edit.CaretColumn = line_edit.Text.Length;
					if (CurrentSuggestion == Suggestions.Count - 1)
					{
						CurrentSuggestion = 0;
					}
					else
					{
						CurrentSuggestion++;
					}
					return;
				}
			}
		}
		else
		{
			Suggesting = true;
			List<string> commands = new List<string>();
			foreach (var command in ConsoleCommands)
			{
				commands.Append(command.Key);
			}
			commands.Sort();
			commands.Reverse();

			int PrevIndex = 0;
			foreach (var command in commands)
			{
				if (command.Contains(line_edit.Text))
				{
					int index = command.Find(line_edit.Text);
					if (index <= PrevIndex)
					{
						Suggestions.Insert(0, command);
					}
					else
					{
						Suggestions.Append(command);
					}
					PrevIndex = index;
				}
			}
			AutoComplete();

		}
	}

	void ScrollToBottom()
	{
		ScrollBar scroll = rich_label.GetVScrollBar();
		scroll.Value = scroll.MaxValue - scroll.Page;
	}
	void ResetAutoComplete()
	{
		this.Suggestions.Clear();
		CurrentSuggestion = 0;
		Suggesting = false;
	}

	void PrintLine(string text)
	{
		if (rich_label == null)
		{
			CallDeferred("PrintLine", text);
		}
		else
		{
			rich_label.AddText(text + "\n");
		}
	}

	void AddInputHistory(string text)
	{
		if (console_history.Count == 0 || text != console_history.Last())
		{
			console_history.Append(text);
		}
		console_history_index = console_history.Count;
	}

	public void AddCommand(string CommandName, Callable function, int paramCount)
	{
		ConsoleCommands.Add(CommandName, new ConsoleCommand(function, paramCount));
	}

	public void AddCommand(string CommandName, Action function, int paramCount)
	{
		ConsoleCommands.Add(CommandName, new ConsoleCommand(new Callable((GodotObject)function.Target, function.Method.Name), paramCount));
	}

    public void RemoveCommand(string CommandName)
	{
        ConsoleCommands.Remove(CommandName);
    }

	public void Quit()
	{
		GetTree().Quit();
	}
	void clear()
	{
		rich_label.Clear();
	}

	void DeleteHistory()
	{
		console_history.Clear();
		console_history_index = 0;
		DirAccess.RemoveAbsolute(GetPath()+"/Commands.log");
	}
	void Help()
	{
		rich_label.AddText("\r\n\tBuilt in commands:\r\n\t\t'clear' : Clears the current registry view\r\n\t\t'commands_list': Shows a list of all the currently registered commands\r\n\t\t'delete_hystory' : Deletes the commands history\r\n\t\t'quit' : Quits the game\r\n\tControls:\r\n\t\tUp and Down arrow keys to navigate commands history\r\n\t\tPageUp and PageDown to navigate registry history\r\n\t\tCtr+Tilde to change console size between half screen and full creen\r\n\t\tTilde or Esc to close the console\r\n\t\tTab for basic autocomplete");
	}

	void CommandList()
	{
		foreach (var command in ConsoleCommands.Keys.OrderBy(d => d))
		{
			rich_label.AddText("> " + command + "\n");   
        }
	}
}
