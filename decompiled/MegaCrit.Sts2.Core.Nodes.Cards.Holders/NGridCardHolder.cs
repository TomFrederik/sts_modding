using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Pooling;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Cards.Holders;

[ScriptPath("res://src/Core/Nodes/Cards/Holders/NGridCardHolder.cs")]
public class NGridCardHolder : NCardHolder, IPoolable
{
	public new class MethodName : NCardHolder.MethodName
	{
		public static readonly StringName InitPool = StringName.op_Implicit("InitPool");

		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName UpdateCardModel = StringName.op_Implicit("UpdateCardModel");

		public static readonly StringName OnInstantiated = StringName.op_Implicit("OnInstantiated");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName EnsureCardLibraryStatsExists = StringName.op_Implicit("EnsureCardLibraryStatsExists");

		public new static readonly StringName OnCardReassigned = StringName.op_Implicit("OnCardReassigned");

		public new static readonly StringName SetCard = StringName.op_Implicit("SetCard");

		public static readonly StringName UpdateName = StringName.op_Implicit("UpdateName");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public static readonly StringName SetIsPreviewingUpgrade = StringName.op_Implicit("SetIsPreviewingUpgrade");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName OnReturnedFromPool = StringName.op_Implicit("OnReturnedFromPool");

		public static readonly StringName OnFreedToPool = StringName.op_Implicit("OnFreedToPool");
	}

	public new class PropertyName : NCardHolder.PropertyName
	{
		public static readonly StringName CardLibraryStats = StringName.op_Implicit("CardLibraryStats");

		public new static readonly StringName IsShowingUpgradedCard = StringName.op_Implicit("IsShowingUpgradedCard");

		public static readonly StringName _isPreviewingUpgrade = StringName.op_Implicit("_isPreviewingUpgrade");
	}

	public new class SignalName : NCardHolder.SignalName
	{
	}

	private CardModel _baseCard;

	private CardModel? _upgradedCard;

	private bool _isPreviewingUpgrade;

	private static string ScenePath => SceneHelper.GetScenePath("cards/holders/grid_card_holder");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public NCardLibraryStats? CardLibraryStats { get; private set; }

	public override CardModel CardModel => _baseCard;

	public override bool IsShowingUpgradedCard
	{
		get
		{
			if (!_isPreviewingUpgrade)
			{
				return base.IsShowingUpgradedCard;
			}
			return true;
		}
	}

	public static void InitPool()
	{
		NodePool.Init<NGridCardHolder>(ScenePath, 30);
	}

