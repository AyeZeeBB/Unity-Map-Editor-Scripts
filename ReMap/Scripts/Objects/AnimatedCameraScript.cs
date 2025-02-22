using UnityEngine;

[AddComponentMenu("ReMap/Animated Camera", 0)]
public class AnimatedCameraScript : MonoBehaviour
{
    public Transform CameraBase;
    public Transform Camera;
    public float AngleOffset = 20f;
    public float MaxLeft = 20f;
    public float MaxRight = 40f;
    public float RotationTime = 4f;
    public float TransitionTime = 2f;

    void OnDrawGizmos()
    {
        if (CameraBase && Camera != null)
        {
            Camera.localPosition = new Vector3(0, 8, -16);
            Camera.localEulerAngles = new Vector3(-AngleOffset, 0, 0);
        }
    }
}
