using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

/// <summary>
/// Body alpha for CharInfo users and city NPCs: CreateSkin fade-in, FNDisappearAction fade-out.
/// </summary>
public sealed partial class AppearFadeService : ITickable
{
    readonly L2ActorFade _fade;
    readonly Dictionary<int, Entry> _appear = new Dictionary<int, Entry>();
    readonly Dictionary<int, Entry> _disappear = new Dictionary<int, Entry>();
    readonly List<int> _remove = new List<int>();

}
