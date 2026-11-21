using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class MetaTableBootstrapLogEditModeTests
{
    [Test]
    public void LogMissingDatFile_EmitsError()
    {
        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("MetaTable TestTable: missing file"));
        MetaTableBootstrapLog.LogMissingDatFile("TestTable", "/tmp/missing_weapongrp.txt");
    }
}