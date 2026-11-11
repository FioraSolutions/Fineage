using System;
using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

public static partial class NetShortCutReceiver
{
    /// <summary>
    /// The wire carries a <b>world slot</b> (page * 12 + slot), matching Mobius.
    /// <see cref="Shortcut"/> wants slot and page split apart.
    /// </summary>
    const int SlotsPerPage = 12;

    /// <summary>Highest force charge Mobius reports; guards against a malformed count.</summary>
    const int MaxCharges = 7;

    /// <summary>Announced once, so a missing skillbar is a single log line rather than per-slot spam.</summary>
    static bool _warnedNoSkillbar;

    /// <summary>
    /// <c>ShortCutInit</c> / <c>ShortCutRegister</c> / <c>ShortCutDelete</c> can land before the
    /// skillbar is registered on the DI scope. Queue them so the player's hotbar is not
    /// silently empty on those loads; <see cref="FlushPending"/> is driven from the same
    /// per-frame pump as <see cref="NetGearSession.FlushPending"/>.
    /// </summary>
    static readonly List<Action> _pending = new List<Action>();

    /// <summary>Drop queued slots. Called when a new session starts.</summary>
}
