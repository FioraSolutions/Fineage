using System.Collections.Generic;

public partial class ShopItemEvaluator
{
    public void UpdateWeightBar(DealerWindow dealer, int currentWeight, int maxWeight)
    {
        // Dealer UXML paints fill on WeightGauge (GetWeiBarBg), matching prior behavior.
        float weightRatio = maxWeight > 0 ? (float)currentWeight / maxWeight : 0f;
        L2ResourceBarView.SetFillFixed(
            dealer.GetWeiBarBg(),
            dealer.GetWeiBarBg(),
            weightRatio,
            138f);
    }

    public void UpdateAllPrice(List<Product> _listSell)
    {
        _dealer.UpdatePrice(ShopCartTotals.SumUnitPriceTimesCount(_listSell));
    }

    public void UpdateWeight(List<Product> _listSell , int _currentDataWeight , int _maxWeight)
    {
        int addWeight = ShopCartTotals.SumStackWeight(_listSell);
        _dealer.UpdateAllWeight(_currentDataWeight + addWeight, _maxWeight);
    }
}
