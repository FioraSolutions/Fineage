public static class TutorialPacketServices
{
    private static TutorialQuestOverlay _overlay;

    public static void Bind(TutorialQuestOverlay overlay = null)
    {
        if (overlay != null)
            _overlay = overlay;
    }

    public static TutorialQuestOverlay Overlay => _overlay;
}