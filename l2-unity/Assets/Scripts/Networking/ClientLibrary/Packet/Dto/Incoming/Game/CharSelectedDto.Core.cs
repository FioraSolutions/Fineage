
using UnityEngine;

public partial class CharSelectedDto : IWireDto
{
    public CharSelectedDto()
    {
        _info = new PlayerInfoInterlude();
        _info.Identity = new EntityIdentity();
        _info.Status = new PlayerStatus();
        _info.Stats = new PlayerStats();
        _info.Appearance = new PlayerAppearance();
    }
}
