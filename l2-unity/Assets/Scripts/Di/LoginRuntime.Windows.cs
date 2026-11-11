using UnityEngine;
using VContainer;

public sealed partial class LoginRuntime
{
    // Resolver-only. Unbound/legacy callers go through LoginPacketServices Instance fallbacks.
    public L2LoginUI Ui => Get<L2LoginUI>();
    public LoginWindow LoginWindow => Get<LoginWindow>();
    public LicenseWindow License => Get<LicenseWindow>();
    public ServerSelectWindow Servers => Get<ServerSelectWindow>();
    public CharSelectWindow CharSelect => Get<CharSelectWindow>();
    public CharCreationWindow CharCreate => Get<CharCreationWindow>();
    public LoginCameraManager Camera => Get<LoginCameraManager>();
    public CharacterSelector Selector => Get<CharacterSelector>();
    public CharacterCreator Creator => Get<CharacterCreator>();
}
