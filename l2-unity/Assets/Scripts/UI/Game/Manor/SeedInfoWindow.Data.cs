using System;

public partial class SeedInfoWindow
{
    public void SetDataSeedInfo(List<SeedProduction> listSeedProduction)
    {
        List<string> harvest = new List<string>();
        List<string> auth = new List<string>();
        List<string> buy = new List<string>();
        List<string> price = new List<string>();


        foreach (var seedProduction in listSeedProduction)
        {
            harvest.Add(""+seedProduction.GetSeedId());
            auth.Add("" + seedProduction.GetAmount());
            buy.Add("" + seedProduction.GetStartAmount());
            price.Add("" + seedProduction.GetPrice());
        }

        _defaultColumns0[0].SetData(harvest);
        _defaultColumns0[1].SetData(auth);
        _defaultColumns0[2].SetData(buy);
        _defaultColumns0[3].SetData(price);

        _creatorTable.UpdateTableData(_defaultColumns0);

    }

    public void SetDataCropInfo(List<CropProcure> listCropInfo)
    {
        List<string> harvest = new List<string>();
        List<string> auth = new List<string>();
        List<string> buy = new List<string>();
        List<string> price = new List<string>();
        List<string> reward = new List<string>();

        foreach (var seedProduction in listCropInfo)
        {
            harvest.Add("" + seedProduction.GetSeedId());
            auth.Add("" + seedProduction.GetAmount());
            buy.Add("" + seedProduction.GetStartAmount());
            price.Add("" + seedProduction.GetPrice());
            reward.Add("" + seedProduction.GetReward());
        }

        _defaultColumns1[0].SetData(harvest);
        _defaultColumns1[1].SetData(auth);
        _defaultColumns1[2].SetData(buy);
        _defaultColumns1[3].SetData(price);
        _defaultColumns1[4].SetData(reward);

        _creatorTable.UpdateTableData(_defaultColumns1);

    }

    public void SetDataDefaultManorInfo(List<Seed> listSeed)
    {
        List<string> harvest = new List<string>();
        List<string> auth = new List<string>();
        List<string> buy = new List<string>();
        List<string> price = new List<string>();
        List<string> reward1 = new List<string>();
        List<string> reward2 = new List<string>();

        foreach (var seed in listSeed)
        {
            ItemName name = ItemNames.GetItemName(seed.CropId);

            if(name != null)
            {
                ItemName reward1name = ItemNames.GetItemName(seed.Reward1ItemId);
                ItemName reward2name = ItemNames.GetItemName(seed.Reward2ItemId);
                harvest.Add("" + name.Name);
                auth.Add("" + seed.SeedLevel);
                buy.Add("" + seed.SeedReferencePrice);
                price.Add("" + seed.CropReferencePrice);
                reward1.Add("" + reward1name.Name);
                reward2.Add("" + reward2name.Name);
            }
        }

        _defaultColumns2[0].SetData(harvest);
        _defaultColumns2[1].SetData(auth);
        _defaultColumns2[2].SetData(buy);
        _defaultColumns2[3].SetData(price);
        _defaultColumns2[4].SetData(reward1);
        _defaultColumns2[5].SetData(reward2);

        _creatorTable.UpdateTableData(_defaultColumns2);
    }

}
