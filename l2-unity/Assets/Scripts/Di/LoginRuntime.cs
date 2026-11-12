using UnityEngine;
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
        get
        {
            if (_loginClient != null)
                return _loginClient;
            _loginClient = Get<LoginClient>();
            return _loginClient;
        }
    }

    public GameManager Manager
    {
        get
        {
            if (_manager != null)
                return _manager;
            _manager = Get<GameManager>();
            return _manager;
        }
    }

    private T Get<T>() where T : class
    {
        if (_resolver == null)
            return null;

        try
        {
            return _resolver.Resolve<T>();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[LoginRuntime] Resolve<" + typeof(T).Name + "> failed: " + ex.Message);
            return null;
        }
    }
}
