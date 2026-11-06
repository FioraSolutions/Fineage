using UnityEngine;

/// <summary>
/// Remnant of the Interlude login socket; kept for serialized account fields the login UI reads.
///
/// <para>
/// It used to be a <c>DefaultClient</c>: a Blowfish socket with a send queue, a receive thread,
/// and RSA/Blowfish key material. Fineage Net owns the login — the lobby token is the zone
/// password — so the socket never opened and the crypto fields were never read. Account and
/// password still come from this component's serialized values, which the login window reads.
/// </para>
/// </summary>
public class LoginClient : MonoBehaviour
{
    [Header("Account")]
    [SerializeField] protected string _account;
    [SerializeField] protected string _password;

    public string Account { get { return _account; } set { _account = value; } }
    public string Password { get { return _password; } set { _password = value; } }

    private static LoginClient _instance;

    public static LoginClient Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this);
    }
}
