using UnityEngine;

public class Check_Location : MonoBehaviour
{
    [Header("target of Z")]
    public float targetZ = 18.44f;

    [Header("Strike Zone")]
    public Collider strikeZoneCollider;

    [Header("Ball")]
    public float ballRadius = 0.03497678f * 2;

    private Vector3 previousPosition;
    private bool targetZLogged = false;

    void Start()
    {
        previousPosition = transform.position;
    }

    void FixedUpdate()
    {
        CheckTargetZ();
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
                DestroyGameObject();
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

    private void DestroyGameObject()
    {
        Destroy(gameObject);
        Debug.Log("Game Object Destroyed");
    }
}