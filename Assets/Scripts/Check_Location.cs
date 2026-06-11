using UnityEngine;
using System.Collections;

public class Check_Location : MonoBehaviour
{
    [Header("target of Z")]
    public float targetZ = 0f;

    [Header("Strike Zone")]
    public Collider strikeZoneCollider;

    [Header("Ball")]
    public float ballRadius;

    private Vector3 previousPosition;
    private bool targetZLogged = false;

    void Start()
    {
        ballRadius = GetBallWorldRadius();
        previousPosition = transform.position;
    }

    void FixedUpdate()
    {
        CheckTargetZ();
    }

    private float GetBallWorldRadius()
    {
        Collider ballCollider = GetComponent<Collider>();
        Bounds bounds = ballCollider.bounds;

        return Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
    }

    private void CheckTargetZ()
    {
        Vector3 currentPosition = transform.position;

        if (targetZLogged == false)
        {
            bool crossedTargetZ =
                (previousPosition.z <= targetZ && currentPosition.z >= targetZ) ||
                (previousPosition.z >= targetZ && currentPosition.z <= targetZ);

            if (crossedTargetZ)
            {
                float t = Mathf.InverseLerp(
                    previousPosition.z,
                    currentPosition.z,
                    targetZ
                );

                Vector3 ballPositionAtTargetZ =
                    Vector3.Lerp(previousPosition, currentPosition, t);

                Debug.Log($"보간된 통과 좌표: {ballPositionAtTargetZ}");

                targetZLogged = true;

                CheckStrikeOrBall(ballPositionAtTargetZ);
                //DestroyGameObject();
                StartCoroutine(DestroyGameObject(1.0f));
            }
        }

        previousPosition = currentPosition;
    }

    private void CheckStrikeOrBall(Vector3 ballPosition)
    {
        if (strikeZoneCollider == null)
        {
            Debug.LogWarning("Strike Zone Collider가 연결되지 않았습니다.");
            return;
        }

        Bounds zoneBounds = strikeZoneCollider.bounds;

        bool isInsideX =
            ballPosition.x >= zoneBounds.min.x - ballRadius &&
            ballPosition.x <= zoneBounds.max.x + ballRadius;

        bool isInsideY =
            ballPosition.y >= zoneBounds.min.y - ballRadius &&
            ballPosition.y <= zoneBounds.max.y + ballRadius;

        if (isInsideX && isInsideY)
        {
            Debug.Log("Strike!");
        }
        else
        {
            Debug.Log("Ball!");
        }
    }

    IEnumerator DestroyGameObject(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
        Debug.Log("Game Object Destroyed");
    }
}