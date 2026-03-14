using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Credits;

[ScriptPath("res://src/Core/Nodes/Screens/Credits/NCreditsScreen.cs")]
public class NCreditsScreen : Control, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName InitMegaCrit = StringName.op_Implicit("InitMegaCrit");

		public static readonly StringName InitComposer = StringName.op_Implicit("InitComposer");

		public static readonly StringName InitAdditionalProgramming = StringName.op_Implicit("InitAdditionalProgramming");

		public static readonly StringName InitAdditionalVfx = StringName.op_Implicit("InitAdditionalVfx");

		public static readonly StringName InitMarketingSupport = StringName.op_Implicit("InitMarketingSupport");

		public static readonly StringName InitConsultants = StringName.op_Implicit("InitConsultants");

		public static readonly StringName InitVoices = StringName.op_Implicit("InitVoices");

		public static readonly StringName InitLocalization = StringName.op_Implicit("InitLocalization");

		public static readonly StringName InitTwitchExtension = StringName.op_Implicit("InitTwitchExtension");

		public static readonly StringName InitModdingSupport = StringName.op_Implicit("InitModdingSupport");

		public static readonly StringName InitPlaytesters = StringName.op_Implicit("InitPlaytesters");

		public static readonly StringName InitTrailer = StringName.op_Implicit("InitTrailer");

		public static readonly StringName InitFmod = StringName.op_Implicit("InitFmod");

		public static readonly StringName InitSpine = StringName.op_Implicit("InitSpine");

		public static readonly StringName InitGodot = StringName.op_Implicit("InitGodot");

		public static readonly StringName InitExitMessage = StringName.op_Implicit("InitExitMessage");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName CloseScreenDebug = StringName.op_Implicit("CloseScreenDebug");

		public static readonly StringName _GuiInput = StringName.op_Implicit("_GuiInput");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName ProcessScrollEvent = StringName.op_Implicit("ProcessScrollEvent");

		public static readonly StringName ShuffleOneColumn = StringName.op_Implicit("ShuffleOneColumn");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _canClose = StringName.op_Implicit("_canClose");

		public static readonly StringName _exitingScreen = StringName.op_Implicit("_exitingScreen");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _screenContents = StringName.op_Implicit("_screenContents");

		public static readonly StringName _backButton = StringName.op_Implicit("_backButton");

		public static readonly StringName _targetPosition = StringName.op_Implicit("_targetPosition");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/credits_screen");

	private bool _canClose;

	private bool _exitingScreen;

	private Tween? _tween;

	private Control _screenContents;

	private NBackButton _backButton;

	private const string _table = "credits";

	private float _targetPosition;

	private const float _scrollSpeed = 50f;

	private const float _trackpadScrollSpeed = 20f;

	private const float _autoScrollSpeed = 80f;

	private const float _lerpSmoothness = 20f;

	public Control DefaultFocusedControl => (Control)(object)this;

	public static NCreditsScreen? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NCreditsScreen>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		NHotkeyManager.Instance.PushHotkeyReleasedBinding(StringName.op_Implicit(MegaInput.back), CloseScreenDebug);
		_screenContents = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ScreenContents"));
		_backButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("BackButton"));
		((CanvasItem)_screenContents).Modulate = StsColors.transparentWhite;
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_screenContents, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 2.0);
		_targetPosition = _screenContents.Position.Y;
		TaskHelper.RunSafely(EnableScreenExit());
		InitMegaCrit();
		InitComposer();
		InitAdditionalProgramming();
		InitAdditionalVfx();
		InitMarketingSupport();
		InitConsultants();
		InitVoices();
		InitLocalization();
		InitTwitchExtension();
		InitModdingSupport();
		InitPlaytesters();
		InitTrailer();
		InitFmod();
		InitSpine();
		InitGodot();
		InitExitMessage();
	}

	private void InitMegaCrit()
	{
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%MegaCritHeader"))).Text = new LocString("credits", "MEGA_CRIT.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%CreatedByNames")).Text = new LocString("credits", "MEGA_CRIT.names").GetRawText();
		var (text, text2) = SplitTwoColumn(new LocString("credits", "MEGA_CRIT_TEAM.names").GetRawText());
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%MegaCritTeamRoles")).Text = text;
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%MegaCritTeamNames")).Text = text2;
	}

	private void InitComposer()
	{
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ComposerHeader"))).Text = new LocString("credits", "COMPOSER.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%ComposerNames")).Text = new LocString("credits", "COMPOSER.names").GetRawText();
	}

	private void InitAdditionalProgramming()
	{
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%AdditionalProgrammingHeader"))).Text = new LocString("credits", "ADDITIONAL_PROGRAMMING.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%AdditionalProgrammingNames")).Text = new LocString("credits", "ADDITIONAL_PROGRAMMING.names").GetRawText();
	}

	private void InitAdditionalVfx()
	{
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%AdditionalVfxHeader"))).Text = new LocString("credits", "ADDITIONAL_VFX.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%AdditionalVfxNames")).Text = new LocString("credits", "ADDITIONAL_VFX.names").GetRawText();
	}

	private void InitMarketingSupport()
	{
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%MarketingSupportHeader"))).Text = new LocString("credits", "MARKETING_SUPPORT.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%MarketingSupportNames")).Text = new LocString("credits", "MARKETING_SUPPORT.names").GetRawText();
	}

	private void InitConsultants()
	{
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ConsultantsHeader"))).Text = new LocString("credits", "CONSULTANTS.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%ConsultantsNames")).Text = new LocString("credits", "CONSULTANTS.names").GetRawText();
	}

	private void InitVoices()
	{
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%VoicesHeader"))).Text = new LocString("credits", "VOICES.header").GetRawText();
		var (text, text2) = SplitTwoColumnMultiRole(new LocString("credits", "VOICES.names").GetRawText());
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%VoicesRoles")).Text = text;
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%VoicesNames")).Text = text2;
	}

	private void InitLocalization()
	{
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%LocalizationHeader"))).Text = new LocString("credits", "LOC.header").GetRawText();
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ptbHeader"))).Text = new LocString("credits", "LOC_PTB.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%ptbNames")).Text = new LocString("credits", "LOC_PTB.names").GetRawText();
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%zhsHeader"))).Text = new LocString("credits", "LOC_ZHS.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%zhsNames")).Text = new LocString("credits", "LOC_ZHS.names").GetRawText();
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%fraHeader"))).Text = new LocString("credits", "LOC_FRA.header").GetRawText();
		(string, string) tuple = SplitTwoColumn(new LocString("credits", "LOC_FRA.names").GetRawText());
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%fraRoles")).Text = tuple.Item1;
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%fraNames")).Text = tuple.Item2;
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%deuHeader"))).Text = new LocString("credits", "LOC_DEU.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%deuNames")).Text = new LocString("credits", "LOC_DEU.names").GetRawText();
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%itaHeader"))).Text = new LocString("credits", "LOC_ITA.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%itaTeam")).Text = new LocString("credits", "LOC_ITA.team").GetRawText();
		tuple = SplitTwoColumn(new LocString("credits", "LOC_ITA.names").GetRawText());
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%itaRoles")).Text = tuple.Item1;
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%itaNames")).Text = tuple.Item2;
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%jpnHeader"))).Text = new LocString("credits", "LOC_JPN.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%jpnNames")).Text = new LocString("credits", "LOC_JPN.names").GetRawText();
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%korHeader"))).Text = new LocString("credits", "LOC_KOR.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%korNames")).Text = new LocString("credits", "LOC_KOR.names").GetRawText();
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%polHeader"))).Text = new LocString("credits", "LOC_POL.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%polNames")).Text = new LocString("credits", "LOC_POL.names").GetRawText();
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%rusHeader"))).Text = new LocString("credits", "LOC_RUS.header").GetRawText();
		tuple = SplitTwoColumn(new LocString("credits", "LOC_RUS.names").GetRawText());
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%rusRoles")).Text = tuple.Item1;
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%rusNames")).Text = tuple.Item2;
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%spaHeader"))).Text = new LocString("credits", "LOC_SPA.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%spaNames")).Text = new LocString("credits", "LOC_SPA.names").GetRawText();
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%espHeader"))).Text = new LocString("credits", "LOC_ESP.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%espTeam")).Text = new LocString("credits", "LOC_ESP.team").GetRawText();
		tuple = SplitTwoColumn(new LocString("credits", "LOC_ESP.names").GetRawText());
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%espRoles")).Text = tuple.Item1;
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%espNames")).Text = tuple.Item2;
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%thaHeader"))).Text = new LocString("credits", "LOC_THA.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%thaNames")).Text = new LocString("credits", "LOC_THA.names").GetRawText();
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%turHeader"))).Text = new LocString("credits", "LOC_TUR.header").GetRawText();
		tuple = SplitTwoColumn(new LocString("credits", "LOC_TUR.names").GetRawText());
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%turRoles")).Text = tuple.Item1;
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%turNames")).Text = tuple.Item2;
	}

	private void InitTwitchExtension()
	{
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%TwitchHeader"))).Text = new LocString("credits", "TWITCH.header").GetRawText();
		(string, string) tuple = SplitTwoColumn(new LocString("credits", "TWITCH.names").GetRawText());
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%TwitchRoles")).Text = tuple.Item1;
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%TwitchNames")).Text = tuple.Item2;
	}

	private void InitModdingSupport()
	{
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ModdingSupportHeader"))).Text = new LocString("credits", "MODDING_SUPPORT.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%ModdingSupportNames")).Text = ShuffleOneColumn(new LocString("credits", "MODDING_SUPPORT.names").GetRawText());
	}

	private void InitPlaytesters()
	{
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%PlaytestersHeader"))).Text = new LocString("credits", "PLAYTESTERS.header").GetRawText();
		(string, string, string) tuple = SplitThreeColumnPlaytesters(new LocString("credits", "PLAYTESTERS.names").GetRawText());
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%PlaytesterNames1")).Text = tuple.Item1;
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%PlaytesterNames2")).Text = tuple.Item2;
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%PlaytesterNames3")).Text = tuple.Item3;
	}

	private void InitTrailer()
	{
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%TrailerHeader"))).Text = new LocString("credits", "TRAILER.header").GetRawText();
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%TrailerAnimationTeam")).Text = new LocString("credits", "TRAILER_ANIMATION.team").GetRawText();
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%TrailerAnimationHeader"))).Text = new LocString("credits", "TRAILER_ANIMATION.header").GetRawText();
		(string, string) tuple = SplitTwoColumn(new LocString("credits", "TRAILER_ANIMATION.names").GetRawText());
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%TrailerAnimationRoles")).Text = tuple.Item1;
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%TrailerAnimationNames")).Text = tuple.Item2;
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%TrailerEditorHeader"))).Text = new LocString("credits", "TRAILER_EDITOR.header").GetRawText();
		tuple = SplitTwoColumn(new LocString("credits", "TRAILER_EDITOR.names").GetRawText());
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%TrailerEditorRoles")).Text = tuple.Item1;
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%TrailerEditorNames")).Text = tuple.Item2;
	}

	private void InitFmod()
	{
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%FmodHeader"))).Text = new LocString("credits", "FMOD").GetRawText();
	}

	private void InitSpine()
	{
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%SpineHeader"))).Text = new LocString("credits", "SPINE").GetRawText();
	}

	private void InitGodot()
	{
		((Label)((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%GodotHeader"))).Text = new LocString("credits", "GODOT").GetRawText();
	}

	private void InitExitMessage()
	{
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%ExitMessage")).Text = new LocString("credits", "EXIT_MESSAGE").GetRawText();
	}

	public override void _EnterTree()
	{
		NHotkeyManager.Instance.AddBlockingScreen((Node)(object)this);
		NHotkeyManager.Instance.PushHotkeyReleasedBinding(StringName.op_Implicit(MegaInput.cancel), CloseScreenDebug);
	}

	public override void _ExitTree()
	{
		NHotkeyManager.Instance.RemoveHotkeyReleasedBinding(StringName.op_Implicit(MegaInput.cancel), CloseScreenDebug);
		NHotkeyManager.Instance.RemoveBlockingScreen((Node)(object)this);
	}

	private async Task EnableScreenExit()
	{
		await Task.Delay(2000);
		_canClose = true;
	}

	private void CloseScreenDebug()
	{
		if (_canClose && !_exitingScreen)
		{
			_exitingScreen = true;
			TaskHelper.RunSafely(FadeAndExitScreen());
		}
	}

	private async Task FadeAndExitScreen()
	{
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_screenContents, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 1.0);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		NModalContainer.Instance.Clear();
	}

	public override void _GuiInput(InputEvent inputEvent)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I8
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val != null && (long)val.ButtonIndex == 1 && val.Pressed)
		{
			CloseScreenDebug();
		}
		ProcessScrollEvent(inputEvent);
	}

	public override void _Process(double delta)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)delta;
		_targetPosition -= 80f * num;
		Control screenContents = _screenContents;
		Vector2 position = _screenContents.Position;
		screenContents.Position = ((Vector2)(ref position)).Lerp(new Vector2(_screenContents.Position.X, _targetPosition), num * 20f);
	}

	private void ProcessScrollEvent(InputEvent inputEvent)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I8
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I8
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val != null)
		{
			if ((long)val.ButtonIndex == 4)
			{
				_targetPosition += 50f;
			}
			else if ((long)val.ButtonIndex == 5)
			{
				_targetPosition -= 50f;
			}
		}
		else
		{
			InputEventPanGesture val2 = (InputEventPanGesture)(object)((inputEvent is InputEventPanGesture) ? inputEvent : null);
			if (val2 != null)
			{
				_targetPosition += (0f - val2.Delta.Y) * 20f;
			}
		}
	}

	private static string ShuffleOneColumn(string input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			return string.Empty;
		}
		List<string> list = input.Split(new string[1] { "||" }, StringSplitOptions.None).ToList();
		for (int num = list.Count - 1; num > 0; num--)
		{
			int num2 = Rng.Chaotic.NextInt(num + 1);
			List<string> list2 = list;
			int index = num;
			int index2 = num2;
			string value = list[num2];
			string value2 = list[num];
			list2[index] = value;
			list[index2] = value2;
		}
		return string.Join("\n", list);
	}

	private (string Roles, string Names) SplitTwoColumn(string input)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		string[] array = input.Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = (from p in text.Split(new string[1] { "||" }, StringSplitOptions.RemoveEmptyEntries)
				select p.Trim() into p
				where !string.IsNullOrWhiteSpace(p)
				select p).ToArray();
			if (array3.Length == 2)
			{
				list.Add(array3[0]);
				list2.Add(array3[1]);
			}
		}
		return (Roles: string.Join("\n", list), Names: string.Join("\n", list2));
	}

	private (string Column1, string Column2, string Column3) SplitThreeColumnPlaytesters(string input)
	{
		string[] array = (from p in input.Split(new string[1] { "||" }, StringSplitOptions.RemoveEmptyEntries)
			select p.Trim() into p
			where !string.IsNullOrWhiteSpace(p)
			select p).ToArray();
		for (int num = array.Length - 1; num > 0; num--)
		{
			int num2 = Rng.Chaotic.NextInt(num + 1);
			ref string reference = ref array[num];
			ref string reference2 = ref array[num2];
			string text = array[num2];
			string text2 = array[num];
			reference = text;
			reference2 = text2;
		}
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		for (int num3 = 0; num3 < array.Length; num3++)
		{
			switch (num3 % 3)
			{
			case 0:
				list.Add(array[num3]);
				break;
			case 1:
				list2.Add(array[num3]);
				break;
			case 2:
				list3.Add(array[num3]);
				break;
			}
		}
		return (Column1: string.Join("\n", list), Column2: string.Join("\n", list2), Column3: string.Join("\n", list3));
	}

	private static (string left, string right) SplitTwoColumnMultiRole(string input)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		string[] array = input.Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new string[1] { "||" }, StringSplitOptions.None);
			if (array2.Length == 2)
			{
				string text = array2[0].Trim();
				string text2 = array2[1].Trim();
				List<string> list3 = (from r in text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries)
					select r.Trim()).ToList();
				for (int num = 0; num < list3.Count; num++)
				{
					list.Add(list3[num]);
					list2.Add((num == 0) ? text2 : "");
				}
				bool flag = list3.Count > 1;
				bool flag2 = i == array.Length - 1;
				if (flag && !flag2)
				{
					list.Add("");
					list2.Add("");
				}
			}
		}
		return (left: string.Join("\n", list), right: string.Join("\n", list2));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Expected O, but got Unknown
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Expected O, but got Unknown
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(25);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitMegaCrit, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitComposer, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitAdditionalProgramming, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitAdditionalVfx, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitMarketingSupport, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitConsultants, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitVoices, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitLocalization, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitTwitchExtension, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitModdingSupport, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitPlaytesters, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitTrailer, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitFmod, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitSpine, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitGodot, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitExitMessage, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CloseScreenDebug, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._GuiInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessScrollEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShuffleOneColumn, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("input"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NCreditsScreen nCreditsScreen = Create();
			ret = VariantUtils.CreateFrom<NCreditsScreen>(ref nCreditsScreen);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitMegaCrit && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitMegaCrit();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitComposer && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitComposer();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitAdditionalProgramming && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitAdditionalProgramming();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitAdditionalVfx && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitAdditionalVfx();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitMarketingSupport && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitMarketingSupport();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitConsultants && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitConsultants();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitVoices && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitVoices();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitLocalization && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitLocalization();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitTwitchExtension && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitTwitchExtension();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitModdingSupport && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitModdingSupport();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitPlaytesters && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitPlaytesters();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitTrailer && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitTrailer();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitFmod && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitFmod();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitSpine && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitSpine();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitGodot && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitGodot();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitExitMessage && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitExitMessage();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CloseScreenDebug && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CloseScreenDebug();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._GuiInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Control)this)._GuiInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessScrollEvent && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessScrollEvent(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShuffleOneColumn && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text = ShuffleOneColumn(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NCreditsScreen nCreditsScreen = Create();
			ret = VariantUtils.CreateFrom<NCreditsScreen>(ref nCreditsScreen);
			return true;
		}
		if ((ref method) == MethodName.ShuffleOneColumn && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text = ShuffleOneColumn(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.InitMegaCrit)
		{
			return true;
		}
		if ((ref method) == MethodName.InitComposer)
		{
			return true;
		}
		if ((ref method) == MethodName.InitAdditionalProgramming)
		{
			return true;
		}
		if ((ref method) == MethodName.InitAdditionalVfx)
		{
			return true;
		}
		if ((ref method) == MethodName.InitMarketingSupport)
		{
			return true;
		}
		if ((ref method) == MethodName.InitConsultants)
		{
			return true;
		}
		if ((ref method) == MethodName.InitVoices)
		{
			return true;
		}
		if ((ref method) == MethodName.InitLocalization)
		{
			return true;
		}
		if ((ref method) == MethodName.InitTwitchExtension)
		{
			return true;
		}
		if ((ref method) == MethodName.InitModdingSupport)
		{
			return true;
		}
		if ((ref method) == MethodName.InitPlaytesters)
		{
			return true;
		}
		if ((ref method) == MethodName.InitTrailer)
		{
			return true;
		}
		if ((ref method) == MethodName.InitFmod)
		{
			return true;
		}
		if ((ref method) == MethodName.InitSpine)
		{
			return true;
		}
		if ((ref method) == MethodName.InitGodot)
		{
			return true;
		}
		if ((ref method) == MethodName.InitExitMessage)
		{
			return true;
		}
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.CloseScreenDebug)
		{
			return true;
		}
		if ((ref method) == MethodName._GuiInput)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessScrollEvent)
		{
			return true;
		}
		if ((ref method) == MethodName.ShuffleOneColumn)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._canClose)
		{
			_canClose = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._exitingScreen)
		{
			_exitingScreen = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._screenContents)
		{
			_screenContents = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			_backButton = VariantUtils.ConvertTo<NBackButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetPosition)
		{
			_targetPosition = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._canClose)
		{
			value = VariantUtils.CreateFrom<bool>(ref _canClose);
			return true;
		}
		if ((ref name) == PropertyName._exitingScreen)
		{
			value = VariantUtils.CreateFrom<bool>(ref _exitingScreen);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._screenContents)
		{
			value = VariantUtils.CreateFrom<Control>(ref _screenContents);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			value = VariantUtils.CreateFrom<NBackButton>(ref _backButton);
			return true;
		}
		if ((ref name) == PropertyName._targetPosition)
		{
			value = VariantUtils.CreateFrom<float>(ref _targetPosition);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName._canClose, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._exitingScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._screenContents, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._targetPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._canClose, Variant.From<bool>(ref _canClose));
		info.AddProperty(PropertyName._exitingScreen, Variant.From<bool>(ref _exitingScreen));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._screenContents, Variant.From<Control>(ref _screenContents));
		info.AddProperty(PropertyName._backButton, Variant.From<NBackButton>(ref _backButton));
		info.AddProperty(PropertyName._targetPosition, Variant.From<float>(ref _targetPosition));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._canClose, ref val))
		{
			_canClose = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._exitingScreen, ref val2))
		{
			_exitingScreen = ((Variant)(ref val2)).As<bool>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val3))
		{
			_tween = ((Variant)(ref val3)).As<Tween>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._screenContents, ref val4))
		{
			_screenContents = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._backButton, ref val5))
		{
			_backButton = ((Variant)(ref val5)).As<NBackButton>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetPosition, ref val6))
		{
			_targetPosition = ((Variant)(ref val6)).As<float>();
		}
	}
}
