namespace Fineage.Net.Contract
{
    /// <summary>
    /// Zone → owner: which pet/mount actions the Interlude client may show (summon window + action 38).
    /// </summary>
    public sealed class PlayerActionStateNetMsg
    {
        public bool PetOut { get; set; }
        public bool MountablePet { get; set; }
        public bool Mounted { get; set; }
        public bool CanRide { get; set; }
        public byte MountKind { get; set; }
        public int PetControlObjectId { get; set; }
    }
}