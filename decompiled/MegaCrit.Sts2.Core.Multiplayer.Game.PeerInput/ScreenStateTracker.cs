using System;
using Godot;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Multiplayer.Game.PeerInput;

public class ScreenStateTracker
{
	private NetScreenType _capstoneScreen;

	private NetScreenType _overlayScreen;

	private bool _mapScreenVisible;

	private bool _isInSharedRelicPicking;

	private NRewardsScreen? _connectedRewardsScreen;

	public ScreenStateTracker(NMapScreen mapScreen, NCapstoneContainer capstoneContainer, NOverlayStack overlayStack)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)capstoneContainer).Connect(NCapstoneContainer.SignalName.Changed, Callable.From((Action)OnCapstoneScreenChanged), 0u);
		((GodotObject)overlayStack).Connect(NOverlayStack.SignalName.Changed, Callable.From((Action)OnOverlayStackChanged), 0u);
		((GodotObject)mapScreen).Connect(SignalName.VisibilityChanged, Callable.From((Action)OnMapScreenVisibilityChanged), 0u);
	}

	private void OnCapstoneScreenChanged()
	{
		if (!RunManager.Instance.IsSinglePlayerOrFakeMultiplayer)
		{
			_capstoneScreen = NCapstoneContainer.Instance.CurrentCapstoneScreen?.ScreenType ?? NetScreenType.None;
			SyncLocalScreen();
		}
	}

	private void OnOverlayStackChanged()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (RunManager.Instance.IsSinglePlayerOrFakeMultiplayer)
		{
			return;
		}
		IOverlayScreen overlayScreen = NOverlayStack.Instance.Peek();
		if (overlayScreen is NRewardsScreen nRewardsScreen)
		{
			if (_connectedRewardsScreen != nRewardsScreen)
			{
				_connectedRewardsScreen = nRewardsScreen;
				((GodotObject)nRewardsScreen).Connect(NRewardsScreen.SignalName.Completed, Callable.From((Action)SyncLocalScreen), 0u);
			}
		}
		else
		{
			_connectedRewardsScreen = null;
		}
		_overlayScreen = overlayScreen?.ScreenType ?? NetScreenType.None;
		SyncLocalScreen();
	}

	private void SyncLocalScreen()
	{
		RunManager.Instance.InputSynchronizer.SyncLocalScreen(GetCurrentScreen());
	}

	private void OnMapScreenVisibilityChanged()
	{
		_mapScreenVisible = ((CanvasItem)NMapScreen.Instance).Visible;
		RunManager.Instance.InputSynchronizer.SyncLocalScreen(GetCurrentScreen());
	}

	public void SetIsInSharedRelicPickingScreen(bool isInSharedRelicPicking)
	{
		_isInSharedRelicPicking = isInSharedRelicPicking;
		RunManager.Instance.InputSynchronizer.SyncLocalScreen(GetCurrentScreen());
	}

	private NetScreenType GetCurrentScreen()
	{
		if (_capstoneScreen != NetScreenType.None)
		{
			return _capstoneScreen;
		}
		if (_mapScreenVisible)
		{
			return NetScreenType.Map;
		}
		if (_overlayScreen == NetScreenType.Rewards)
		{
			if (NOverlayStack.Instance.Peek() is NRewardsScreen { IsComplete: false })
			{
				return _overlayScreen;
			}
		}
		else if (_overlayScreen != NetScreenType.None)
		{
			return _overlayScreen;
		}
		if (_isInSharedRelicPicking)
		{
			return NetScreenType.SharedRelicPicking;
		}
		return NetScreenType.Room;
	}
}
