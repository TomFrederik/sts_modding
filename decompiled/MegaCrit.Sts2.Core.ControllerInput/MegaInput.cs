using Godot;

namespace MegaCrit.Sts2.Core.ControllerInput;

public static class MegaInput
{
	public static readonly StringName up = StringName.op_Implicit("ui_up");

	public static readonly StringName down = StringName.op_Implicit("ui_down");

	public static readonly StringName left = StringName.op_Implicit("ui_left");

	public static readonly StringName right = StringName.op_Implicit("ui_right");

	public static readonly StringName accept = StringName.op_Implicit("ui_accept");

	public static readonly StringName select = StringName.op_Implicit("ui_select");

	public static readonly StringName cancel = StringName.op_Implicit("ui_cancel");

	public static readonly StringName selectCard1 = StringName.op_Implicit("mega_select_card_1");

	public static readonly StringName selectCard2 = StringName.op_Implicit("mega_select_card_2");

	public static readonly StringName selectCard3 = StringName.op_Implicit("mega_select_card_3");

	public static readonly StringName selectCard4 = StringName.op_Implicit("mega_select_card_4");

	public static readonly StringName selectCard5 = StringName.op_Implicit("mega_select_card_5");

	public static readonly StringName selectCard6 = StringName.op_Implicit("mega_select_card_6");

	public static readonly StringName selectCard7 = StringName.op_Implicit("mega_select_card_7");

	public static readonly StringName selectCard8 = StringName.op_Implicit("mega_select_card_8");

	public static readonly StringName selectCard9 = StringName.op_Implicit("mega_select_card_9");

	public static readonly StringName selectCard10 = StringName.op_Implicit("mega_select_card_10");

	public static readonly StringName releaseCard = StringName.op_Implicit("mega_release_card");

	public static readonly StringName topPanel = StringName.op_Implicit("mega_top_panel");

	public static readonly StringName viewDrawPile = StringName.op_Implicit("mega_view_draw_pile");

	public static readonly StringName viewDiscardPile = StringName.op_Implicit("mega_view_discard_pile");

	public static readonly StringName viewDeckAndTabLeft = StringName.op_Implicit("mega_view_deck_and_tab_left");

	public static readonly StringName viewExhaustPileAndTabRight = StringName.op_Implicit("mega_view_exhaust_pile_and_tab_right");

	public static readonly StringName viewMap = StringName.op_Implicit("mega_view_map");

	public static readonly StringName pauseAndBack = StringName.op_Implicit("mega_pause_and_back");

	public static readonly StringName back = StringName.op_Implicit("mega_back");

	public static readonly StringName peek = StringName.op_Implicit("mega_peek");

	public static string[] AllInputs => new string[15]
	{
		StringName.op_Implicit(accept),
		StringName.op_Implicit(cancel),
		StringName.op_Implicit(down),
		StringName.op_Implicit(left),
		StringName.op_Implicit(pauseAndBack),
		StringName.op_Implicit(peek),
		StringName.op_Implicit(right),
		StringName.op_Implicit(select),
		StringName.op_Implicit(topPanel),
		StringName.op_Implicit(up),
		StringName.op_Implicit(viewDeckAndTabLeft),
		StringName.op_Implicit(viewDiscardPile),
		StringName.op_Implicit(viewDrawPile),
		StringName.op_Implicit(viewExhaustPileAndTabRight),
		StringName.op_Implicit(viewMap)
	};
}
