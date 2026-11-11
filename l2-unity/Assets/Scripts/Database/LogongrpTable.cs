using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using VContainer;

public partial class LogongrpTable
{
    private static LogongrpTable _instance;

    [System.Obsolete("Use [Inject] LogongrpTable or TablePacketServices.LogonGrps instead")]
    public static LogongrpTable Instance {
        get {
            if (_instance == null && App.HasContainer) {
                try {
                    _instance = App.Resolve<LogongrpTable>();
                } catch {
                }
            }

            if (_instance == null) {
                _instance = new LogongrpTable();
            }

            return _instance;
        }
    }


    public List<Logongrp> LogonGrps { get; private set; }


}
