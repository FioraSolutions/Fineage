using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Linq;
using UnityEngine;

public class ActionData 
{
    [SerializeField] public int _id;
    [SerializeField] public string _name;
    [SerializeField] public string _description;
    [SerializeField] public string _icon;
    [SerializeField] public int _category;
    [SerializeField] public int _type;
    [SerializeField] public int[] _category2;

    public int Id { get => _id; set => _id = value; }
    public string Name { get => _name; set => _name = value; }
    public string Descripion { get => _description; set => _description = value; }
    public string Icon { get => _icon; set => _icon = value; }
    public int Category { get => _category; set => _category = value; }
    public int Type { get => _type; set => _type = value; }
    public int[] Category2 { get => _category2; set => _category2 = value; }

    public string GetName()
    {
        return _name;
    }

    public string GetDescription()
    {
        return _description;
    }

    public int Level { get { return 0; } }
}
