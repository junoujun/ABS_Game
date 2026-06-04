using UnityEngine;

public class Check_Location : MonoBehaviour
{
    [Header("target of Z")] // strike zone을 지나는 순간 공의 좌표를 구하기 위함
    public float targetZ = 18.44f; // strike zone의 z좌표

    [Header("Strike Zone")]
    public Collider strikeZoneCollider;
    private float previousZ; // 특정 순간에 ball의 z좌표를 담아놓을 변수
    private bool targetZLogged = false; // debug.log를 입력 했는지 확인하는 변수
    void Start()
    {
        previousZ = transform.position.z;
    }

    void FixedUpdate()
    {
        CheckTargetZ();
    }

    private void CheckTargetZ() // fixed update에 참조 됨
    {
        float currentZ = transform.position.z; //현재 position의 z값을 구함

        if (targetZLogged == false) // Debug.log를 하지 않았다면
        {
            bool crossedTargetZ = //이전 프레임에서 현재 프레임으로 올 때 원하는 지점을 지나쳤는가?
                (previousZ <= targetZ && currentZ >= targetZ) || 
                (previousZ >= targetZ && currentZ <= targetZ);

            if (crossedTargetZ)
            {
                Vector3 ballPosition = transform.position;

                Debug.Log($"현재 좌표: {ballPosition}");
                targetZLogged = true;

                CheckStrikeOrBall(ballPosition);
            }
        }
        previousZ = currentZ; // 다음 실행에서 현재 z좌표를 이전지점 좌표로 사용하기 위함
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
            ballPosition.x >= zoneBounds.min.x &&
            ballPosition.x <= zoneBounds.max.x;

        bool isInsideY =
            ballPosition.y >= zoneBounds.min.y &&
            ballPosition.y <= zoneBounds.max.y;

        if (isInsideX && isInsideY)
        {
            Debug.Log("Strike!");
        }
        else
        {
            Debug.Log("Ball!");
        }
    }

}
