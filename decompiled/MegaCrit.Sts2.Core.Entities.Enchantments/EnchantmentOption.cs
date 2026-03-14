using MegaCrit.Sts2.Core.Models;

namespace MegaCrit.Sts2.Core.Entities.Enchantments;

public struct EnchantmentOption(EnchantmentModel enchantment, int minAmount, int maxAmount)
{
	public readonly EnchantmentModel enchantment = enchantment;

	public readonly int minAmount = minAmount;

	public readonly int maxAmount = maxAmount;
}
