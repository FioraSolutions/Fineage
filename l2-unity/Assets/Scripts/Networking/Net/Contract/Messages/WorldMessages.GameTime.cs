namespace Fineage.Net.Contract
{
    /// <summary>
    /// Zone → client: Mobius <c>ClientSetTime</c> (Interlude wire 0xEC).
    /// Layout: game minute-of-day (0–1439), then clock-speed constant (Mobius sends 6).
    /// </summary>
    public sealed class ClientSetTimeNetMsg
    {
        public int GameMinutes { get; set; }
        /// <summary>Mobius constant 6: one game minute every 10 real seconds (240 real minutes per game day).</summary>
        public int ClockSpeed { get; set; }
    }
}