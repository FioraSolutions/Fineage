using UnityEngine;
using System.Collections;

public partial class BoatSway : MonoBehaviour
{
    IEnumerator PauseSwayX() {
        pauseSwayX = true;
        yield return new WaitForSeconds(pauseSwayXDuration);
        pauseSwayX = false;
    }

    IEnumerator PauseSwayZ() {
        pauseSwayZ = true;
        yield return new WaitForSeconds(pauseSwayZDuration);
        pauseSwayZ = false;
    }
}
