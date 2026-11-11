using System;

public partial class SeedInfoWindow
{
    private void AddContent2Footer(ICreator creatorWindow, VisualElement footerContainer)
    {
        if (footerContainer != null)
        {
            if(_buttonAll1 == null)
            {
                _buttonAll1 = (Button)ToolTipsUtils.CloneOne(_buttonTemplate);
                _buttonAll1.Q<Label>("ButtonLabel").text = "Buy seeds";
            }



            if (_buttonAll2 == null)
            {
                _buttonAll2 = (Button)ToolTipsUtils.CloneOne(_buttonTemplate);
                _buttonAll2.Q<Label>("ButtonLabel").text = "Sale of harvest";
            }

            UnRegisterAllButtons();

            // Create a container for the buttons
            var buttonContainer = new VisualElement();
            buttonContainer.style.flexDirection = FlexDirection.Row;

            buttonContainer.Add(_buttonAll1);
            buttonContainer.Add(_buttonAll2);


            _buttonAll1.RegisterCallback<ClickEvent>(OnClickAllButton1);
            _buttonAll2.RegisterCallback<ClickEvent>(OnClickAllButton2);

            creatorWindow.InsertFooterIntoContent(buttonContainer, footerContainer);
        }
    }

    private List<TableColumn> GetColumnWindowTab0()
    {

        var harvest = new TableColumn(false, "Name Of The Seed", 0, new List<string>() {  }, 13 , 200);
        var auth = new TableColumn(true, "In Stock", 0, new List<string>() {  }, 0);
        var buy = new TableColumn(true, "For Sale", 0, new List<string>() { }, 0);
        var price = new TableColumn(true, "Price", 0, new List<string>() {  }, 0);

        return new List<TableColumn> { harvest, auth, buy, price};
    }

    private List<TableColumn> GetColumnWindowTab1()
    {

        var harvest = new TableColumn(false, "Name Of Fruits", 0, new List<string>() {  }, 13, 200);
        var auth = new TableColumn(true, "In Stock", 0, new List<string>() {  }, 0);
        var buy = new TableColumn(true, "For Sale", 0, new List<string>() {  }, 0);
        var price = new TableColumn(true, "Price", 0, new List<string>() {  }, 0);
        var reward = new TableColumn(true, "Reward", 0, new List<string>() {  }, 0);

        return new List<TableColumn> { harvest, auth, buy, price , reward };
    }

    private List<TableColumn> GetColumnWindowTab2()
    {

        var harvest = new TableColumn(false, "Name Of Fruits", 0, new List<string>() { }, 13, 200);
        var auth = new TableColumn(true, "Lvl", 0, new List<string>() {  }, 0);
        var buy = new TableColumn(true, "Price Seed", 0, new List<string>() {  }, 0);
        var price = new TableColumn(true, "Price Fruits", 0, new List<string>() {  }, 0);
        var reward1 = new TableColumn(true, "Reward1", 0, new List<string>() {  }, 0);
        var reward2 = new TableColumn(true, "Reward2", 0, new List<string>() {  }, 0);

        return new List<TableColumn> { harvest, auth, buy, price , reward1, reward2 };
    }
}
