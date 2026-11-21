using NUnit.Framework;
using UnityEngine;

public sealed class ReleaseLogPolicyEditModeTests
{
    [Test]
    public void Release_policy_disables_log_and_warning_stack_traces()
    {
        Assert.AreEqual(StackTraceLogType.None, FineageReleaseLogPolicy.ReleaseLogStackTrace);
        Assert.AreEqual(StackTraceLogType.None, FineageReleaseLogPolicy.ReleaseWarningStackTrace);
    }
}