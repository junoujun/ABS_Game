using UnityEngine;

public class SlowMotionDebug : MonoBehaviour
{
    [Range(0.05f, 1f)]
    public float timeScale = 0.2f;

    private void Start()
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}