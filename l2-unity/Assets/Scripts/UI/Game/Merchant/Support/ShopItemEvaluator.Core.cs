
using System.Collections.Generic;

public partial class ShopItemEvaluator
{
    public ShopItemEvaluator(DealerWindow dealer)
    {
        this._dealer = dealer;
    }

    public void UpdateDataForm(int adena, double weightPercent, int currentWeight, int maxWeight)
    {
        _dealer.UpdateDataWeight(currentWeight);
        _dealer.UpdateDataMaxWeight(maxWeight);
        _dealer.UpdateAdena(adena);
        _dealer.UpdateWeight(weightPercent);

        UpdateWeightBar(_dealer, currentWeight, maxWeight);
    }
}
