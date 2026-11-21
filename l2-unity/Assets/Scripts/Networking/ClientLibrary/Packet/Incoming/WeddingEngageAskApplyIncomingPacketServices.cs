using Fineage.Net.Contract;
using UnityEngine;

/// <summary>Wedding engage ask → generic yes/no confirm dialog (<see cref="SystemMessageWindow"/>).</summary>
public static class WeddingEngageAskApplyIncomingPacketServices
{
    public static void Apply(WeddingEngageAskNetMsg ask)
    {
        if (ask == null)
            return;

        QueueApplyIncomingPacketServices.Queue(() =>
        {
            SystemMessageWindow window = SystemMessagePacketServices.Window;
            if (window == null)
            {
                Debug.LogError("[WeddingEngage] SystemMessageWindow is not bound.");
                return;
            }

            string requestor = ask.RequestorName ?? "";
            window.ClearDialogHandlers();
            window.OnButtonOk += () => ZoneCommands.Send(new WeddingEngageAnswerCommand(1));
            window.OnButtonClosed += () => ZoneCommands.Send(new WeddingEngageAnswerCommand(0));
            window.ShowWindowDialogYesOrNot(
                string.IsNullOrEmpty(requestor)
                    ? "Do you want to get engaged?"
                    : requestor + " has requested engagement. Do you accept?");
        });
    }
}