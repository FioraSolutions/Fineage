using System;

public partial class CharCreationWindow
{
    private void PreviousButtonPressed() {
        classManipulator.ClearInput();
        genderManipulator.ClearInput();
        hairstyleManipulator.ClearInput();
        hairColorManipulator.ClearInput();
        faceManipulator.ClearInput();
        raceManipulator.ClearInput();
        userInputField.value = "";
        SetlabelError("");
        ReleaseCreatePawns();
        Manager.OnAuthAllowed();
    }

    public void ReleaseCreatePawns()
    {
        if (Creator != null)
            Creator.DestroyAllPawns();
        isInit = false;
    }

    private void ShowRotatePawnWindow() {
        pawnRotateWindow.style.display = DisplayStyle.Flex;

        if (pawnZoominButton.ClassListContains("toggle")) {
            pawnZoominButton.RemoveFromClassList("toggle");
        }
    }

    private void HideRotatePawnWindow() {
        pawnRotateWindow.style.display = DisplayStyle.None;
    }

    private void ToggleZoomin(bool removeOnly) {
        if(pawnZoominButton.ClassListContains("toggle")) {
            LoginCamera.ZoomOut();

            pawnZoominButton.RemoveFromClassList("toggle");
        } else {

            LoginCamera.ZoomIn();

            pawnZoominButton.AddToClassList("toggle");
        }
    }

}
