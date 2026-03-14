using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Debug;

[ScriptPath("res://src/Core/Nodes/Debug/NDevConsole.cs")]
public class NDevConsole : Panel
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName PrintUsage = StringName.op_Implicit("PrintUsage");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName HandleReadlineKeybinding = StringName.op_Implicit("HandleReadlineKeybinding");

		public static readonly StringName DeleteWordBackward = StringName.op_Implicit("DeleteWordBackward");

		public static readonly StringName KillToEndOfLine = StringName.op_Implicit("KillToEndOfLine");

		public static readonly StringName Yank = StringName.op_Implicit("Yank");

		public static readonly StringName EnableTabBuffer = StringName.op_Implicit("EnableTabBuffer");

		public static readonly StringName DisableTabBuffer = StringName.op_Implicit("DisableTabBuffer");

		public static readonly StringName SetBackgroundColor = StringName.op_Implicit("SetBackgroundColor");

		public static readonly StringName HideGhostText = StringName.op_Implicit("HideGhostText");

		public static readonly StringName ShowGhostText = StringName.op_Implicit("ShowGhostText");

		public static readonly StringName UpdateGhostText = StringName.op_Implicit("UpdateGhostText");

		public static readonly StringName AutocompleteCommand = StringName.op_Implicit("AutocompleteCommand");

		public static readonly StringName RenderSelectionMenu = StringName.op_Implicit("RenderSelectionMenu");

		public static readonly StringName OnInputTextChanged = StringName.op_Implicit("OnInputTextChanged");

		public static readonly StringName ExitSelectionMode = StringName.op_Implicit("ExitSelectionMode");

		public static readonly StringName NavigateSelection = StringName.op_Implicit("NavigateSelection");

		public static readonly StringName AcceptSelection = StringName.op_Implicit("AcceptSelection");

		public static readonly StringName ProcessCommand = StringName.op_Implicit("ProcessCommand");

		public static readonly StringName ShowConsole = StringName.op_Implicit("ShowConsole");

		public static readonly StringName HideConsole = StringName.op_Implicit("HideConsole");

		public static readonly StringName MakeHalfScreen = StringName.op_Implicit("MakeHalfScreen");

		public static readonly StringName MakeFullScreen = StringName.op_Implicit("MakeFullScreen");

		public static readonly StringName OnToggleMaximizeButtonPressed = StringName.op_Implicit("OnToggleMaximizeButtonPressed");

		public static readonly StringName MoveInputCursorToEndOfLine = StringName.op_Implicit("MoveInputCursorToEndOfLine");

		public static readonly StringName UpdatePromptStyle = StringName.op_Implicit("UpdatePromptStyle");

		public static readonly StringName AddChildToTree = StringName.op_Implicit("AddChildToTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _outputBuffer = StringName.op_Implicit("_outputBuffer");

		public static readonly StringName _tabBuffer = StringName.op_Implicit("_tabBuffer");

		public static readonly StringName _inputContainer = StringName.op_Implicit("_inputContainer");

		public static readonly StringName _inputBuffer = StringName.op_Implicit("_inputBuffer");

		public static readonly StringName _promptLabel = StringName.op_Implicit("_promptLabel");

		public static readonly StringName _ghostTextLabel = StringName.op_Implicit("_ghostTextLabel");

		public static readonly StringName _isFullscreen = StringName.op_Implicit("_isFullscreen");

		public static readonly StringName _yankBuffer = StringName.op_Implicit("_yankBuffer");
	}

	public class SignalName : SignalName
	{
	}

	private static NDevConsole? _instance;

	private RichTextLabel _outputBuffer;

	private RichTextLabel _tabBuffer;

	private Control _inputContainer;

	private LineEdit _inputBuffer;

	private Label _promptLabel;

	private Label _ghostTextLabel;

	private bool _isFullscreen;

	private MegaCrit.Sts2.Core.DevConsole.DevConsole _devConsole;

	private const float _inputBufferSizeY = 40f;

	private readonly TabCompletionState _tabCompletion = new TabCompletionState();

	private string _yankBuffer = string.Empty;

	public static NDevConsole Instance => _instance ?? throw new InvalidOperationException("Dev console used before being created.");

	public override void _Ready()
	{
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		if (TestMode.IsOn)
		{
			((Node)(object)this).QueueFreeSafely();
			return;
		}
		if (_instance != null)
		{
			((Node)(object)this).QueueFreeSafely();
			return;
		}
		_instance = this;
		bool shouldAllowDebugCommands = OS.HasFeature("editor") || TestMode.IsOn || ModManager.LoadedMods.Count > 0 || SaveManager.Instance.SettingsSave.FullConsole;
		HideConsole();
		_devConsole = new MegaCrit.Sts2.Core.DevConsole.DevConsole(shouldAllowDebugCommands);
		_outputBuffer = ((Node)this).GetNode<RichTextLabel>(NodePath.op_Implicit("OutputContainer/OutputBuffer"));
		_tabBuffer = ((Node)this).GetNode<RichTextLabel>(NodePath.op_Implicit("OutputContainer/TabBuffer"));
		_inputContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("InputContainer"));
		_inputBuffer = ((Node)this).GetNode<LineEdit>(NodePath.op_Implicit("InputContainer/InputBufferContainer/InputBuffer"));
		_promptLabel = ((Node)this).GetNode<Label>(NodePath.op_Implicit("InputContainer/PromptLabel"));
		_ghostTextLabel = ((Node)this).GetNode<Label>(NodePath.op_Implicit("InputContainer/InputBufferContainer/GhostText"));
		MakeHalfScreen();
		DisableTabBuffer();
		HideGhostText();
		_inputBuffer.CaretBlink = true;
		UpdatePromptStyle();
		_inputBuffer.TextChanged += new TextChangedEventHandler(OnInputTextChanged);
		PrintUsage();
	}

	public override void _ExitTree()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		if (TestMode.IsOff)
		{
			_inputBuffer.TextChanged -= new TextChangedEventHandler(OnInputTextChanged);
		}
		((Node)this)._ExitTree();
	}

	private void PrintUsage()
	{
		RichTextLabel outputBuffer = _outputBuffer;
		outputBuffer.Text += "[color=#888888]Use 'F11' to toggle console fullscreen. Press 'up arrow' to use the last command. You can autocomplete commands with 'tab'.[/color]";
		RichTextLabel outputBuffer2 = _outputBuffer;
		outputBuffer2.Text += "\n\n";
	}

	public override void _Input(InputEvent inputEvent)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I8
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I8
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I8
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I8
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Invalid comparison between Unknown and I8
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Invalid comparison between Unknown and I8
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Invalid comparison between Unknown and I8
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Invalid comparison between Unknown and I8
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Invalid comparison between Unknown and I8
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Invalid comparison between Unknown and I8
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Invalid comparison between Unknown and I8
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Invalid comparison between Unknown and I8
		InputEventKey val = (InputEventKey)(object)((inputEvent is InputEventKey) ? inputEvent : null);
		if (val == null || !val.Pressed)
		{
			return;
		}
		Key keycode = val.Keycode;
		if ((long)keycode <= 42L)
		{
			if ((long)keycode == 39 || (long)keycode == 42)
			{
				goto IL_003b;
			}
		}
		else if ((long)keycode == 94 || (long)keycode == 96)
		{
			goto IL_003b;
		}
		bool flag = false;
		goto IL_0041;
		IL_003b:
		flag = true;
		goto IL_0041;
		IL_0041:
		if (flag || (((InputEventWithModifiers)val).IsShiftPressed() && (long)val.Keycode == 56))
		{
			if (((CanvasItem)this).Visible)
			{
				HideConsole();
			}
			else
			{
				Control val2 = ((Node)this).GetViewport().GuiGetFocusOwner();
				if ((!(val2 is TextEdit) && !(val2 is LineEdit)) || 1 == 0)
				{
					ShowConsole();
				}
			}
		}
		if (!((CanvasItem)this).Visible)
		{
			return;
		}
		if ((long)val.Keycode == 4194305)
		{
			if (_tabCompletion.InSelectionMode)
			{
				ExitSelectionMode();
			}
			else
			{
				HideConsole();
			}
		}
		else if ((long)val.Keycode == 4194342)
		{
			OnToggleMaximizeButtonPressed();
		}
		else if ((long)val.Keycode == 4194306)
		{
			if (_tabCompletion.InSelectionMode)
			{
				NavigateSelection(1);
			}
			else
			{
				AutocompleteCommand();
			}
		}
		else if ((long)val.Keycode == 4194320)
		{
			if (_tabCompletion.InSelectionMode)
			{
				NavigateSelection(-1);
			}
			else if (_devConsole.historyIndex < _devConsole.history.Count)
			{
				_tabCompletion.ProgrammaticTextChange = true;
				string text = _devConsole.history[_devConsole.historyIndex];
				_inputBuffer.Text = text;
				if (_devConsole.historyIndex < _devConsole.history.Count - 1)
				{
					_devConsole.historyIndex++;
					while (_devConsole.historyIndex < _devConsole.history.Count - 1 && _devConsole.history[_devConsole.historyIndex] == text)
					{
						_devConsole.historyIndex++;
					}
				}
				MoveInputCursorToEndOfLine();
			}
			((Node)this).GetViewport().SetInputAsHandled();
		}
		else if ((long)val.Keycode == 4194322)
		{
			if (_tabCompletion.InSelectionMode)
			{
				NavigateSelection(1);
			}
			else if (_devConsole.historyIndex < _devConsole.history.Count)
			{
				_tabCompletion.ProgrammaticTextChange = true;
				string text2 = _devConsole.history[_devConsole.historyIndex];
				_inputBuffer.Text = text2;
				if (_devConsole.historyIndex > 0)
				{
					_devConsole.historyIndex--;
					while (_devConsole.historyIndex > 0 && _devConsole.history[_devConsole.historyIndex] == text2)
					{
						_devConsole.historyIndex--;
					}
				}
				MoveInputCursorToEndOfLine();
			}
			((Node)this).GetViewport().SetInputAsHandled();
		}
		else if ((long)val.Keycode == 4194309)
		{
			if (_tabCompletion.InSelectionMode)
			{
				AcceptSelection();
			}
			else
			{
				ProcessCommand();
			}
		}
		else if (((InputEventWithModifiers)val).IsCtrlPressed())
		{
			HandleReadlineKeybinding(val);
		}
	}

	private void HandleReadlineKeybinding(InputEventKey keyEvent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I8
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Invalid comparison between Unknown and I8
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I8
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Invalid comparison between Unknown and I8
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I8
		Key keycode = keyEvent.Keycode;
		if ((long)keycode <= 75L)
		{
			_003F val = keycode - 65;
			if ((long)val <= 4L)
			{
				switch ((uint)val)
				{
				case 0u:
					((Node)this).GetViewport().SetInputAsHandled();
					_inputBuffer.CaretColumn = 0;
					((GodotObject)_inputBuffer).CallDeferred(MethodName.Deselect, Array.Empty<Variant>());
					return;
				case 4u:
					((Node)this).GetViewport().SetInputAsHandled();
					MoveInputCursorToEndOfLine();
					return;
				case 2u:
					((Node)this).GetViewport().SetInputAsHandled();
					_inputBuffer.Text = string.Empty;
					ExitSelectionMode();
					return;
				case 3u:
					((Node)this).GetViewport().SetInputAsHandled();
					HideConsole();
					return;
				case 1u:
					return;
				}
			}
			if ((long)keycode == 75)
			{
				((Node)this).GetViewport().SetInputAsHandled();
				KillToEndOfLine();
			}
		}
		else if ((long)keycode != 76)
		{
			_003F val2 = keycode - 85;
			if ((long)val2 <= 4L)
			{
				switch ((uint)val2)
				{
				case 2u:
					((Node)this).GetViewport().SetInputAsHandled();
					DeleteWordBackward();
					break;
				case 0u:
					((Node)this).GetViewport().SetInputAsHandled();
					_yankBuffer = _inputBuffer.Text;
					_inputBuffer.Text = string.Empty;
					ExitSelectionMode();
					break;
				case 4u:
					((Node)this).GetViewport().SetInputAsHandled();
					Yank();
					break;
				case 1u:
				case 3u:
					break;
				}
			}
		}
		else
		{
			((Node)this).GetViewport().SetInputAsHandled();
			_outputBuffer.Text = string.Empty;
		}
	}

	private void DeleteWordBackward()
	{
		string text = _inputBuffer.Text;
		int caretColumn = _inputBuffer.CaretColumn;
		if (caretColumn != 0 && !string.IsNullOrEmpty(text))
		{
			int num = caretColumn - 1;
			while (num >= 0 && char.IsWhiteSpace(text[num]))
			{
				num--;
			}
			while (num >= 0 && !char.IsWhiteSpace(text[num]))
			{
				num--;
			}
			num++;
			_yankBuffer = text.Substring(num, caretColumn - num);
			string text2 = text.Substring(0, num) + text.Substring(caretColumn);
			_tabCompletion.ProgrammaticTextChange = true;
			_inputBuffer.Text = text2;
			_inputBuffer.CaretColumn = num;
		}
	}

	private void KillToEndOfLine()
	{
		string text = _inputBuffer.Text;
		int caretColumn = _inputBuffer.CaretColumn;
		if (caretColumn < text.Length)
		{
			_yankBuffer = text.Substring(caretColumn);
			string text2 = text.Substring(0, caretColumn);
			_tabCompletion.ProgrammaticTextChange = true;
			_inputBuffer.Text = text2;
			_inputBuffer.CaretColumn = caretColumn;
		}
	}

	private void Yank()
	{
		if (!string.IsNullOrEmpty(_yankBuffer))
		{
			string text = _inputBuffer.Text;
			int caretColumn = _inputBuffer.CaretColumn;
			string text2 = text.Insert(caretColumn, _yankBuffer);
			_tabCompletion.ProgrammaticTextChange = true;
			_inputBuffer.Text = text2;
			_inputBuffer.CaretColumn = caretColumn + _yankBuffer.Length;
		}
	}

	private void EnableTabBuffer()
	{
		((CanvasItem)_outputBuffer).Visible = false;
		((CanvasItem)_tabBuffer).Visible = true;
	}

	private void DisableTabBuffer()
	{
		((CanvasItem)_outputBuffer).Visible = true;
		((CanvasItem)_tabBuffer).Visible = false;
	}

	public void SetBackgroundColor(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).Modulate = color;
	}

	private void HideGhostText()
	{
		((CanvasItem)_ghostTextLabel).Visible = false;
		_ghostTextLabel.Text = string.Empty;
	}

	private void ShowGhostText(string ghostText)
	{
		_ghostTextLabel.Text = ghostText;
		((CanvasItem)_ghostTextLabel).Visible = true;
	}

	private void UpdateGhostText()
	{
		if (_tabCompletion.InSelectionMode)
		{
			HideGhostText();
			return;
		}
		string text = _inputBuffer.Text;
		if (string.IsNullOrWhiteSpace(text))
		{
			HideGhostText();
			return;
		}
		CompletionResult completionResults = _devConsole.GetCompletionResults(text);
		if (completionResults.Candidates.Count == 1 && !string.IsNullOrEmpty(completionResults.CommonPrefix))
		{
			string commonPrefix = completionResults.CommonPrefix;
			if (!commonPrefix.StartsWith(text, StringComparison.OrdinalIgnoreCase))
			{
				GD.PushError($"BUG: CommonPrefix '{commonPrefix}' doesn't start with input '{text}'");
				HideGhostText();
			}
			else
			{
				string text2 = commonPrefix.Substring(text.Length);
				string text3 = new string(' ', text.Length);
				ShowGhostText(text3 + text2);
			}
		}
		else
		{
			HideGhostText();
		}
	}

	private void AutocompleteCommand()
	{
		string text = _inputBuffer.Text;
		CompletionResult completionResults = _devConsole.GetCompletionResults(text);
		_tabCompletion.LastCompletionResult = completionResults;
		if (string.IsNullOrWhiteSpace(text) && completionResults.Candidates.Count == 0)
		{
			completionResults = _devConsole.GetCompletionResults("");
			_tabCompletion.LastCompletionResult = completionResults;
		}
		if (completionResults.Candidates.Count == 0)
		{
			ExitSelectionMode();
		}
		else if (completionResults.Candidates.Count == 1)
		{
			_tabCompletion.ProgrammaticTextChange = true;
			_inputBuffer.Text = completionResults.CommonPrefix;
			MoveInputCursorToEndOfLine();
			ExitSelectionMode();
		}
		else
		{
			_tabCompletion.CompletionCandidates.Clear();
			_tabCompletion.CompletionCandidates.AddRange(completionResults.Candidates);
			_tabCompletion.InSelectionMode = true;
			_tabCompletion.SelectionIndex = 0;
			HideGhostText();
			RenderSelectionMenu();
			MoveInputCursorToEndOfLine();
		}
	}

	private void RenderSelectionMenu()
	{
		if (!_tabCompletion.InSelectionMode || _tabCompletion.CompletionCandidates.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		list.Add(_tabCompletion.LastCompletionResult?.Type switch
		{
			CompletionType.Command => "Select command:", 
			CompletionType.Subcommand => "Select " + _tabCompletion.LastCompletionResult.ArgumentContext + " action:", 
			CompletionType.Argument => "Select " + _tabCompletion.LastCompletionResult.ArgumentContext + " argument:", 
			_ => "Select option:", 
		});
		list.Add("[color=gray]Tab/↑↓: navigate, Enter: accept, Esc: cancel, Type to filter[/color]");
		list.Add("");
		int count = _tabCompletion.CompletionCandidates.Count;
		if (count <= 12)
		{
			for (int i = 0; i < count; i++)
			{
				AddCandidateToDisplay(list, i);
			}
		}
		else
		{
			int num = Math.Max(0, _tabCompletion.SelectionIndex - 6);
			int num2 = Math.Min(count, num + 12);
			if (num2 - num < 12)
			{
				num = Math.Max(0, num2 - 12);
			}
			if (num > 0)
			{
				list.Add($"[color=gray]↑ {num} more above ↑[/color]");
			}
			for (int j = num; j < num2; j++)
			{
				AddCandidateToDisplay(list, j);
			}
			if (num2 < count)
			{
				int value = count - num2;
				list.Add($"[color=gray]↓ {value} more below ↓[/color]");
			}
		}
		list.Add("");
		list.Add($"[color=gray]({_tabCompletion.CompletionCandidates.Count} matches)[/color]");
		_tabBuffer.Text = string.Join("\n", list);
		EnableTabBuffer();
	}

	private void AddCandidateToDisplay(List<string> displayLines, int index)
	{
		if (index >= 0 && index < _tabCompletion.CompletionCandidates.Count)
		{
			string text = _tabCompletion.CompletionCandidates[index];
			string item = ((index == _tabCompletion.SelectionIndex) ? ("[color=yellow]➜ " + text + "[/color]") : ("  " + text));
			displayLines.Add(item);
		}
	}

	private void OnInputTextChanged(string newText)
	{
		bool programmaticTextChange = _tabCompletion.ProgrammaticTextChange;
		_tabCompletion.ProgrammaticTextChange = false;
		if (_tabCompletion.InSelectionMode && !programmaticTextChange)
		{
			CompletionResult completionResults = _devConsole.GetCompletionResults(newText);
			_tabCompletion.LastCompletionResult = completionResults;
			if (completionResults.Candidates.Count == 0)
			{
				ExitSelectionMode();
			}
			else if (completionResults.Candidates.Count == 1)
			{
				_tabCompletion.ProgrammaticTextChange = true;
				_inputBuffer.Text = completionResults.CommonPrefix;
				MoveInputCursorToEndOfLine();
				ExitSelectionMode();
			}
			else
			{
				_tabCompletion.CompletionCandidates.Clear();
				_tabCompletion.CompletionCandidates.AddRange(completionResults.Candidates);
				_tabCompletion.SelectionIndex = 0;
				RenderSelectionMenu();
			}
		}
		if (!programmaticTextChange)
		{
			UpdateGhostText();
		}
	}

	private void ExitSelectionMode()
	{
		_tabCompletion.Reset();
		_tabBuffer.Text = string.Empty;
		DisableTabBuffer();
		UpdateGhostText();
	}

	private void NavigateSelection(int direction)
	{
		if (_tabCompletion.InSelectionMode && _tabCompletion.CompletionCandidates.Count != 0)
		{
			_tabCompletion.SelectionIndex = (_tabCompletion.SelectionIndex + direction + _tabCompletion.CompletionCandidates.Count) % _tabCompletion.CompletionCandidates.Count;
			RenderSelectionMenu();
		}
	}

	private void AcceptSelection()
	{
		if (!_tabCompletion.InSelectionMode || _tabCompletion.SelectionIndex < 0 || _tabCompletion.SelectionIndex >= _tabCompletion.CompletionCandidates.Count)
		{
			return;
		}
		string text = _tabCompletion.CompletionCandidates[_tabCompletion.SelectionIndex];
		_tabCompletion.ProgrammaticTextChange = true;
		string text2 = _inputBuffer.Text;
		CompletionResult lastCompletionResult = _tabCompletion.LastCompletionResult;
		if (lastCompletionResult != null)
		{
			switch (lastCompletionResult.Type)
			{
			case CompletionType.Command:
				_inputBuffer.Text = text + " ";
				break;
			case CompletionType.Subcommand:
			case CompletionType.Argument:
			{
				string text3 = (text.Contains(' ') ? ("\"" + text + "\"") : text);
				if (text2.EndsWith(' '))
				{
					_inputBuffer.Text = lastCompletionResult.CommandPrefix + text3 + " ";
				}
				else
				{
					_inputBuffer.Text = lastCompletionResult.CommandPrefix + text3 + " ";
				}
				break;
			}
			}
		}
		else
		{
			string[] array = text2.Trim().Split(' ');
			string[] array2 = ((array.Length > 1) ? array.Take(array.Length - 1).ToArray() : Array.Empty<string>());
			string text4 = ((array2.Length != 0) ? (string.Join(" ", array2) + " ") : string.Empty);
			_inputBuffer.Text = text4 + text + " ";
		}
		MoveInputCursorToEndOfLine();
		ExitSelectionMode();
	}

	private void ProcessCommand()
	{
		if (string.IsNullOrWhiteSpace(_inputBuffer.Text))
		{
			return;
		}
		RichTextLabel outputBuffer = _outputBuffer;
		outputBuffer.Text = outputBuffer.Text + "[color=#00ff00]➜[/color] " + _inputBuffer.Text + "\n";
		if (_inputBuffer.Text.Trim().Equals("clear"))
		{
			_outputBuffer.Text = string.Empty;
			_inputBuffer.Text = string.Empty;
			return;
		}
		if (_inputBuffer.Text.Trim().Equals("exit"))
		{
			HideConsole();
			return;
		}
		Exception ex = null;
		CmdResult cmdResult;
		try
		{
			cmdResult = _devConsole.ProcessCommand(_inputBuffer.Text);
		}
		catch (Exception ex2)
		{
			cmdResult = new CmdResult(success: false, $"An exception occurred: {ex2}");
			ex = ex2;
		}
		if (cmdResult.success)
		{
			RichTextLabel outputBuffer2 = _outputBuffer;
			outputBuffer2.Text = outputBuffer2.Text + cmdResult.msg + "\n";
		}
		else
		{
			RichTextLabel outputBuffer3 = _outputBuffer;
			outputBuffer3.Text = outputBuffer3.Text + "[color=#ff5555]⚠ " + cmdResult.msg + "[/color]\n";
		}
		_inputBuffer.Text = string.Empty;
		_tabBuffer.Text = string.Empty;
		DisableTabBuffer();
		HideGhostText();
		if (ex != null)
		{
			ExceptionDispatchInfo.Capture(ex).Throw();
		}
	}

	public Task ProcessNetCommand(Player? player, string netCommand)
	{
		Exception ex = null;
		CmdResult cmdResult;
		try
		{
			cmdResult = _devConsole.ProcessNetCommand(player, netCommand);
		}
		catch (Exception ex2)
		{
			cmdResult = new CmdResult(success: false, $"An exception occurred: {ex2}");
			ex = ex2;
		}
		if (cmdResult.success)
		{
			RichTextLabel outputBuffer = _outputBuffer;
			outputBuffer.Text = outputBuffer.Text + cmdResult.msg + "\n";
		}
		else
		{
			RichTextLabel outputBuffer2 = _outputBuffer;
			outputBuffer2.Text = outputBuffer2.Text + "[color=#ff5555]⚠ " + cmdResult.msg + "[/color]\n";
		}
		if (ex != null)
		{
			ExceptionDispatchInfo.Capture(ex).Throw();
		}
		return cmdResult.task ?? Task.CompletedTask;
	}

	public void ShowConsole()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).Visible = true;
		((GodotObject)_inputBuffer).CallDeferred(MethodName.GrabFocus, Array.Empty<Variant>());
	}

	public void HideConsole()
	{
		((CanvasItem)this).Visible = false;
		Viewport viewport = ((Node)this).GetViewport();
		if (viewport != null)
		{
			viewport.GuiReleaseFocus();
		}
	}

	public void MakeHalfScreen()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		Rect2 viewportRect = ((CanvasItem)this).GetViewportRect();
		Vector2 size = ((Rect2)(ref viewportRect)).Size;
		float num = size.Y * 0.5f;
		_inputContainer.SetSize(new Vector2(size.X, 40f), false);
		_inputContainer.Position = new Vector2(0f, num - 40f);
		((Control)_outputBuffer).SetSize(new Vector2(size.X, num), false);
		((Control)_tabBuffer).SetSize(new Vector2(size.X, num), false);
		((Control)this).SetSize(new Vector2(size.X, size.Y / 2f), false);
		_isFullscreen = false;
	}

	public void MakeFullScreen()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		Rect2 viewportRect = ((CanvasItem)this).GetViewportRect();
		Vector2 size = ((Rect2)(ref viewportRect)).Size;
		float y = size.Y;
		_inputContainer.SetSize(new Vector2(size.X, 40f), false);
		_inputContainer.Position = new Vector2(0f, y - 40f);
		((Control)_outputBuffer).SetSize(new Vector2(((Control)_outputBuffer).Size.X, y - 40f), false);
		((Control)_tabBuffer).SetSize(new Vector2(((Control)_outputBuffer).Size.X, y - 40f), false);
		((Control)this).SetSize(size, false);
		_isFullscreen = true;
	}

	private void OnToggleMaximizeButtonPressed()
	{
		if (!_isFullscreen)
		{
			MakeFullScreen();
		}
		else
		{
			MakeHalfScreen();
		}
	}

	public void MoveInputCursorToEndOfLine()
	{
		_inputBuffer.CaretColumn = _inputBuffer.Text.Length;
	}

	private void UpdatePromptStyle()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		_promptLabel.Text = "➜";
		((Control)_promptLabel).AddThemeColorOverride(ThemeConstants.Label.fontColor, new Color(0f, 0.831f, 1f, 1f));
		((Control)_promptLabel).AddThemeFontSizeOverride(ThemeConstants.Label.fontSize, 18);
	}

	public void AddChildToTree(Node node)
	{
		((Node)(object)this).AddChildSafely(node);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Expected O, but got Unknown
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(29);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PrintUsage, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HandleReadlineKeybinding, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("keyEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEventKey"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DeleteWordBackward, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.KillToEndOfLine, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Yank, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EnableTabBuffer, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisableTabBuffer, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetBackgroundColor, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)20, StringName.op_Implicit("color"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideGhostText, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowGhostText, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("ghostText"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateGhostText, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AutocompleteCommand, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RenderSelectionMenu, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnInputTextChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("newText"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ExitSelectionMode, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.NavigateSelection, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("direction"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AcceptSelection, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessCommand, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowConsole, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideConsole, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.MakeHalfScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.MakeFullScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnToggleMaximizeButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.MoveInputCursorToEndOfLine, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdatePromptStyle, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddChildToTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("node"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PrintUsage && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PrintUsage();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HandleReadlineKeybinding && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			HandleReadlineKeybinding(VariantUtils.ConvertTo<InputEventKey>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DeleteWordBackward && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DeleteWordBackward();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.KillToEndOfLine && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			KillToEndOfLine();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Yank && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Yank();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EnableTabBuffer && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EnableTabBuffer();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisableTabBuffer && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisableTabBuffer();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetBackgroundColor && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetBackgroundColor(VariantUtils.ConvertTo<Color>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideGhostText && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideGhostText();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowGhostText && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ShowGhostText(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateGhostText && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateGhostText();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AutocompleteCommand && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AutocompleteCommand();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RenderSelectionMenu && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RenderSelectionMenu();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnInputTextChanged && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnInputTextChanged(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ExitSelectionMode && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ExitSelectionMode();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.NavigateSelection && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NavigateSelection(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AcceptSelection && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AcceptSelection();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessCommand && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ProcessCommand();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowConsole && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowConsole();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideConsole && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideConsole();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.MakeHalfScreen && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			MakeHalfScreen();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.MakeFullScreen && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			MakeFullScreen();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnToggleMaximizeButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnToggleMaximizeButtonPressed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.MoveInputCursorToEndOfLine && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			MoveInputCursorToEndOfLine();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdatePromptStyle && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdatePromptStyle();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AddChildToTree && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AddChildToTree(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Panel)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.PrintUsage)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.HandleReadlineKeybinding)
		{
			return true;
		}
		if ((ref method) == MethodName.DeleteWordBackward)
		{
			return true;
		}
		if ((ref method) == MethodName.KillToEndOfLine)
		{
			return true;
		}
		if ((ref method) == MethodName.Yank)
		{
			return true;
		}
		if ((ref method) == MethodName.EnableTabBuffer)
		{
			return true;
		}
		if ((ref method) == MethodName.DisableTabBuffer)
		{
			return true;
		}
		if ((ref method) == MethodName.SetBackgroundColor)
		{
			return true;
		}
		if ((ref method) == MethodName.HideGhostText)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowGhostText)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateGhostText)
		{
			return true;
		}
		if ((ref method) == MethodName.AutocompleteCommand)
		{
			return true;
		}
		if ((ref method) == MethodName.RenderSelectionMenu)
		{
			return true;
		}
		if ((ref method) == MethodName.OnInputTextChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.ExitSelectionMode)
		{
			return true;
		}
		if ((ref method) == MethodName.NavigateSelection)
		{
			return true;
		}
		if ((ref method) == MethodName.AcceptSelection)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessCommand)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowConsole)
		{
			return true;
		}
		if ((ref method) == MethodName.HideConsole)
		{
			return true;
		}
		if ((ref method) == MethodName.MakeHalfScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.MakeFullScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.OnToggleMaximizeButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.MoveInputCursorToEndOfLine)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdatePromptStyle)
		{
			return true;
		}
		if ((ref method) == MethodName.AddChildToTree)
		{
			return true;
		}
		return ((Panel)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._outputBuffer)
		{
			_outputBuffer = VariantUtils.ConvertTo<RichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tabBuffer)
		{
			_tabBuffer = VariantUtils.ConvertTo<RichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._inputContainer)
		{
			_inputContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._inputBuffer)
		{
			_inputBuffer = VariantUtils.ConvertTo<LineEdit>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._promptLabel)
		{
			_promptLabel = VariantUtils.ConvertTo<Label>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ghostTextLabel)
		{
			_ghostTextLabel = VariantUtils.ConvertTo<Label>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isFullscreen)
		{
			_isFullscreen = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._yankBuffer)
		{
			_yankBuffer = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._outputBuffer)
		{
			value = VariantUtils.CreateFrom<RichTextLabel>(ref _outputBuffer);
			return true;
		}
		if ((ref name) == PropertyName._tabBuffer)
		{
			value = VariantUtils.CreateFrom<RichTextLabel>(ref _tabBuffer);
			return true;
		}
		if ((ref name) == PropertyName._inputContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _inputContainer);
			return true;
		}
		if ((ref name) == PropertyName._inputBuffer)
		{
			value = VariantUtils.CreateFrom<LineEdit>(ref _inputBuffer);
			return true;
		}
		if ((ref name) == PropertyName._promptLabel)
		{
			value = VariantUtils.CreateFrom<Label>(ref _promptLabel);
			return true;
		}
		if ((ref name) == PropertyName._ghostTextLabel)
		{
			value = VariantUtils.CreateFrom<Label>(ref _ghostTextLabel);
			return true;
		}
		if ((ref name) == PropertyName._isFullscreen)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isFullscreen);
			return true;
		}
		if ((ref name) == PropertyName._yankBuffer)
		{
			value = VariantUtils.CreateFrom<string>(ref _yankBuffer);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._outputBuffer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tabBuffer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._inputContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._inputBuffer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._promptLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ghostTextLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isFullscreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._yankBuffer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._outputBuffer, Variant.From<RichTextLabel>(ref _outputBuffer));
		info.AddProperty(PropertyName._tabBuffer, Variant.From<RichTextLabel>(ref _tabBuffer));
		info.AddProperty(PropertyName._inputContainer, Variant.From<Control>(ref _inputContainer));
		info.AddProperty(PropertyName._inputBuffer, Variant.From<LineEdit>(ref _inputBuffer));
		info.AddProperty(PropertyName._promptLabel, Variant.From<Label>(ref _promptLabel));
		info.AddProperty(PropertyName._ghostTextLabel, Variant.From<Label>(ref _ghostTextLabel));
		info.AddProperty(PropertyName._isFullscreen, Variant.From<bool>(ref _isFullscreen));
		info.AddProperty(PropertyName._yankBuffer, Variant.From<string>(ref _yankBuffer));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._outputBuffer, ref val))
		{
			_outputBuffer = ((Variant)(ref val)).As<RichTextLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._tabBuffer, ref val2))
		{
			_tabBuffer = ((Variant)(ref val2)).As<RichTextLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._inputContainer, ref val3))
		{
			_inputContainer = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._inputBuffer, ref val4))
		{
			_inputBuffer = ((Variant)(ref val4)).As<LineEdit>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._promptLabel, ref val5))
		{
			_promptLabel = ((Variant)(ref val5)).As<Label>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._ghostTextLabel, ref val6))
		{
			_ghostTextLabel = ((Variant)(ref val6)).As<Label>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._isFullscreen, ref val7))
		{
			_isFullscreen = ((Variant)(ref val7)).As<bool>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._yankBuffer, ref val8))
		{
			_yankBuffer = ((Variant)(ref val8)).As<string>();
		}
	}
}
