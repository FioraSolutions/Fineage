using Fineage.Net.Contract;

/// <summary>Hooks for life-stone augmentation / variation windows (Interlude refine UI).</summary>
public static class ItemAugmentPacketServices
{
    public static IAugmentMakeWindow Make { get; set; }
    public static IAugmentCancelWindow Cancel { get; set; }
}

/// <summary>Mobius variation make window — wired when Interlude refine UI lands.</summary>
public interface IAugmentMakeWindow
{
    void OpenFromZone();
    void OnPutTarget(VariationPutItemNetMsg msg);
    void OnPutRefiner(VariationPutRefinerNetMsg msg);
    void OnPutGemstone(VariationPutGemNetMsg msg);
    void OnVariationResult(VariationResultNetMsg msg);
}

/// <summary>Mobius variation cancel window — wired when Interlude remove-augment UI lands.</summary>
public interface IAugmentCancelWindow
{
    void OpenFromZone();
    void OnPutTarget(VariationCancelItemNetMsg msg);
    void OnCancelResult(VariationCancelResultNetMsg msg);
}