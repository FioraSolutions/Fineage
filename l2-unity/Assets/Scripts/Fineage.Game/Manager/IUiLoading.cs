/// <summary>
/// Loading overlay that <c>GameManager</c> shows (#131 S33).
/// <c>L2LoginUI</c> and <c>L2GameUI</c> implement it through <c>L2UI</c>.
/// </summary>
public interface IUiLoading
{
    void StartLoading();
}
