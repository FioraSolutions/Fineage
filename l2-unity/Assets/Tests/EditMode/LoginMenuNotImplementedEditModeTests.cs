using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class LoginMenuNotImplementedEditModeTests
{
    [Test]
    public void Production_team_logs_concrete_not_implemented_error()
    {
        LogAssert.Expect(LogType.Error, new Regex("NCLoginFunctionWnd/ProductionTeamButton"));
        InterludeLoginNotImplemented.Log("NCLoginFunctionWnd/ProductionTeamButton", "Interlude credits roll is not ported");
    }
}