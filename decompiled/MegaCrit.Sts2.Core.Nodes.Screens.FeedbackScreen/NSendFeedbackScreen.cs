using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;
using Sentry;

namespace MegaCrit.Sts2.Core.Nodes.Screens.FeedbackScreen;

[ScriptPath("res://src/Core/Nodes/Screens/FeedbackScreen/NSendFeedbackScreen.cs")]
public class NSendFeedbackScreen : Control, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Relocalize = StringName.op_Implicit("Relocalize");

		public static readonly StringName OnDescriptionChanged = StringName.op_Implicit("OnDescriptionChanged");

		public static readonly StringName SetScreenshot = StringName.op_Implicit("SetScreenshot");

		public static readonly StringName EmojiButtonSelected = StringName.op_Implicit("EmojiButtonSelected");

		public static readonly StringName SendButtonFocused = StringName.op_Implicit("SendButtonFocused");

		public static readonly StringName SendButtonUnfocused = StringName.op_Implicit("SendButtonUnfocused");

		public static readonly StringName Open = StringName.op_Implicit("Open");

		public static readonly StringName Close = StringName.op_Implicit("Close");

		public static readonly StringName ClearInput = StringName.op_Implicit("ClearInput");

		public static readonly StringName SetSelectedEmoji = StringName.op_Implicit("SetSelectedEmoji");

		public static readonly StringName SendButtonSelected = StringName.op_Implicit("SendButtonSelected");

		public static readonly StringName WiggleCartoons1 = StringName.op_Implicit("WiggleCartoons1");

		public static readonly StringName WiggleCartoons2 = StringName.op_Implicit("WiggleCartoons2");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _backButton = StringName.op_Implicit("_backButton");

		public static readonly StringName _mainPanel = StringName.op_Implicit("_mainPanel");

		public static readonly StringName _descriptionInput = StringName.op_Implicit("_descriptionInput");

		public static readonly StringName _emojiLabel = StringName.op_Implicit("_emojiLabel");

		public static readonly StringName _sendButton = StringName.op_Implicit("_sendButton");

		public static readonly StringName _sendLabel = StringName.op_Implicit("_sendLabel");

		public static readonly StringName _categoryDropdown = StringName.op_Implicit("_categoryDropdown");

		public static readonly StringName _successBackstop = StringName.op_Implicit("_successBackstop");

		public static readonly StringName _successPanel = StringName.op_Implicit("_successPanel");

		public static readonly StringName _successLabel = StringName.op_Implicit("_successLabel");

		public static readonly StringName _flower = StringName.op_Implicit("_flower");

		public static readonly StringName _selectedEmoteButton = StringName.op_Implicit("_selectedEmoteButton");

		public static readonly StringName _screenshotBytes = StringName.op_Implicit("_screenshotBytes");

		public static readonly StringName _originalSuccessPosition = StringName.op_Implicit("_originalSuccessPosition");

		public static readonly StringName _lastClosedMsec = StringName.op_Implicit("_lastClosedMsec");

		public static readonly StringName _descriptionText = StringName.op_Implicit("_descriptionText");

		public static readonly StringName _descriptionCaretLine = StringName.op_Implicit("_descriptionCaretLine");

		public static readonly StringName _descriptionCaretColumn = StringName.op_Implicit("_descriptionCaretColumn");

		public static readonly StringName _wiggleTween = StringName.op_Implicit("_wiggleTween");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/feedback_screen/feedback_screen");

	private const float _superWiggleTime = 0.25f;

	private const string _defaultUrl = "https://feedback.sts2.megacrit.com/feedback";

	private static readonly string _url = Environment.GetEnvironmentVariable("STS2_FEEDBACK_URL") ?? "https://feedback.sts2.megacrit.com/feedback";

	private static readonly HttpClient _httpClient = new HttpClient();

	private const int _maxDescriptionChars = 500;

	private NBackButton _backButton;

	private Control _mainPanel;

	private NMegaTextEdit _descriptionInput;

	private MegaLabel _emojiLabel;

	private NButton _sendButton;

	private MegaLabel _sendLabel;

	private NFeedbackCategoryDropdown _categoryDropdown;

	private Control _successBackstop;

	private Control _successPanel;

	private MegaLabel _successLabel;

	private List<NSendFeedbackCartoon> _cartoons = new List<NSendFeedbackCartoon>();

	private NSendFeedbackFlower _flower;

	private CancellationTokenSource? _cancelToken;

	private CancellationTokenSource? _sendCancelToken;

	private readonly List<NSendFeedbackEmojiButton> _emojiButtons = new List<NSendFeedbackEmojiButton>();

	private NSendFeedbackEmojiButton? _selectedEmoteButton;

	private byte[]? _screenshotBytes;

	private Vector2 _originalSuccessPosition;

	private ulong _lastClosedMsec;

	private string _descriptionText = string.Empty;

	private int _descriptionCaretLine;

	private int _descriptionCaretColumn;

	private Tween? _wiggleTween;

	public Control DefaultFocusedControl => (Control)(object)_descriptionInput;

	public static NSendFeedbackScreen? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NSendFeedbackScreen>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		_mainPanel = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%MainPanel"));
		_descriptionInput = ((Node)this).GetNode<NMegaTextEdit>(NodePath.op_Implicit("%DescriptionInput"));
		_emojiLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%EmojiLabel"));
		_sendButton = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("%SendButton"));
		_sendLabel = ((Node)_sendButton).GetNode<MegaLabel>(NodePath.op_Implicit("Label"));
		_categoryDropdown = ((Node)this).GetNode<NFeedbackCategoryDropdown>(NodePath.op_Implicit("%CategoryDropdown"));
		_successBackstop = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%SuccessBackstop"));
		_successPanel = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%SuccessPanel"));
		_successLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%SuccessLabel"));
		_backButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("BackButton"));
		_originalSuccessPosition = _successPanel.Position;
		int num = 3;
		List<NSendFeedbackCartoon> list = new List<NSendFeedbackCartoon>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<NSendFeedbackCartoon> span = CollectionsMarshal.AsSpan(list);
		int num2 = 0;
		span[num2] = ((Node)this).GetNode<NSendFeedbackCartoon>(NodePath.op_Implicit("Sun"));
		num2++;
		span[num2] = ((Node)this).GetNode<NSendFeedbackCartoon>(NodePath.op_Implicit("Cupcake"));
		num2++;
		span[num2] = ((Node)this).GetNode<NSendFeedbackCartoon>(NodePath.op_Implicit("FlowerContainer/Flower"));
		_cartoons = list;
		_flower = ((Node)this).GetNode<NSendFeedbackFlower>(NodePath.op_Implicit("FlowerContainer"));
		foreach (Node child in ((Node)this).GetNode(NodePath.op_Implicit("%EmojiButtonContainer")).GetChildren(false))
		{
			if (child is NSendFeedbackEmojiButton nSendFeedbackEmojiButton)
			{
				_emojiButtons.Add(nSendFeedbackEmojiButton);
				((Control)nSendFeedbackEmojiButton).PivotOffset = ((Control)nSendFeedbackEmojiButton).Size * 0.5f;
				((GodotObject)nSendFeedbackEmojiButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)EmojiButtonSelected), 0u);
			}
		}
		((GodotObject)_sendButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)SendButtonSelected), 0u);
		((GodotObject)_sendButton).Connect(NClickableControl.SignalName.Focused, Callable.From<NClickableControl>((Action<NClickableControl>)SendButtonFocused), 0u);
		((GodotObject)_sendButton).Connect(NClickableControl.SignalName.Unfocused, Callable.From<NClickableControl>((Action<NClickableControl>)SendButtonUnfocused), 0u);
		((GodotObject)_descriptionInput).Connect(SignalName.TextChanged, Callable.From((Action)OnDescriptionChanged), 0u);
		((Control)_sendButton).FocusNeighborTop = ((Node)_categoryDropdown).GetPath();
		((Control)_sendButton).FocusNeighborLeft = ((Node)_emojiButtons.Last()).GetPath();
		((Control)_sendButton).FocusNeighborBottom = ((Node)_sendButton).GetPath();
		((Control)_sendButton).FocusNeighborRight = ((Node)_sendButton).GetPath();
		((Control)_emojiButtons.Last()).FocusNeighborRight = ((Node)_sendButton).GetPath();
		foreach (NSendFeedbackEmojiButton emojiButton in _emojiButtons)
		{
			((Control)emojiButton).FocusNeighborTop = ((Node)_categoryDropdown).GetPath();
			((Control)emojiButton).FocusNeighborBottom = ((Node)emojiButton).GetPath();
		}
		((Control)_categoryDropdown).FocusNeighborRight = ((Node)_sendButton).GetPath();
		((Control)_categoryDropdown).FocusNeighborBottom = ((Node)_emojiButtons.First()).GetPath();
		((Control)_categoryDropdown).FocusNeighborTop = ((Node)_descriptionInput).GetPath();
		((Control)_descriptionInput).FocusNeighborTop = ((Node)_descriptionInput).GetPath();
		((Control)_descriptionInput).FocusNeighborLeft = ((Node)_descriptionInput).GetPath();
		((Control)_descriptionInput).FocusNeighborRight = ((Node)_descriptionInput).GetPath();
		((Control)_descriptionInput).FocusNeighborBottom = ((Node)_categoryDropdown).GetPath();
		((GodotObject)_backButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
		{
			Close();
		}), 0u);
		((CanvasItem)this).Visible = false;
		((Control)this).MouseFilter = (MouseFilterEnum)2;
		_backButton.Disable();
	}

	public void Relocalize()
	{
		((TextEdit)_descriptionInput).PlaceholderText = new LocString("settings_ui", "FEEDBACK_DESCRIPTION_PLACEHOLDER").GetFormattedText();
		_emojiLabel.SetTextAutoSize(new LocString("settings_ui", "FEEDBACK_EMOJI_LABEL").GetFormattedText());
		_sendLabel.SetTextAutoSize(new LocString("settings_ui", "FEEDBACK_SEND_BUTTON_LABEL").GetFormattedText());
		_descriptionInput.RefreshFont();
		_emojiLabel.RefreshFont();
		_sendLabel.RefreshFont();
	}

	private void OnDescriptionChanged()
	{
		if (((TextEdit)_descriptionInput).Text.Length > 500)
		{
			((TextEdit)_descriptionInput).Text = _descriptionText;
			((TextEdit)_descriptionInput).SetCaretLine(_descriptionCaretLine, true, true, 0, 0);
			((TextEdit)_descriptionInput).SetCaretColumn(_descriptionCaretColumn, true, 0);
		}
		else
		{
			_descriptionText = ((TextEdit)_descriptionInput).Text;
			_descriptionCaretLine = ((TextEdit)_descriptionInput).GetCaretLine(0);
			_descriptionCaretColumn = ((TextEdit)_descriptionInput).GetCaretColumn(0);
		}
	}

	public void SetScreenshot(Image screenshot)
	{
		int width = screenshot.GetWidth();
		int height = screenshot.GetHeight();
		float num = (float)width / (float)height;
		if (width > 1280)
		{
			screenshot.Resize(1280, Mathf.RoundToInt(1280f / num), (Interpolation)1);
		}
		if (height > 720)
		{
			screenshot.Resize(Mathf.RoundToInt(720f * num), 720, (Interpolation)1);
		}
		_screenshotBytes = screenshot.SavePngToBuffer();
	}

	private void EmojiButtonSelected(NButton button)
	{
		SetSelectedEmoji((NSendFeedbackEmojiButton)button);
	}

	private void SendButtonFocused(NClickableControl _)
	{
		_flower.SetState(NSendFeedbackFlower.State.Anticipation);
	}

	private void SendButtonUnfocused(NClickableControl _)
	{
		if (_flower.MyState == NSendFeedbackFlower.State.Anticipation)
		{
			_flower.SetState(NSendFeedbackFlower.State.None);
		}
	}

	public void Open()
	{
		Log.Info("Feedback screen opened");
		if (Time.GetTicksMsec() - _lastClosedMsec > 60000)
		{
			ClearInput();
		}
		((CanvasItem)this).Visible = true;
		_flower.SetState(NSendFeedbackFlower.State.None);
		((CanvasItem)_successBackstop).Visible = false;
		((Control)this).MouseFilter = (MouseFilterEnum)0;
		NHotkeyManager.Instance.AddBlockingScreen((Node)(object)this);
		ActiveScreenContext.Instance.Update();
		_backButton.Enable();
	}

	private void Close()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Log.Info("Feedback screen closed");
		_flower.SetState(NSendFeedbackFlower.State.None);
		((CanvasItem)_successBackstop).Visible = false;
		((CanvasItem)_mainPanel).Modulate = Colors.White;
		Tween? wiggleTween = _wiggleTween;
		if (wiggleTween != null)
		{
			wiggleTween.Kill();
		}
		((CanvasItem)this).Visible = false;
		_lastClosedMsec = Time.GetTicksMsec();
		_cancelToken?.Cancel();
		((Control)this).MouseFilter = (MouseFilterEnum)2;
		_backButton.Disable();
		NHotkeyManager.Instance.RemoveBlockingScreen((Node)(object)this);
		ActiveScreenContext.Instance.Update();
	}

	private void ClearInput()
	{
		((TextEdit)_descriptionInput).Text = string.Empty;
		_descriptionText = string.Empty;
		SetSelectedEmoji(null);
	}

	private void SetSelectedEmoji(NSendFeedbackEmojiButton? button)
	{
		NSendFeedbackEmojiButton selectedEmoteButton = _selectedEmoteButton;
		_selectedEmoteButton?.SetSelected(isSelected: false);
		if (selectedEmoteButton != button)
		{
			_selectedEmoteButton = button;
			_selectedEmoteButton?.SetSelected(isSelected: true);
		}
	}

	private void SendButtonSelected(NButton _)
	{
		Log.Info("Beginning asynchronous feedback send at " + Log.Timestamp + ": " + _descriptionText);
		ReleaseInfo releaseInfo = ReleaseInfoManager.Instance.ReleaseInfo;
		string text = releaseInfo?.Commit ?? GitHelper.ShortCommitId;
		FeedbackData data = new FeedbackData
		{
			description = _descriptionText,
			category = _categoryDropdown.CurrentCategory,
			gameVersion = (releaseInfo?.Version ?? GitHelper.ShortCommitId ?? "unknown"),
			uniqueId = SaveManager.Instance.Progress.UniqueId,
			commit = (text ?? "unknown"),
			platformBranch = PlatformUtil.GetPlatformBranch()
		};
		byte[] screenshotBytes = _screenshotBytes;
		int currentProfileId = SaveManager.Instance.CurrentProfileId;
		_sendCancelToken?.Cancel();
		_sendCancelToken?.Dispose();
		_sendCancelToken = new CancellationTokenSource();
		TaskHelper.RunSafely(SendFeedback(data, screenshotBytes, currentProfileId, _sendCancelToken.Token));
		ClearInput();
		_screenshotBytes = null;
		TaskHelper.RunSafely(OnFeedbackSuccess());
	}

	private static async Task SendFeedback(FeedbackData data, byte[] screenshotBytes, int profileId, CancellationToken cancellationToken)
	{
		if (string.IsNullOrEmpty(data.description))
		{
			return;
		}
		using MemoryStream logsMemoryStream = new MemoryStream();
		GetLogsConsoleCmd.ZipFeedbackLogs(logsMemoryStream, profileId);
		byte[] logsZipBytes = logsMemoryStream.ToArray();
		using MultipartFormDataContent formContent = BuildMultipartContent(data, screenshotBytes, logsZipBytes);
		byte[] bodyBytes = await formContent.ReadAsByteArrayAsync(cancellationToken);
		MediaTypeHeaderValue contentType = formContent.Headers.ContentType;
		int[] delaysMs = new int[3] { 1000, 2000, 4000 };
		string sentryMessage = null;
		for (int attempt = 0; attempt <= 3; attempt++)
		{
			cancellationToken.ThrowIfCancellationRequested();
			try
			{
				using ByteArrayContent content = new ByteArrayContent(bodyBytes);
				content.Headers.ContentType = contentType;
				using HttpResponseMessage response = await _httpClient.PutAsync(_url, content, cancellationToken);
				if (response.IsSuccessStatusCode)
				{
					Log.Info("Feedback successfully posted!");
					return;
				}
				int statusCode = (int)response.StatusCode;
				if (statusCode >= 400 && statusCode < 500 && statusCode != 429)
				{
					string value = await response.Content.ReadAsStringAsync(cancellationToken);
					Log.Warn($"Feedback rejected ({response.StatusCode}): {value}");
					SentrySdk.CaptureMessage($"Feedback rejected: Response status code {response.StatusCode}", (SentryLevel)1);
					return;
				}
				sentryMessage = $"Response status code {response.StatusCode}";
				Log.Warn($"Feedback attempt {attempt + 1}/{4} failed: {response.StatusCode}");
			}
			catch (HttpRequestException ex)
			{
				string text = $"Feedback attempt {attempt + 1}/{4} network error: {ExceptionMessageWithInner(ex)} {ex.HttpRequestError}";
				if (ex.HttpRequestError != HttpRequestError.NameResolutionError)
				{
					sentryMessage = "HttpRequestException: " + ExceptionMessageWithInner(ex);
				}
				Log.Warn(text);
			}
			if (attempt < 3)
			{
				await Task.Delay(delaysMs[attempt], cancellationToken);
			}
		}
		Log.Warn("Feedback send failed after all retry attempts");
		if (sentryMessage != null)
		{
			SentrySdk.CaptureMessage("Feedback failed to send: " + sentryMessage, (SentryLevel)1);
		}
	}

	private static string ExceptionMessageWithInner(Exception ex)
	{
		if (ex.InnerException == null)
		{
			return ex.Message;
		}
		return ex.Message + " | " + ExceptionMessageWithInner(ex.InnerException);
	}

	private static MultipartFormDataContent BuildMultipartContent(FeedbackData data, byte[] screenshotBytes, byte[] logsZipBytes)
	{
		string content = JsonSerializer.Serialize(data, JsonSerializationUtility.GetTypeInfo<FeedbackData>());
		MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
		StringContent stringContent = new StringContent(content);
		stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
		stringContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
		{
			Name = "payload_json"
		};
		multipartFormDataContent.Add(stringContent);
		ByteArrayContent byteArrayContent = new ByteArrayContent(screenshotBytes);
		byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
		byteArrayContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
		{
			Name = "screenshot"
		};
		multipartFormDataContent.Add(byteArrayContent);
		ByteArrayContent byteArrayContent2 = new ByteArrayContent(logsZipBytes);
		byteArrayContent2.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
		byteArrayContent2.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
		{
			Name = "logs"
		};
		multipartFormDataContent.Add(byteArrayContent2);
		return multipartFormDataContent;
	}

	private async Task OnFeedbackSuccess()
	{
		((CanvasItem)_successBackstop).Visible = true;
		((CanvasItem)_successPanel).Modulate = Colors.Transparent;
		Control successPanel = _successPanel;
		Vector2 position = _successPanel.Position;
		position.Y = _originalSuccessPosition.Y + 20f;
		successPanel.Position = position;
		_successLabel.SetTextAutoSize(new LocString("settings_ui", "FEEDBACK_SEND_SUCCESS_LABEL").GetFormattedText());
		((CanvasItem)_successLabel).Modulate = StsColors.green;
		Tween val = ((Node)this).GetTree().CreateTween().Parallel();
		val.TweenProperty((GodotObject)(object)_mainPanel, NodePath.op_Implicit("modulate"), Variant.op_Implicit(new Color(0.1f, 0.1f, 0.1f, 1f)), 0.15000000596046448);
		val.TweenProperty((GodotObject)(object)_successPanel, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.15000000596046448);
		val.TweenProperty((GodotObject)(object)_successPanel, NodePath.op_Implicit("position:y"), Variant.op_Implicit(_originalSuccessPosition.Y), 0.15000000596046448).SetEase((EaseType)1).SetTrans((TransitionType)4);
		val.Chain().TweenProperty((GodotObject)(object)_successLabel, NodePath.op_Implicit("position:y"), Variant.op_Implicit(((Control)_successLabel).Position.Y - 10f), 0.10000000149011612).SetEase((EaseType)1)
			.SetTrans((TransitionType)4);
		val.Chain().TweenProperty((GodotObject)(object)_successLabel, NodePath.op_Implicit("position:y"), Variant.op_Implicit(((Control)_successLabel).Position.Y), 0.10000000149011612).SetEase((EaseType)0)
			.SetTrans((TransitionType)4);
		Tween? wiggleTween = _wiggleTween;
		if (wiggleTween != null)
		{
			wiggleTween.Kill();
		}
		_wiggleTween = ((Node)this).CreateTween();
		_wiggleTween.TweenCallback(Callable.From((Action)WiggleCartoons1));
		_wiggleTween.TweenInterval(0.25);
		_wiggleTween.TweenCallback(Callable.From((Action)WiggleCartoons2));
		_wiggleTween.TweenInterval(0.25);
		_wiggleTween.SetLoops(0);
		string scenePath = SceneHelper.GetScenePath("vfx/vfx_dramatic_entrance_fullscreen");
		Node2D val2 = PreloadManager.Cache.GetScene(scenePath).Instantiate<Node2D>((GenEditState)0);
		((Node)(object)this).AddChildSafely((Node?)(object)val2);
		((Node)this).MoveChild((Node)(object)val2, 1);
		Rect2 viewportRect = ((CanvasItem)NGame.Instance).GetViewportRect();
		val2.GlobalPosition = ((Rect2)(ref viewportRect)).Size * 0.5f;
		_flower.SetState(NSendFeedbackFlower.State.NoddingFast);
		_cancelToken = new CancellationTokenSource();
		await Task.Delay(2000, _cancelToken.Token);
		Close();
	}

	private void WiggleCartoons1()
	{
		foreach (NSendFeedbackCartoon cartoon in _cartoons)
		{
			if (_flower.MyState == NSendFeedbackFlower.State.None || cartoon != _flower.Cartoon)
			{
				cartoon.SetRotation1();
			}
		}
	}

	private void WiggleCartoons2()
	{
		foreach (NSendFeedbackCartoon cartoon in _cartoons)
		{
			if (_flower.MyState == NSendFeedbackFlower.State.None || cartoon != _flower.Cartoon)
			{
				cartoon.SetRotation2();
			}
		}
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
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Expected O, but got Unknown
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Expected O, but got Unknown
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Expected O, but got Unknown
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Expected O, but got Unknown
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(15);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Relocalize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDescriptionChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetScreenshot, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("screenshot"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Image"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EmojiButtonSelected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SendButtonFocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SendButtonUnfocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Open, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Close, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetSelectedEmoji, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SendButtonSelected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.WiggleCartoons1, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.WiggleCartoons2, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NSendFeedbackScreen nSendFeedbackScreen = Create();
			ret = VariantUtils.CreateFrom<NSendFeedbackScreen>(ref nSendFeedbackScreen);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Relocalize && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Relocalize();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDescriptionChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDescriptionChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetScreenshot && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetScreenshot(VariantUtils.ConvertTo<Image>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EmojiButtonSelected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			EmojiButtonSelected(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SendButtonFocused && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SendButtonFocused(VariantUtils.ConvertTo<NClickableControl>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SendButtonUnfocused && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SendButtonUnfocused(VariantUtils.ConvertTo<NClickableControl>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.ClearInput && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearInput();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetSelectedEmoji && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetSelectedEmoji(VariantUtils.ConvertTo<NSendFeedbackEmojiButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SendButtonSelected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SendButtonSelected(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.WiggleCartoons1 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			WiggleCartoons1();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.WiggleCartoons2 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			WiggleCartoons2();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NSendFeedbackScreen nSendFeedbackScreen = Create();
			ret = VariantUtils.CreateFrom<NSendFeedbackScreen>(ref nSendFeedbackScreen);
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
		if ((ref method) == MethodName.Relocalize)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDescriptionChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.SetScreenshot)
		{
			return true;
		}
		if ((ref method) == MethodName.EmojiButtonSelected)
		{
			return true;
		}
		if ((ref method) == MethodName.SendButtonFocused)
		{
			return true;
		}
		if ((ref method) == MethodName.SendButtonUnfocused)
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
		if ((ref method) == MethodName.ClearInput)
		{
			return true;
		}
		if ((ref method) == MethodName.SetSelectedEmoji)
		{
			return true;
		}
		if ((ref method) == MethodName.SendButtonSelected)
		{
			return true;
		}
		if ((ref method) == MethodName.WiggleCartoons1)
		{
			return true;
		}
		if ((ref method) == MethodName.WiggleCartoons2)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._backButton)
		{
			_backButton = VariantUtils.ConvertTo<NBackButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mainPanel)
		{
			_mainPanel = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._descriptionInput)
		{
			_descriptionInput = VariantUtils.ConvertTo<NMegaTextEdit>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._emojiLabel)
		{
			_emojiLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sendButton)
		{
			_sendButton = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sendLabel)
		{
			_sendLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._categoryDropdown)
		{
			_categoryDropdown = VariantUtils.ConvertTo<NFeedbackCategoryDropdown>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._successBackstop)
		{
			_successBackstop = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._successPanel)
		{
			_successPanel = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._successLabel)
		{
			_successLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._flower)
		{
			_flower = VariantUtils.ConvertTo<NSendFeedbackFlower>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectedEmoteButton)
		{
			_selectedEmoteButton = VariantUtils.ConvertTo<NSendFeedbackEmojiButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._screenshotBytes)
		{
			_screenshotBytes = VariantUtils.ConvertTo<byte[]>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._originalSuccessPosition)
		{
			_originalSuccessPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lastClosedMsec)
		{
			_lastClosedMsec = VariantUtils.ConvertTo<ulong>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._descriptionText)
		{
			_descriptionText = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._descriptionCaretLine)
		{
			_descriptionCaretLine = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._descriptionCaretColumn)
		{
			_descriptionCaretColumn = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._wiggleTween)
		{
			_wiggleTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			value = VariantUtils.CreateFrom<NBackButton>(ref _backButton);
			return true;
		}
		if ((ref name) == PropertyName._mainPanel)
		{
			value = VariantUtils.CreateFrom<Control>(ref _mainPanel);
			return true;
		}
		if ((ref name) == PropertyName._descriptionInput)
		{
			value = VariantUtils.CreateFrom<NMegaTextEdit>(ref _descriptionInput);
			return true;
		}
		if ((ref name) == PropertyName._emojiLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _emojiLabel);
			return true;
		}
		if ((ref name) == PropertyName._sendButton)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _sendButton);
			return true;
		}
		if ((ref name) == PropertyName._sendLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _sendLabel);
			return true;
		}
		if ((ref name) == PropertyName._categoryDropdown)
		{
			value = VariantUtils.CreateFrom<NFeedbackCategoryDropdown>(ref _categoryDropdown);
			return true;
		}
		if ((ref name) == PropertyName._successBackstop)
		{
			value = VariantUtils.CreateFrom<Control>(ref _successBackstop);
			return true;
		}
		if ((ref name) == PropertyName._successPanel)
		{
			value = VariantUtils.CreateFrom<Control>(ref _successPanel);
			return true;
		}
		if ((ref name) == PropertyName._successLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _successLabel);
			return true;
		}
		if ((ref name) == PropertyName._flower)
		{
			value = VariantUtils.CreateFrom<NSendFeedbackFlower>(ref _flower);
			return true;
		}
		if ((ref name) == PropertyName._selectedEmoteButton)
		{
			value = VariantUtils.CreateFrom<NSendFeedbackEmojiButton>(ref _selectedEmoteButton);
			return true;
		}
		if ((ref name) == PropertyName._screenshotBytes)
		{
			value = VariantUtils.CreateFrom<byte[]>(ref _screenshotBytes);
			return true;
		}
		if ((ref name) == PropertyName._originalSuccessPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _originalSuccessPosition);
			return true;
		}
		if ((ref name) == PropertyName._lastClosedMsec)
		{
			value = VariantUtils.CreateFrom<ulong>(ref _lastClosedMsec);
			return true;
		}
		if ((ref name) == PropertyName._descriptionText)
		{
			value = VariantUtils.CreateFrom<string>(ref _descriptionText);
			return true;
		}
		if ((ref name) == PropertyName._descriptionCaretLine)
		{
			value = VariantUtils.CreateFrom<int>(ref _descriptionCaretLine);
			return true;
		}
		if ((ref name) == PropertyName._descriptionCaretColumn)
		{
			value = VariantUtils.CreateFrom<int>(ref _descriptionCaretColumn);
			return true;
		}
		if ((ref name) == PropertyName._wiggleTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _wiggleTween);
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
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._backButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mainPanel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._descriptionInput, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._emojiLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._sendButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._sendLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._categoryDropdown, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._successBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._successPanel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._successLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._flower, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectedEmoteButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)29, PropertyName._screenshotBytes, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._originalSuccessPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._lastClosedMsec, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._descriptionText, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._descriptionCaretLine, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._descriptionCaretColumn, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._wiggleTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._backButton, Variant.From<NBackButton>(ref _backButton));
		info.AddProperty(PropertyName._mainPanel, Variant.From<Control>(ref _mainPanel));
		info.AddProperty(PropertyName._descriptionInput, Variant.From<NMegaTextEdit>(ref _descriptionInput));
		info.AddProperty(PropertyName._emojiLabel, Variant.From<MegaLabel>(ref _emojiLabel));
		info.AddProperty(PropertyName._sendButton, Variant.From<NButton>(ref _sendButton));
		info.AddProperty(PropertyName._sendLabel, Variant.From<MegaLabel>(ref _sendLabel));
		info.AddProperty(PropertyName._categoryDropdown, Variant.From<NFeedbackCategoryDropdown>(ref _categoryDropdown));
		info.AddProperty(PropertyName._successBackstop, Variant.From<Control>(ref _successBackstop));
		info.AddProperty(PropertyName._successPanel, Variant.From<Control>(ref _successPanel));
		info.AddProperty(PropertyName._successLabel, Variant.From<MegaLabel>(ref _successLabel));
		info.AddProperty(PropertyName._flower, Variant.From<NSendFeedbackFlower>(ref _flower));
		info.AddProperty(PropertyName._selectedEmoteButton, Variant.From<NSendFeedbackEmojiButton>(ref _selectedEmoteButton));
		info.AddProperty(PropertyName._screenshotBytes, Variant.From<byte[]>(ref _screenshotBytes));
		info.AddProperty(PropertyName._originalSuccessPosition, Variant.From<Vector2>(ref _originalSuccessPosition));
		info.AddProperty(PropertyName._lastClosedMsec, Variant.From<ulong>(ref _lastClosedMsec));
		info.AddProperty(PropertyName._descriptionText, Variant.From<string>(ref _descriptionText));
		info.AddProperty(PropertyName._descriptionCaretLine, Variant.From<int>(ref _descriptionCaretLine));
		info.AddProperty(PropertyName._descriptionCaretColumn, Variant.From<int>(ref _descriptionCaretColumn));
		info.AddProperty(PropertyName._wiggleTween, Variant.From<Tween>(ref _wiggleTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._backButton, ref val))
		{
			_backButton = ((Variant)(ref val)).As<NBackButton>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._mainPanel, ref val2))
		{
			_mainPanel = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._descriptionInput, ref val3))
		{
			_descriptionInput = ((Variant)(ref val3)).As<NMegaTextEdit>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._emojiLabel, ref val4))
		{
			_emojiLabel = ((Variant)(ref val4)).As<MegaLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._sendButton, ref val5))
		{
			_sendButton = ((Variant)(ref val5)).As<NButton>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._sendLabel, ref val6))
		{
			_sendLabel = ((Variant)(ref val6)).As<MegaLabel>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._categoryDropdown, ref val7))
		{
			_categoryDropdown = ((Variant)(ref val7)).As<NFeedbackCategoryDropdown>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._successBackstop, ref val8))
		{
			_successBackstop = ((Variant)(ref val8)).As<Control>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._successPanel, ref val9))
		{
			_successPanel = ((Variant)(ref val9)).As<Control>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._successLabel, ref val10))
		{
			_successLabel = ((Variant)(ref val10)).As<MegaLabel>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._flower, ref val11))
		{
			_flower = ((Variant)(ref val11)).As<NSendFeedbackFlower>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectedEmoteButton, ref val12))
		{
			_selectedEmoteButton = ((Variant)(ref val12)).As<NSendFeedbackEmojiButton>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._screenshotBytes, ref val13))
		{
			_screenshotBytes = ((Variant)(ref val13)).As<byte[]>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._originalSuccessPosition, ref val14))
		{
			_originalSuccessPosition = ((Variant)(ref val14)).As<Vector2>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._lastClosedMsec, ref val15))
		{
			_lastClosedMsec = ((Variant)(ref val15)).As<ulong>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._descriptionText, ref val16))
		{
			_descriptionText = ((Variant)(ref val16)).As<string>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._descriptionCaretLine, ref val17))
		{
			_descriptionCaretLine = ((Variant)(ref val17)).As<int>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._descriptionCaretColumn, ref val18))
		{
			_descriptionCaretColumn = ((Variant)(ref val18)).As<int>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._wiggleTween, ref val19))
		{
			_wiggleTween = ((Variant)(ref val19)).As<Tween>();
		}
	}
}
