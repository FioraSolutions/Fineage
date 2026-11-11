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
}
