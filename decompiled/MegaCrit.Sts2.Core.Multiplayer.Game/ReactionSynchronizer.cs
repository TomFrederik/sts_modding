using System;
using Godot;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game.PeerInput;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Game.Flavor;
using MegaCrit.Sts2.Core.Nodes.Reaction;

namespace MegaCrit.Sts2.Core.Multiplayer.Game;

public class ReactionSynchronizer : IDisposable
{
	private readonly NReactionContainer _container;

	public INetGameService NetService { get; }

	public ReactionSynchronizer(INetGameService netService, NReactionContainer container)
	{
		NetService = netService;
		_container = container;
		NetService.RegisterMessageHandler<ReactionMessage>(HandleReactionMessage);
	}

	public void Dispose()
	{
		NetService.UnregisterMessageHandler<ReactionMessage>(HandleReactionMessage);
	}

	public void SendLocalReaction(ReactionType type, Vector2 mouseScreenPos)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		ReactionMessage message = new ReactionMessage
		{
			type = type,
			normalizedPosition = NetCursorHelper.GetNormalizedPosition(mouseScreenPos, (Control)(object)_container)
		};
		NetService.SendMessage(message);
	}

	private void HandleReactionMessage(ReactionMessage message, ulong senderId)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		_container.DoRemoteReaction(message.type, NetCursorHelper.GetControlSpacePosition(message.normalizedPosition, (Control)(object)_container));
	}
}
