using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.ControllerInput.ControllerConfigs;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.ControllerInput;

public abstract class ControllerConfig
{
	private Dictionary<string, string>? _glyphs;

	protected abstract string FolderPath { get; }

	public virtual ControllerMappingType ControllerMappingType => ControllerMappingType.Default;

	public virtual Dictionary<string, StringName> SteamInputControllerMap => new Dictionary<string, StringName>
	{
		{
			"Confirm",
			Controller.faceButtonNorth
		},
		{
			"Cancel",
			Controller.faceButtonEast
		},
		{
			"Up",
			Controller.dPadNorth
		},
		{
			"Down",
			Controller.dPadSouth
		},
		{
			"Left",
			Controller.dPadWest
		},
		{
			"Right",
			Controller.dPadEast
		},
		{
			"Select",
			Controller.faceButtonSouth
		},
		{
			"Top_Panel",
			Controller.faceButtonWest
		},
		{
			"View_Draw_Pile",
			Controller.leftTrigger
		},
		{
			"View_Discard_Pile",
			Controller.rightTrigger
		},
		{
			"Tab_Right",
			Controller.rightBumper
		},
		{
			"Tab_Left",
			Controller.leftBumper
		},
		{
			"View_Map",
			Controller.selectButton
		},
		{
			"Settings",
			Controller.startButton
		},
		{
			"Peek",
			Controller.joystickPress
		}
	};

	public virtual Dictionary<StringName, StringName> DefaultControllerInputMap => new Dictionary<StringName, StringName>
	{
		{
			MegaInput.accept,
			Controller.faceButtonNorth
		},
		{
			MegaInput.cancel,
			Controller.faceButtonEast
		},
		{
			MegaInput.select,
			Controller.faceButtonSouth
		},
		{
			MegaInput.viewExhaustPileAndTabRight,
			Controller.rightBumper
		},
		{
			MegaInput.viewDeckAndTabLeft,
			Controller.leftBumper
		},
		{
			MegaInput.topPanel,
			Controller.faceButtonWest
		},
		{
			MegaInput.viewDrawPile,
			Controller.leftTrigger
		},
		{
			MegaInput.viewDiscardPile,
			Controller.rightTrigger
		},
		{
			MegaInput.viewMap,
			Controller.selectButton
		},
		{
			MegaInput.peek,
			Controller.joystickPress
		},
		{
			MegaInput.up,
			Controller.dPadNorth
		},
		{
			MegaInput.down,
			Controller.dPadSouth
		},
		{
			MegaInput.left,
			Controller.dPadWest
		},
		{
			MegaInput.right,
			Controller.dPadEast
		},
		{
			MegaInput.pauseAndBack,
			Controller.startButton
		}
	};

	protected virtual string FaceButtonNorthGlyph => ImageHelper.GetImagePath(FolderPath + "/y.tres");

	protected virtual string FaceButtonSouthGlyph => ImageHelper.GetImagePath(FolderPath + "/a.tres");

	protected virtual string FaceButtonEastGlyph => ImageHelper.GetImagePath(FolderPath + "/b.tres");

	protected virtual string FaceButtonWestGlyph => ImageHelper.GetImagePath(FolderPath + "/x.tres");

	private string LeftTriggerGlyph => ImageHelper.GetImagePath(FolderPath + "/lt.tres");

	private string RightTriggerGlyph => ImageHelper.GetImagePath(FolderPath + "/rt.tres");

	private string SelectButtonGlyph => ImageHelper.GetImagePath(FolderPath + "/back.tres");

	private string LeftBumperGlyph => ImageHelper.GetImagePath(FolderPath + "/lb.tres");

	private string RightBumperGlyph => ImageHelper.GetImagePath(FolderPath + "/rb.tres");

	private string StartButtonGlyph => ImageHelper.GetImagePath(FolderPath + "/start.tres");

	private string JoystickPressGlyph => ImageHelper.GetImagePath(FolderPath + "/ls.tres");

	private string DPadNorth => ImageHelper.GetImagePath(FolderPath + "/up.tres");

	private string DPadSouth => ImageHelper.GetImagePath(FolderPath + "/down.tres");

	private string DPadWest => ImageHelper.GetImagePath(FolderPath + "/left.tres");

	private string DPadEast => ImageHelper.GetImagePath(FolderPath + "/right.tres");

	private string Ps4Touchpad => ImageHelper.GetImagePath(FolderPath + "/touchpad.tres");

	private Dictionary<string, string> GlyphMap
	{
		get
		{
			if (_glyphs == null)
			{
				_glyphs = new Dictionary<string, string>
				{
					{
						StringName.op_Implicit(Controller.faceButtonNorth),
						FaceButtonNorthGlyph
					},
					{
						StringName.op_Implicit(Controller.faceButtonSouth),
						FaceButtonSouthGlyph
					},
					{
						StringName.op_Implicit(Controller.faceButtonEast),
						FaceButtonEastGlyph
					},
					{
						StringName.op_Implicit(Controller.faceButtonWest),
						FaceButtonWestGlyph
					},
					{
						StringName.op_Implicit(Controller.leftTrigger),
						LeftTriggerGlyph
					},
					{
						StringName.op_Implicit(Controller.rightTrigger),
						RightTriggerGlyph
					},
					{
						StringName.op_Implicit(Controller.leftBumper),
						LeftBumperGlyph
					},
					{
						StringName.op_Implicit(Controller.rightBumper),
						RightBumperGlyph
					},
					{
						StringName.op_Implicit(Controller.selectButton),
						SelectButtonGlyph
					},
					{
						StringName.op_Implicit(Controller.startButton),
						StartButtonGlyph
					},
					{
						StringName.op_Implicit(Controller.joystickPress),
						JoystickPressGlyph
					},
					{
						StringName.op_Implicit(Controller.dPadNorth),
						DPadNorth
					},
					{
						StringName.op_Implicit(Controller.dPadSouth),
						DPadSouth
					},
					{
						StringName.op_Implicit(Controller.dPadEast),
						DPadEast
					},
					{
						StringName.op_Implicit(Controller.dPadWest),
						DPadWest
					},
					{
						StringName.op_Implicit(Controller.ps4Touchpad),
						Ps4Touchpad
					}
				};
			}
			return _glyphs;
		}
	}

	public IEnumerable<string> AssetPaths => GlyphMap.Values.Where((string path) => ResourceLoader.Exists(path, ""));

	public static IEnumerable<string> AllAssetPaths => new ControllerConfig[4]
	{
		new SteamControllerConfig(),
		new Ps4Config(),
		new Xbox360Config(),
		new XboxOneConfig()
	}.SelectMany((ControllerConfig c) => c.AssetPaths);

	public Texture2D? GetButtonIcon(string button)
	{
		if (GlyphMap.TryGetValue(button, out string value))
		{
			return ResourceLoader.Load<Texture2D>(value, (string)null, (CacheMode)1);
		}
		return null;
	}
}
