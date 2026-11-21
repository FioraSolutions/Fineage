/// <summary>Static reach-in for availability updates from net/UI seams (#142).</summary>
public static class ActionAvailabilityPacketServices
{
  static InterludeActionAvailabilityService _availability;

  public static InterludeActionAvailabilityService Availability =>
      _availability ?? InterludeActionAvailabilityService.Instance;

  public static void Bind(InterludeActionAvailabilityService availability = null)
  {
    if (availability != null)
    {
      _availability = availability;
      InterludeActionAvailabilityService.Bind(availability);
    }
  }

  public static void Unbind() => _availability = null;
}