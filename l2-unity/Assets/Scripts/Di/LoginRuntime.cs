using VContainer;

public sealed class LoginRuntime
{
    private readonly IObjectResolver _resolver;
    private GameClient _game;
    private LoginClient _loginClient;
    private GameManager _manager;

    public LoginRuntime(IObjectResolver resolver)
    {
        _resolver = resolver;
    }

    [Inject]
    public void Construct(GameClient game, LoginClient loginClient, GameManager manager)
    {
        if (game != null)
            _game = game;
        if (loginClient != null)
            _loginClient = loginClient;
        if (manager != null)
            _manager = manager;
        DiRuntimePacketServices.Bind(
            game: _game ?? Get<GameClient>(),
            login: _loginClient ?? Get<LoginClient>(),
            manager: _manager ?? Get<GameManager>());
    }

    public L2LoginUI Ui { get { return Get<L2LoginUI>() ?? L2LoginUI.Instance; } }
    public LoginWindow LoginWindow { get { return Get<LoginWindow>() ?? LoginWindow.Instance; } }
    public LicenseWindow License { get { return Get<LicenseWindow>() ?? LicenseWindow.Instance; } }
    public ServerSelectWindow Servers { get { return Get<ServerSelectWindow>() ?? ServerSelectWindow.Instance; } }
    public CharSelectWindow CharSelect { get { return Get<CharSelectWindow>() ?? CharSelectWindow.Instance; } }
    public CharCreationWindow CharCreate { get { return Get<CharCreationWindow>() ?? CharCreationWindow.Instance; } }
    public LoginCameraManager Camera { get { return Get<LoginCameraManager>() ?? LoginCameraManager.Instance; } }
    public CharacterSelector Selector { get { return Get<CharacterSelector>() ?? CharacterSelector.Instance; } }
    public CharacterCreator Creator { get { return Get<CharacterCreator>() ?? CharacterCreator.Instance; } }

    public GameClient Game => DiRuntimePacketServices.Game;

    public LoginClient LoginClient => DiRuntimePacketServices.Login;

    public GameManager Manager => DiRuntimePacketServices.Manager;

    private T Get<T>() where T : class
    {
        if (_resolver == null)
            return null;

        try
        {
            return _resolver.Resolve<T>();
        }
        catch
        {
            return null;
        }
    }
}
