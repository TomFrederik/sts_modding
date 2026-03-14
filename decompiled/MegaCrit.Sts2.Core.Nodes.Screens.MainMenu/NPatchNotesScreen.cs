using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

[ScriptPath("res://src/Core/Nodes/Screens/MainMenu/NPatchNotesScreen.cs")]
public class NPatchNotesScreen : Control, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName CreateNewPatchEntry = StringName.op_Implicit("CreateNewPatchEntry");

		public static readonly StringName NextPatchNote = StringName.op_Implicit("NextPatchNote");

		public static readonly StringName PreviousPatchNote = StringName.op_Implicit("PreviousPatchNote");

		public static readonly StringName Open = StringName.op_Implicit("Open");

		public static readonly StringName Close = StringName.op_Implicit("Close");

		public static readonly StringName LoadPatchNoteText = StringName.op_Implicit("LoadPatchNoteText");

		public static readonly StringName ReadPatchNoteFile = StringName.op_Implicit("ReadPatchNoteFile");

		public static readonly StringName UpdateDateLabel = StringName.op_Implicit("UpdateDateLabel");

		public static readonly StringName GetFileNameFromPath = StringName.op_Implicit("GetFileNameFromPath");

		public static readonly StringName RemoveFileExtension = StringName.op_Implicit("RemoveFileExtension");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName IsOpen = StringName.op_Implicit("IsOpen");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _screenContents = StringName.op_Implicit("_screenContents");

		public static readonly StringName _marginContainer = StringName.op_Implicit("_marginContainer");

		public static readonly StringName _prevButton = StringName.op_Implicit("_prevButton");

		public static readonly StringName _nextButton = StringName.op_Implicit("_nextButton");

		public static readonly StringName _patchNotesToggle = StringName.op_Implicit("_patchNotesToggle");

		public static readonly StringName _backButton = StringName.op_Implicit("_backButton");

		public static readonly StringName _dateLabel = StringName.op_Implicit("_dateLabel");

		public static readonly StringName _patchText = StringName.op_Implicit("_patchText");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _cachedScene = StringName.op_Implicit("_cachedScene");

		public static readonly StringName _index = StringName.op_Implicit("_index");

		public static readonly StringName _currentScrollLine = StringName.op_Implicit("_currentScrollLine");
	}

	public class SignalName : SignalName
	{
	}

	private NScrollableContainer _screenContents;

	private MarginContainer _marginContainer;

	private NButton _prevButton;

	private NButton _nextButton;

	private NButton _patchNotesToggle;

	private NButton _backButton;

	private MegaLabel _dateLabel;

	private MegaRichTextLabel _patchText;

	private Tween? _tween;

	private PackedScene _cachedScene;

	private const string _patchNotesPath = "res://localization/eng/patch_notes";

	private List<string>? _patchNotePaths;

	private int _index;

	private int _currentScrollLine;

	public bool IsOpen { get; private set; }

	public Control? DefaultFocusedControl => null;

	public override void _Ready()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		_cachedScene = ResourceLoader.Load<PackedScene>("res://scenes/screens/patch_screen_contents.tscn", (string)null, (CacheMode)1);
		CreateNewPatchEntry();
		_prevButton = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("PrevButton"));
		((GodotObject)_prevButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
		{
			PreviousPatchNote();
		}), 0u);
		_nextButton = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("NextButton"));
		((GodotObject)_nextButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
		{
			NextPatchNote();
		}), 0u);
		((CanvasItem)_nextButton).Visible = false;
		_patchNotesToggle = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("%PatchNotesToggle"));
		((GodotObject)_patchNotesToggle).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
		{
			Close();
		}), 0u);
		_patchNotesToggle.Disable();
		_backButton = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("%BackButton"));
		((GodotObject)_backButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
		{
			Close();
		}), 0u);
	}

	private void CreateNewPatchEntry()
	{
		_screenContents = _cachedScene.Instantiate<NScrollableContainer>((GenEditState)0);
		((Node)(object)this).AddChildSafely((Node?)(object)_screenContents);
		((Node)this).MoveChild((Node)(object)_screenContents, 0);
		_marginContainer = ((Node)_screenContents).GetNode<MarginContainer>(NodePath.op_Implicit("Content"));
		_patchText = ((Node)_screenContents).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("Content/PatchText"));
		_dateLabel = ((Node)_patchText).GetNode<MegaLabel>(NodePath.op_Implicit("DateLabel"));
		if (_patchNotePaths != null)
		{
			string patchNotePath = _patchNotePaths[_index];
			LoadPatchNoteText(patchNotePath);
		}
	}

	private void NextPatchNote()
	{
		if (!((CanvasItem)_nextButton).Visible)
		{
			return;
		}
		if (_patchNotePaths == null)
		{
			Log.Error("NPatchNotesScreen: No patch paths available!");
			return;
		}
		_index--;
		((CanvasItem)_prevButton).Visible = true;
		if (_index == 0)
		{
			((CanvasItem)_nextButton).Visible = false;
		}
		((Node)(object)_screenContents).QueueFreeSafely();
		CreateNewPatchEntry();
	}

	private void PreviousPatchNote()
	{
		if (_patchNotePaths == null)
		{
			Log.Error("NPatchNotesScreen: No patch paths available!");
			return;
		}
		_index++;
		((CanvasItem)_nextButton).Visible = true;
		if (_index == _patchNotePaths.Count - 1)
		{
			((CanvasItem)_prevButton).Visible = false;
		}
		((Node)(object)_screenContents).QueueFreeSafely();
		CreateNewPatchEntry();
	}

	public void Open()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		IsOpen = true;
		NGame.Instance.MainMenu?.EnableBackstop();
		_patchNotesToggle.Enable();
		_backButton.Enable();
		((CanvasItem)this).Visible = true;
		_tween?.FastForwardToCompletion();
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25);
		if (_patchNotePaths == null)
		{
			_patchNotePaths = (from fileName in DirAccess.GetFilesAt("res://localization/eng/patch_notes")
				select "res://localization/eng/patch_notes/" + fileName).Reverse().ToList();
		}
		LoadPatchNoteText(_patchNotePaths[_index]);
		ActiveScreenContext.Instance.Update();
		NHotkeyManager.Instance.PushHotkeyReleasedBinding(StringName.op_Implicit(MegaInput.left), PreviousPatchNote);
		NHotkeyManager.Instance.PushHotkeyReleasedBinding(StringName.op_Implicit(MegaInput.right), NextPatchNote);
		NHotkeyManager.Instance.PushHotkeyReleasedBinding(StringName.op_Implicit(MegaInput.pauseAndBack), Close);
	}

	private void Close()
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		NHotkeyManager.Instance.RemoveHotkeyReleasedBinding(StringName.op_Implicit(MegaInput.left), PreviousPatchNote);
		NHotkeyManager.Instance.RemoveHotkeyReleasedBinding(StringName.op_Implicit(MegaInput.right), NextPatchNote);
		NHotkeyManager.Instance.RemoveHotkeyReleasedBinding(StringName.op_Implicit(MegaInput.pauseAndBack), Close);
		_patchNotesToggle.Disable();
		_backButton.Disable();
		NGame.Instance.MainMenu?.DisableBackstop();
		_tween?.FastForwardToCompletion();
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.25);
		_tween.TweenCallback(Callable.From((Action)delegate
		{
			IsOpen = false;
			((CanvasItem)this).SetVisible(false);
			ActiveScreenContext.Instance.Update();
		}));
	}

	private void LoadPatchNoteText(string patchNotePath)
	{
		((RichTextLabel)_patchText).ScrollToLine(0);
		_currentScrollLine = 0;
		string textAutoSize = ReadPatchNoteFile(patchNotePath);
		_patchText.SetTextAutoSize(textAutoSize);
		UpdateDateLabel(patchNotePath);
	}

	private static string ReadPatchNoteFile(string patchNotePath)
	{
		FileAccess val = FileAccess.Open(patchNotePath, (ModeFlags)1);
		try
		{
			return val.GetAsText(false);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void UpdateDateLabel(string patchNotePath)
	{
		string fileNameFromPath = GetFileNameFromPath(patchNotePath);
		string text = RemoveFileExtension(fileNameFromPath);
		if (TryParseDate(text, out string formattedDate))
		{
			_dateLabel.SetTextAutoSize(formattedDate);
		}
		else
		{
			Log.Error("Invalid date format in file name: " + text);
		}
	}

	private static string GetFileNameFromPath(string path)
	{
		int num = path.LastIndexOf('/') + 1;
		return path.Substring(num, path.Length - num);
	}

	private static string RemoveFileExtension(string fileName)
	{
		return fileName.Split('.')[0];
	}

	private static bool TryParseDate(string dateString, out string formattedDate)
	{
		if (DateTime.TryParseExact(dateString, "yyyy_MM_d", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
		{
			formattedDate = result.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture);
			return true;
		}
		formattedDate = string.Empty;
		return false;
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
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(11);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CreateNewPatchEntry, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.NextPatchNote, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PreviousPatchNote, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Open, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Close, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.LoadPatchNoteText, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("patchNotePath"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ReadPatchNoteFile, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("patchNotePath"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateDateLabel, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("patchNotePath"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetFileNameFromPath, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("path"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveFileExtension, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("fileName"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CreateNewPatchEntry && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CreateNewPatchEntry();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.NextPatchNote && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NextPatchNote();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PreviousPatchNote && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PreviousPatchNote();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Open && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Open();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Close && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Close();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.LoadPatchNoteText && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			LoadPatchNoteText(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ReadPatchNoteFile && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text = ReadPatchNoteFile(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		if ((ref method) == MethodName.UpdateDateLabel && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateDateLabel(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetFileNameFromPath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string fileNameFromPath = GetFileNameFromPath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref fileNameFromPath);
			return true;
		}
		if ((ref method) == MethodName.RemoveFileExtension && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text2 = RemoveFileExtension(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text2);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.ReadPatchNoteFile && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text = ReadPatchNoteFile(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		if ((ref method) == MethodName.GetFileNameFromPath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string fileNameFromPath = GetFileNameFromPath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref fileNameFromPath);
			return true;
		}
		if ((ref method) == MethodName.RemoveFileExtension && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text2 = RemoveFileExtension(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text2);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.CreateNewPatchEntry)
		{
			return true;
		}
		if ((ref method) == MethodName.NextPatchNote)
		{
			return true;
		}
		if ((ref method) == MethodName.PreviousPatchNote)
		{
			return true;
		}
		if ((ref method) == MethodName.Open)
		{
			return true;
		}
		if ((ref method) == MethodName.Close)
		{
			return true;
		}
		if ((ref method) == MethodName.LoadPatchNoteText)
		{
			return true;
		}
		if ((ref method) == MethodName.ReadPatchNoteFile)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateDateLabel)
		{
			return true;
		}
		if ((ref method) == MethodName.GetFileNameFromPath)
		{
			return true;
		}
		if ((ref method) == MethodName.RemoveFileExtension)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.IsOpen)
		{
			IsOpen = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._screenContents)
		{
			_screenContents = VariantUtils.ConvertTo<NScrollableContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._marginContainer)
		{
			_marginContainer = VariantUtils.ConvertTo<MarginContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._prevButton)
		{
			_prevButton = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._nextButton)
		{
			_nextButton = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._patchNotesToggle)
		{
			_patchNotesToggle = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			_backButton = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dateLabel)
		{
			_dateLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._patchText)
		{
			_patchText = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cachedScene)
		{
			_cachedScene = VariantUtils.ConvertTo<PackedScene>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._index)
		{
			_index = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentScrollLine)
		{
			_currentScrollLine = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsOpen)
		{
			bool isOpen = IsOpen;
			value = VariantUtils.CreateFrom<bool>(ref isOpen);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._screenContents)
		{
			value = VariantUtils.CreateFrom<NScrollableContainer>(ref _screenContents);
			return true;
		}
		if ((ref name) == PropertyName._marginContainer)
		{
			value = VariantUtils.CreateFrom<MarginContainer>(ref _marginContainer);
			return true;
		}
		if ((ref name) == PropertyName._prevButton)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _prevButton);
			return true;
		}
		if ((ref name) == PropertyName._nextButton)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _nextButton);
			return true;
		}
		if ((ref name) == PropertyName._patchNotesToggle)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _patchNotesToggle);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _backButton);
			return true;
		}
		if ((ref name) == PropertyName._dateLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _dateLabel);
			return true;
		}
		if ((ref name) == PropertyName._patchText)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _patchText);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._cachedScene)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _cachedScene);
			return true;
		}
		if ((ref name) == PropertyName._index)
		{
			value = VariantUtils.CreateFrom<int>(ref _index);
			return true;
		}
		if ((ref name) == PropertyName._currentScrollLine)
		{
			value = VariantUtils.CreateFrom<int>(ref _currentScrollLine);
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
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._screenContents, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._marginContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._prevButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._nextButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._patchNotesToggle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dateLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._patchText, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cachedScene, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsOpen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._index, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._currentScrollLine, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName isOpen = PropertyName.IsOpen;
		bool isOpen2 = IsOpen;
		info.AddProperty(isOpen, Variant.From<bool>(ref isOpen2));
		info.AddProperty(PropertyName._screenContents, Variant.From<NScrollableContainer>(ref _screenContents));
		info.AddProperty(PropertyName._marginContainer, Variant.From<MarginContainer>(ref _marginContainer));
		info.AddProperty(PropertyName._prevButton, Variant.From<NButton>(ref _prevButton));
		info.AddProperty(PropertyName._nextButton, Variant.From<NButton>(ref _nextButton));
		info.AddProperty(PropertyName._patchNotesToggle, Variant.From<NButton>(ref _patchNotesToggle));
		info.AddProperty(PropertyName._backButton, Variant.From<NButton>(ref _backButton));
		info.AddProperty(PropertyName._dateLabel, Variant.From<MegaLabel>(ref _dateLabel));
		info.AddProperty(PropertyName._patchText, Variant.From<MegaRichTextLabel>(ref _patchText));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._cachedScene, Variant.From<PackedScene>(ref _cachedScene));
		info.AddProperty(PropertyName._index, Variant.From<int>(ref _index));
		info.AddProperty(PropertyName._currentScrollLine, Variant.From<int>(ref _currentScrollLine));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.IsOpen, ref val))
		{
			IsOpen = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._screenContents, ref val2))
		{
			_screenContents = ((Variant)(ref val2)).As<NScrollableContainer>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._marginContainer, ref val3))
		{
			_marginContainer = ((Variant)(ref val3)).As<MarginContainer>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._prevButton, ref val4))
		{
			_prevButton = ((Variant)(ref val4)).As<NButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._nextButton, ref val5))
		{
			_nextButton = ((Variant)(ref val5)).As<NButton>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._patchNotesToggle, ref val6))
		{
			_patchNotesToggle = ((Variant)(ref val6)).As<NButton>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._backButton, ref val7))
		{
			_backButton = ((Variant)(ref val7)).As<NButton>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._dateLabel, ref val8))
		{
			_dateLabel = ((Variant)(ref val8)).As<MegaLabel>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._patchText, ref val9))
		{
			_patchText = ((Variant)(ref val9)).As<MegaRichTextLabel>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val10))
		{
			_tween = ((Variant)(ref val10)).As<Tween>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._cachedScene, ref val11))
		{
			_cachedScene = ((Variant)(ref val11)).As<PackedScene>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._index, ref val12))
		{
			_index = ((Variant)(ref val12)).As<int>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentScrollLine, ref val13))
		{
			_currentScrollLine = ((Variant)(ref val13)).As<int>();
		}
	}
}