	public static NGridCardHolder? Create(NCard cardNode)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NGridCardHolder nGridCardHolder = NodePool.Get<NGridCardHolder>();
		nGridCardHolder.SetCard(cardNode);
		nGridCardHolder.UpdateCardModel();
		nGridCardHolder.UpdateName();
		((Control)nGridCardHolder).Scale = nGridCardHolder.SmallScale;
		return nGridCardHolder;
	}

	private void UpdateCardModel()
	{
		CardModel cardModel = (_baseCard = base.CardNode.Model);
		if (cardModel.IsUpgradable)
		{
			_upgradedCard = (CardModel)cardModel.MutableClone();
			_upgradedCard.UpgradeInternal();
			if (((Node)this).IsNodeReady())
			{
				bool isPreviewingUpgrade = _isPreviewingUpgrade;
				_isPreviewingUpgrade = false;
				SetIsPreviewingUpgrade(isPreviewingUpgrade);
			}
		}
	}

	public void OnInstantiated()
	{
	}

	public override void _Ready()
	{
		bool isPreviewingUpgrade = _isPreviewingUpgrade;
		_isPreviewingUpgrade = false;
		SetIsPreviewingUpgrade(isPreviewingUpgrade);
		ConnectSignals();
	}

	public void EnsureCardLibraryStatsExists()
	{
		if (CardLibraryStats == null)
		{
			CardLibraryStats = NCardLibraryStats.Create();
			((Node)(object)this).AddChildSafely((Node?)(object)CardLibraryStats);
		}
	}

	protected override void OnCardReassigned()
	{
		UpdateCardModel();
		UpdateName();
	}

	protected override void SetCard(NCard node)
	{
		base.SetCard(node);
		if (CardLibraryStats != null)
		{
			((Node)this).MoveChild((Node)(object)CardLibraryStats, ((Node)this).GetChildCount(false) - 1);
		}
	}

	private void UpdateName()
	{
		((Node)this).Name = StringName.op_Implicit($"GridCardHolder-{base.CardNode.Model.Id}");
	}

	protected override void OnFocus()
	{
		base.OnFocus();
		((CanvasItem)this).MoveToFront();
	}

	public void SetIsPreviewingUpgrade(bool showUpgradePreview)
	{
		if (!((CanvasItem)this).Visible)
		{
			return;
		}
		if (!_baseCard.IsUpgradable && showUpgradePreview)
		{
			throw new InvalidExpressionException($"{_baseCard.Id} is not upgradable.");
		}
		if (_isPreviewingUpgrade != showUpgradePreview)
		{
			if (showUpgradePreview && _upgradedCard != null)
			{
				base.CardNode.Model = _upgradedCard;
				base.CardNode.ShowUpgradePreview();
			}
			else
			{
				base.CardNode.Model = _baseCard;
				base.CardNode.UpdateVisuals(base.CardNode.DisplayingPile, CardPreviewMode.Normal);
			}
			_isPreviewingUpgrade = showUpgradePreview;
		}
	}

	public override void _ExitTree()
	{
		if (((Node)this).IsAncestorOf((Node)(object)base.CardNode))
		{
			((Node)(object)base.CardNode)?.QueueFreeSafely();
		}
		base.CardNode = null;
	}

	public void OnReturnedFromPool()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (((Node)this).IsNodeReady())
		{
			((Control)this).Position = Vector2.Zero;
			((Control)this).Rotation = 0f;
			((Control)this).Scale = Vector2.One;
			((CanvasItem)this).Modulate = Colors.White;
			((CanvasItem)this).Visible = true;
			SetClickable(isClickable: true);
			((Control)base.Hitbox).MouseDefaultCursorShape = (CursorShape)0;
			_isPreviewingUpgrade = false;
			if (CardLibraryStats != null)
			{
				((CanvasItem)CardLibraryStats).Visible = false;
				((CanvasItem)CardLibraryStats).Modulate = Colors.White;
			}
		}
	}

	public void OnFreedToPool()
	{
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Expected O, but got Unknown
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(14);
		list.Add(new MethodInfo(MethodName.InitPool, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("cardNode"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateCardModel, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnInstantiated, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EnsureCardLibraryStatsExists, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCardReassigned, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetCard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("node"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateName, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetIsPreviewingUpgrade, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("showUpgradePreview"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnReturnedFromPool, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFreedToPool, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.InitPool && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitPool();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NGridCardHolder nGridCardHolder = Create(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NGridCardHolder>(ref nGridCardHolder);
			return true;
		}
		if ((ref method) == MethodName.UpdateCardModel && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateCardModel();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnInstantiated && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnInstantiated();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EnsureCardLibraryStatsExists && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EnsureCardLibraryStatsExists();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCardReassigned && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnCardReassigned();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetCard && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetCard(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateName && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateName();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetIsPreviewingUpgrade && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetIsPreviewingUpgrade(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnReturnedFromPool && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnReturnedFromPool();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFreedToPool && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFreedToPool();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.InitPool && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitPool();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NGridCardHolder nGridCardHolder = Create(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NGridCardHolder>(ref nGridCardHolder);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.InitPool)
		{
			return true;
		}
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateCardModel)
		{
			return true;
		}
		if ((ref method) == MethodName.OnInstantiated)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.EnsureCardLibraryStatsExists)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCardReassigned)
		{
			return true;
		}
		if ((ref method) == MethodName.SetCard)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateName)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.SetIsPreviewingUpgrade)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.OnReturnedFromPool)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFreedToPool)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.CardLibraryStats)
		{
			CardLibraryStats = VariantUtils.ConvertTo<NCardLibraryStats>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isPreviewingUpgrade)
		{
			_isPreviewingUpgrade = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
		if ((ref name) == PropertyName.CardLibraryStats)
		{
			NCardLibraryStats cardLibraryStats = CardLibraryStats;
			value = VariantUtils.CreateFrom<NCardLibraryStats>(ref cardLibraryStats);
			return true;
		}
		if ((ref name) == PropertyName.IsShowingUpgradedCard)
		{
			bool isShowingUpgradedCard = IsShowingUpgradedCard;
			value = VariantUtils.CreateFrom<bool>(ref isShowingUpgradedCard);
			return true;
		}
		if ((ref name) == PropertyName._isPreviewingUpgrade)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isPreviewingUpgrade);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.CardLibraryStats, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isPreviewingUpgrade, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsShowingUpgradedCard, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName cardLibraryStats = PropertyName.CardLibraryStats;
		NCardLibraryStats cardLibraryStats2 = CardLibraryStats;
		info.AddProperty(cardLibraryStats, Variant.From<NCardLibraryStats>(ref cardLibraryStats2));
		info.AddProperty(PropertyName._isPreviewingUpgrade, Variant.From<bool>(ref _isPreviewingUpgrade));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.CardLibraryStats, ref val))
		{
			CardLibraryStats = ((Variant)(ref val)).As<NCardLibraryStats>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._isPreviewingUpgrade, ref val2))
		{
			_isPreviewingUpgrade = ((Variant)(ref val2)).As<bool>();
		}
	}
}
