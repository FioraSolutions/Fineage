using VContainer;

public sealed partial class LoginRuntime
{
    private readonly IObjectResolver _resolver;
    private LoginClient _loginClient;
    private GameManager _manager;

    public LoginRuntime(IObjectResolver resolver)
    {
        _resolver = resolver;
    }

    [Inject]
    public void Construct(LoginClient loginClient, GameManager manager)
    {
        if (loginClient != null)
            _loginClient = loginClient;
        if (manager != null)
            _manager = manager;
    }

    public LoginClient LoginClient
    {
        get { return _loginClient ?? Get<LoginClient>() ?? NetworkingPacketServices.Login; }
    }

    public GameManager Manager
    {
        get { return _manager ?? Get<GameManager>() ?? NetworkingPacketServices.Manager; }
    }

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
