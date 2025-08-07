using UnityEngine;
using DG.Tweening;

public class CameraSubtleRotation : MonoBehaviour
{
    public float angle = 5f;     // maximum rotation angle
    public float duration = 3f;  // duration of a full cycle

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.rotation;
        RotateCamera();
    }

    void RotateCamera()
    {
        // Target rotation: rotate by "angle" degrees around the Y axis from the initial rotation
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, angle, 0);

        // Tween that rotates back and forth with infinite yoyo loop
        transform.DORotateQuaternion(targetRotation, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
