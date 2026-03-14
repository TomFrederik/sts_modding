using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NMultiplayerVoteContainer.cs")]
public class NMultiplayerVoteContainer : Control
{
	public delegate bool PlayerVotedDelegate(Player player);

	private record VoteIcon
	{
		public required Player player;

		public required TextureRect node;

		public Tween? tween;

		[CompilerGenerated]
		[SetsRequiredMembers]
		protected VoteIcon(VoteIcon original)
		{
			player = original.player;
			node = original.node;
			tween = original.tween;
		}
	}

	public class MethodName : MethodName
	{
		public static readonly StringName RefreshPlayerVotes = StringName.op_Implicit("RefreshPlayerVotes");

		public static readonly StringName BouncePlayers = StringName.op_Implicit("BouncePlayers");
	}

	public class PropertyName : PropertyName
	{
	}

	public class SignalName : SignalName
	{
	}

	private const string _voteIconPath = "ui/multiplayer_vote_icon";

	private readonly List<VoteIcon> _votes = new List<VoteIcon>();

	private readonly List<VoteIcon> _iconsAnimatingOut = new List<VoteIcon>();

	private PlayerVotedDelegate _playerVotedDelegate;

	private readonly List<Player> _allPlayers = new List<Player>();

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(SceneHelper.GetScenePath("ui/multiplayer_vote_icon"));

	public IEnumerable<Player> Players => _votes.Select((VoteIcon v) => v.player);

	public void Initialize(PlayerVotedDelegate del, IReadOnlyList<Player> players)
	{
		_playerVotedDelegate = del;
		_allPlayers.AddRange(players);
	}

	public void RefreshPlayerVotes(bool animate = true)
	{
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		if (_allPlayers.Count == 1)
		{
			return;
		}
		for (int i = 0; i < _votes.Count; i++)
		{
			VoteIcon voteIcon = _votes[i];
			if (!_playerVotedDelegate(voteIcon.player))
			{
				AnimVoteOut(voteIcon, animate);
				_votes.RemoveAt(i);
				i--;
			}
		}
		foreach (Player player in _allPlayers)
		{
			if (_playerVotedDelegate(player))
			{
				int num = _votes.FindIndex((VoteIcon p) => p.player == player);
				if (num < 0)
				{
					VoteIcon voteIcon2 = new VoteIcon
					{
						player = player,
						node = SceneHelper.Instantiate<TextureRect>("ui/multiplayer_vote_icon")
					};
					voteIcon2.node.Texture = player.Character.IconTexture;
					((Node)voteIcon2.node).GetNode<TextureRect>(NodePath.op_Implicit("Outline")).Texture = player.Character.IconOutlineTexture;
					_votes.Add(voteIcon2);
					((Node)(object)this).AddChildSafely((Node?)(object)voteIcon2.node);
					((Control)voteIcon2.node).PivotOffset = ((Control)voteIcon2.node).Size * 0.5f;
					AnimVoteIn(voteIcon2, animate);
				}
			}
		}
	}

	public int GetVoteIndex(Player player)
	{
		return _votes.FindIndex((VoteIcon v) => v.player == player);
	}

	public void SetPlayerHighlighted(Player player, bool isHighlighted)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		VoteIcon voteIcon = _votes.FirstOrDefault((VoteIcon v) => v.player == player);
		if (voteIcon == null)
		{
			throw new InvalidOperationException();
		}
		voteIcon.tween?.FastForwardToCompletion();
		if (isHighlighted)
		{
			((Control)voteIcon.node).Scale = Vector2.One * 1.25f;
		}
		else
		{
			((Control)voteIcon.node).Scale = Vector2.One;
		}
	}

	public void BouncePlayers()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		foreach (VoteIcon vote in _votes)
		{
			vote.tween?.FastForwardToCompletion();
			vote.tween = ((Node)this).CreateTween().SetParallel(true);
			vote.tween.TweenProperty((GodotObject)(object)vote.node, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1.3f), 0.15).SetEase((EaseType)1).SetTrans((TransitionType)4);
			vote.tween.TweenProperty((GodotObject)(object)vote.node, NodePath.op_Implicit("position:y"), Variant.op_Implicit(-10f), 0.15).SetEase((EaseType)1).SetTrans((TransitionType)4);
			vote.tween.Chain().TweenProperty((GodotObject)(object)vote.node, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.2).SetEase((EaseType)0)
				.SetTrans((TransitionType)4);
			vote.tween.TweenProperty((GodotObject)(object)vote.node, NodePath.op_Implicit("position:y"), Variant.op_Implicit(0f), 0.3).SetEase((EaseType)0).SetTrans((TransitionType)4);
		}
	}

	private void AnimVoteIn(VoteIcon vote, bool animate)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		int num = _iconsAnimatingOut.FindIndex((VoteIcon i) => i.player == vote.player);
		if (num > 0)
		{
			Tween? tween = _iconsAnimatingOut[num].tween;
			if (tween != null)
			{
				tween.Kill();
			}
			((Node)(object)_iconsAnimatingOut[num].node).QueueFreeSafely();
			_iconsAnimatingOut.RemoveAt(num);
		}
		if (animate)
		{
			Tween? tween2 = vote.tween;
			if (tween2 != null)
			{
				tween2.Kill();
			}
			vote.tween = ((Node)this).CreateTween().SetParallel(true);
			vote.tween.TweenProperty((GodotObject)(object)vote.node, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.2).From(Variant.op_Implicit(0f));
			vote.tween.TweenProperty((GodotObject)(object)vote.node, NodePath.op_Implicit("position:y"), Variant.op_Implicit(0f), 0.3).From(Variant.op_Implicit(20f)).SetTrans((TransitionType)10)
				.SetEase((EaseType)1);
		}
	}

	private void AnimVoteOut(VoteIcon vote, bool animate)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		_iconsAnimatingOut.Add(vote);
		if (animate)
		{
			Tween? tween = vote.tween;
			if (tween != null)
			{
				tween.Kill();
			}
			vote.tween = ((Node)this).CreateTween().SetParallel(true);
			vote.tween.TweenProperty((GodotObject)(object)vote.node, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.1).SetDelay(0.15000000596046448);
			vote.tween.TweenProperty((GodotObject)(object)vote.node, NodePath.op_Implicit("position:y"), Variant.op_Implicit(20f), 0.25).SetTrans((TransitionType)5).SetEase((EaseType)0);
			vote.tween.Chain().TweenCallback(Callable.From((Action)delegate
			{
				RemoveVoteAfterAnimation(vote);
			}));
		}
		else
		{
			RemoveVoteAfterAnimation(vote);
		}
	}

	private void RemoveVoteAfterAnimation(VoteIcon vote)
	{
		_iconsAnimatingOut.Remove(vote);
		((Node)(object)vote.node).QueueFreeSafely();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName.RefreshPlayerVotes, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("animate"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.BouncePlayers, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.RefreshPlayerVotes && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RefreshPlayerVotes(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.BouncePlayers && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			BouncePlayers();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.RefreshPlayerVotes)
		{
			return true;
		}
		if ((ref method) == MethodName.BouncePlayers)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).SaveGodotObjectData(info);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
	}
}
