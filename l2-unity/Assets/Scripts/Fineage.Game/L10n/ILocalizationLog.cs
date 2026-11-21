public interface ILocalizationLog
{
    void Fallback(string key, string locale);
    void Missing(string key);
}