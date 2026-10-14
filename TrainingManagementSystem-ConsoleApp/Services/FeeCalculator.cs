using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Services;

public readonly record struct FeeSummary(decimal BaseFee, decimal DiscountAmount, decimal FinalFee);

public sealed class FeeCalculator
{
    public FeeSummary CalculateFee(decimal baseFee, decimal sessionCharge)
    {
        ValidateAmounts(baseFee, sessionCharge, 0m, 0m);
        return new FeeSummary(baseFee, 0m, baseFee + sessionCharge);
    }

    public FeeSummary CalculateFee(decimal baseFee, decimal sessionCharge,
        bool qualifiesForEarlyBird, bool qualifiesForGroup)
    {
        ValidateAmounts(baseFee, sessionCharge, 0m, 0m);

        decimal earlyBirdDiscount = qualifiesForEarlyBird ? baseFee * 0.10m : 0m;
        decimal groupDiscount = qualifiesForGroup ? baseFee * 0.05m : 0m;
        decimal totalDiscount = earlyBirdDiscount + groupDiscount;
        decimal finalFee = baseFee - totalDiscount + sessionCharge;

        return new FeeSummary(baseFee, totalDiscount, finalFee);
    }

    public FeeSummary CalculateFee(decimal baseFee, decimal sessionCharge,
        decimal earlyBirdDiscount, decimal groupDiscount)
    {
        ValidateAmounts(baseFee, sessionCharge, earlyBirdDiscount, groupDiscount);
        decimal totalDiscount = earlyBirdDiscount + groupDiscount;
        return new FeeSummary(baseFee, totalDiscount, baseFee - totalDiscount + sessionCharge);
    }

    public static decimal GetFinalFee(in FeeSummary summary) => summary.FinalFee;

    private static void ValidateAmounts(decimal baseFee, decimal sessionCharge,
        decimal earlyBirdDiscount, decimal groupDiscount)
    {
        if (baseFee < 0 || sessionCharge < 0 || earlyBirdDiscount < 0 || groupDiscount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(baseFee), "Fees and discounts cannot be negative.");
        }

        if (earlyBirdDiscount + groupDiscount > baseFee)
        {
            throw new ArgumentException("Discounts cannot exceed the base fee.");
        }
    }
}
