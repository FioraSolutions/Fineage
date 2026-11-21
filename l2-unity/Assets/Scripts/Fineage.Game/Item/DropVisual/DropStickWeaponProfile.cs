/// <summary>Stick-style ground drop mesh for weapon types (audit batch 9 boundary).</summary>
public static class DropStickWeaponProfile
{
    public static bool UsesStickDropModel(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.sword:
            case WeaponType.blunt:
            case WeaponType.dagger:
            case WeaponType.bow:
            case WeaponType.pole:
            case WeaponType.etc:
            case WeaponType.fist:
            case WeaponType.dual:
            case WeaponType.dualfist:
            case WeaponType.bigword:
            case WeaponType.staff:
            case WeaponType.bigblunt:
            case WeaponType.rod:
            case WeaponType.rake:
                return true;
            default:
                return false;
        }
    }
}