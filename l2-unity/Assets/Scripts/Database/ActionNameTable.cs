using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using VContainer;

public class ActionNameTable
{
    private static ActionNameTable _instance;

    [Obsolete("Use [Inject] ActionNameTable or TablePacketServices.ActionNames instead")]
    public static ActionNameTable Instance
    {
        get
        {
            if (_instance == null && App.HasContainer)
            {
                try
                {
                    _instance = App.Resolve<ActionNameTable>();
                }
                catch
                {
                }
            }

            if (_instance == null)
            {
                _instance = new ActionNameTable();
            }

            return _instance;
        }
    }

    public static void Bind(ActionNameTable table)
    {
        if (table != null)
            _instance = table;
    }

    public static void Unbind() => _instance = null;

    private Dictionary<int, ActionData> _actions;


    public ActionData GetAction(ActionType type)
    {
        return _actions.ContainsKey((int)type) ? _actions[(int)type] : null;
    }

    public Dictionary<int, ActionData> Actions { get { return _actions; } }

    public void Initialize()
    {
        ReadActions();
    
    }

    private void ReadActions()
    {
        _actions = new Dictionary<int, ActionData>();
        if (!InterludeActionNameFile.TryLoad(out InterludeActionNameFile.Row[] rows))
            return;

        for (int i = 0; i < rows.Length; i++)
        {
            InterludeActionNameFile.Row row = rows[i];
            if (_actions.ContainsKey(row.Id))
                continue;

            var actionData = new ActionData
            {
                Id = row.Id,
                Name = row.Cmd,
                Icon = row.IconStem,
                Descripion = row.Name,
                Category = row.Category,
                Type = row.Type,
                Category2 = row.Category2
            };
            _actions.Add(row.Id, actionData);
        }
    }

}

