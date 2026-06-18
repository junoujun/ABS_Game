// using UnityEngine;
// using System.Collections;
// using NUnit.Framework;

// public class Check_Location : MonoBehaviour
// {
//     [Header("target of Z")]
//     public float targetZ = 0f;

//     [Header("Strike Zone")]
//     public Collider strikeZoneCollider;

//     [Header("Ball")]
//     public float ballRadius;

//     private Vector3 previousPosition;

//     public Vector3 ballPositionAtTargetZ;
//     private bool targetZLogged = false;

//     public bool Is_Strike;

//     void Start()
//     {
//         ballRadius = GetBallWorldRadius();
//         previousPosition = transform.position;
//     }

//     void FixedUpdate()
//     {
//         CheckTargetZ();
//     }

//     private float GetBallWorldRadius()
//     {
//         Collider ballCollider = GetComponent<Collider>();
//         Bounds bounds = ballCollider.bounds;

//         return Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
//     }

//     private void CheckTargetZ()
//     {
//         Vector3 currentPosition = transform.position;

//         if (targetZLogged == false)
//         {
//             bool crossedTargetZ =
//                 (previousPosition.z <= targetZ && currentPosition.z >= targetZ) ||
//                 (previousPosition.z >= targetZ && currentPosition.z <= targetZ);

//             if (crossedTargetZ)
//             {
//                 float t = Mathf.InverseLerp(
//                     previousPosition.z,
//                     currentPosition.z,
//                     targetZ
//                 );

//                 ballPositionAtTargetZ = Vector3.Lerp(previousPosition, currentPosition, t);

//                 Debug.Log($"보간된 통과 좌표: {ballPositionAtTargetZ}");

//                 targetZLogged = true;

//                 CheckStrikeOrBall(ballPositionAtTargetZ);
//                 // DestroyGameObject();
//                 StartCoroutine(DestroyGameObject(1.0f));
//             }
//         }

//         previousPosition = currentPosition;
//     }

//     private void CheckStrikeOrBall(Vector3 ballPosition)
//     {
//         if (strikeZoneCollider == null)
//         {
//             Debug.LogWarning("Strike Zone Collider가 연결되지 않았습니다.");
//             return;
//         }

//         Bounds zoneBounds = strikeZoneCollider.bounds;

//         bool isInsideX =
//             ballPosition.x >= zoneBounds.min.x - ballRadius &&
//             ballPosition.x <= zoneBounds.max.x + ballRadius;

//         bool isInsideY =
//             ballPosition.y >= zoneBounds.min.y - ballRadius &&
//             ballPosition.y <= zoneBounds.max.y + ballRadius;

//         if (isInsideX && isInsideY)
//         {
//             Debug.Log("Strike!");
//             Is_Strike = true;
//         }
//         else
//         {
//             Debug.Log("Ball!");
//             Is_Strike = false;
//         }
//     }


//     IEnumerator DestroyGameObject(float waitTime)
//     {
//         yield return new WaitForSeconds(waitTime);
//         Destroy(gameObject);
//         Debug.Log("Game Object Destroyed");
//     }
// }

using System.Collections;
using UnityEngine;

public class Check_Location : MonoBehaviour
{
    [Header("target of Z")]
    public float targetZ = 0f;

    [Header("Strike Zone")]
    public Collider strikeZoneCollider;

    [Header("Ball")]
    public float ballRadius;

    [Header("Game Flow")]
    [SerializeField] private GamaManager gameManager;
    [SerializeField] private float hideDelay = 1.0f;

    private Vector3 previousPosition;

    public Vector3 ballPositionAtTargetZ;
    private bool targetZLogged = false;
    private bool isTracking = false;

    public bool Is_Strike;

    private Coroutine hideRoutine;

    private void Awake()
    {
        ballRadius = GetBallWorldRadius();
    }

    private void OnEnable()
    {
        ResetCheck();
    }

    private void FixedUpdate()
    {
        if (!isTracking)
            return;

        CheckTargetZ();
    }

    public void Initialize(Collider zoneCollider, GamaManager manager, float newTargetZ)
    {
        strikeZoneCollider = zoneCollider;
        gameManager = manager;
        targetZ = newTargetZ;

        ResetCheck();
    }

    public void ResetCheck()
    {
        targetZLogged = false;
        isTracking = false;
        Is_Strike = false;
        previousPosition = transform.position;

        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }
    }

    public void BeginTracking()
    {
        ballRadius = GetBallWorldRadius();
        previousPosition = transform.position;
        targetZLogged = false;
        isTracking = true;
    }

    private float GetBallWorldRadius()
    {
        Collider ballCollider = GetComponent<Collider>();

        if (ballCollider == null)
            return 0f;

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

                ballPositionAtTargetZ = Vector3.Lerp(previousPosition, currentPosition, t);

                Debug.Log($"보간된 통과 좌표: {ballPositionAtTargetZ}");

                targetZLogged = true;
                isTracking = false;

                bool result = CheckStrikeOrBall(ballPositionAtTargetZ);

                if (gameManager != null)
                    gameManager.ReceivePitchResult(result, ballPositionAtTargetZ);

                hideRoutine = StartCoroutine(HideGameObject(hideDelay));
            }
        }

        previousPosition = currentPosition;
    }

    private bool CheckStrikeOrBall(Vector3 ballPosition)
    {
        if (strikeZoneCollider == null)
        {
            Debug.LogWarning("Strike Zone Collider가 연결되지 않았습니다.");
            Is_Strike = false;
            return false;
        }

        Bounds zoneBounds = strikeZoneCollider.bounds;

        bool isInsideX =
            ballPosition.x >= zoneBounds.min.x - ballRadius &&
            ballPosition.x <= zoneBounds.max.x + ballRadius;

        bool isInsideY =
            ballPosition.y >= zoneBounds.min.y - ballRadius &&
            ballPosition.y <= zoneBounds.max.y + ballRadius;

        Is_Strike = isInsideX && isInsideY;

        Debug.Log(Is_Strike ? "Strike!" : "Ball!");
        return Is_Strike;
    }

    private IEnumerator HideGameObject(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        gameObject.SetActive(false);
        Debug.Log("Ball GameObject Hidden");
    }
}