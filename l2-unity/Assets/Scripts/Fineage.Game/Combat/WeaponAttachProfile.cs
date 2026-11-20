using System.Collections.Generic;

public sealed class WeaponAttachProfile
{
    public readonly List<WeaponAttachMount> Mounts = new List<WeaponAttachMount>();

    public void Add(WeaponAttachMount mount) => Mounts.Add(mount);
}