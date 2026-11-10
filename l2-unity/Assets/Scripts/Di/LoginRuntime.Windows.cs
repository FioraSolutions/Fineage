public sealed partial class LoginRuntime
{
#pragma warning disable CS0618 // Locator fallback until LoginLifetime fully owns login UI
    public L2LoginUI Ui { get { return Get<L2LoginUI>() ?? L2LoginUI.Instance; } }
    public LoginWindow LoginWindow { get { return Get<LoginWindow>() ?? LoginWindow.Instance; } }
    public LicenseWindow License { get { return Get<LicenseWindow>() ?? LicenseWindow.Instance; } }
    public ServerSelectWindow Servers { get { return Get<ServerSelectWindow>() ?? ServerSelectWindow.Instance; } }
    public CharSelectWindow CharSelect { get { return Get<CharSelectWindow>() ?? CharSelectWindow.Instance; } }
#pragma warning restore CS0618
#pragma warning disable CS0618 // Locator fallback until LoginLifetime fully owns CharCreationWindow
    public CharCreationWindow CharCreate { get { return Get<CharCreationWindow>() ?? CharCreationWindow.Instance; } }
#pragma warning restore CS0618
    public LoginCameraManager Camera { get { return Get<LoginCameraManager>() ?? LoginCameraManager.Instance; } }
    public CharacterSelector Selector { get { return Get<CharacterSelector>() ?? CharacterSelector.Instance; } }
    public CharacterCreator Creator { get { return Get<CharacterCreator>() ?? CharacterCreator.Instance; } }
}
