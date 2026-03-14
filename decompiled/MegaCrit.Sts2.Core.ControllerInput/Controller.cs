using Godot;

namespace MegaCrit.Sts2.Core.ControllerInput;

public static class Controller
{
	public static readonly StringName leftTrigger = StringName.op_Implicit("controller_left_trigger");

	public static readonly StringName rightTrigger = StringName.op_Implicit("controller_right_trigger");

	public static readonly StringName leftBumper = StringName.op_Implicit("controller_left_bumper");

	public static readonly StringName rightBumper = StringName.op_Implicit("controller_right_bumper");

	public static readonly StringName dPadNorth = StringName.op_Implicit("controller_d_pad_north");

	public static readonly StringName dPadSouth = StringName.op_Implicit("controller_d_pad_south");

	public static readonly StringName dPadEast = StringName.op_Implicit("controller_d_pad_east");

	public static readonly StringName dPadWest = StringName.op_Implicit("controller_d_pad_west");

	public static readonly StringName faceButtonNorth = StringName.op_Implicit("controller_face_button_north");

	public static readonly StringName faceButtonSouth = StringName.op_Implicit("controller_face_button_south");

	public static readonly StringName faceButtonEast = StringName.op_Implicit("controller_face_button_east");

	public static readonly StringName faceButtonWest = StringName.op_Implicit("controller_face_button_west");

	public static readonly StringName startButton = StringName.op_Implicit("controller_start_button");

	public static readonly StringName selectButton = StringName.op_Implicit("controller_select_button");

	public static readonly StringName joystickPress = StringName.op_Implicit("controller_joystick_press");

	public static readonly StringName joystickLeft = StringName.op_Implicit("controller_joystick_left");

	public static readonly StringName joystickRight = StringName.op_Implicit("controller_joystick_right");

	public static readonly StringName joystickUp = StringName.op_Implicit("controller_joystick_up");

	public static readonly StringName joystickDown = StringName.op_Implicit("controller_joystick_down");

	public static readonly StringName ps4Touchpad = StringName.op_Implicit("ui_controller_touch_pad");

	public static StringName[] AllControllerInputs => (StringName[])(object)new StringName[20]
	{
		dPadEast, dPadNorth, dPadSouth, dPadWest, faceButtonEast, faceButtonNorth, faceButtonSouth, faceButtonWest, joystickDown, joystickLeft,
		joystickPress, joystickRight, joystickUp, leftBumper, leftTrigger, rightBumper, rightTrigger, selectButton, startButton, ps4Touchpad
	};
}
