using UnityEngine;
using System.Collections;

public partial class BoatSway : MonoBehaviour
{
    private void Start() {
        swayDirectionRightX = Random.Range(0f, 1f) > 0.5f;
        swayDirectionRightZ = Random.Range(0f, 1f) > 0.5f;
        swayAngleX = Random.Range(-swayRangeDegreesX, swayRangeDegreesX);
        swayAngleZ = Random.Range(-swayRangeDegreesZ, swayRangeDegreesZ);
        transform.eulerAngles = new Vector3(swayAngleX, transform.eulerAngles.y, swayAngleZ);
    }

    void Update() {
        if (!pauseSwayX) {
            if (swayAngleX > swayRangeDegreesX) {
                swayDirectionRightX = false;
                StartCoroutine(PauseSwayX());
            } else if (swayAngleX < -swayRangeDegreesX) {
                swayDirectionRightX = true;
                StartCoroutine(PauseSwayX());
            }

            if (swayDirectionRightX) {
                swayAngleX += Time.deltaTime * swaySpeedX;
            } else {
                swayAngleX -= Time.deltaTime * swaySpeedX;
            }
        }

        if (!pauseSwayZ) {
            if (swayAngleZ > swayRangeDegreesZ) {
                swayDirectionRightZ = false;
                StartCoroutine(PauseSwayZ());
            } else if (swayAngleZ < -swayRangeDegreesZ) {
                swayDirectionRightZ = true;
                StartCoroutine(PauseSwayZ());
            }

            if (swayDirectionRightZ) {
                swayAngleZ += Time.deltaTime * swaySpeedZ;
            } else {
                swayAngleZ -= Time.deltaTime * swaySpeedZ;
            }
        }

        transform.eulerAngles = new Vector3(swayAngleX, transform.eulerAngles.y, swayAngleZ);
    }
}
