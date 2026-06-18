// using System.Collections;
// using UnityEngine;

// public class Baseball : MonoBehaviour
// {
//     public enum PitchType
//     {
//         Fastball,
//         Slider,
//         Curve,
//     }

//     private Rigidbody rb;

//     [Header("Pitch Type")]
//     public PitchType pitchType;

//     [Header("Random Setting")]
//     public bool randomPitchOnStart = true;

//     [Header("Throw Settings")]
//     public float throwSpeed = 20f;

//     [Header("Spin Settings")]
//     public Vector3 spinAxis = Vector3.right;
//     public float spinSpeed = 0f;

//     [Header("Magnus Effect")]
//     public float magnusStrength = 0.05f;

//     private Vector3 newPosition = new Vector3();

//     private bool isThrown = false;
//     private bool isPreparingThrow = false;

//     private void Start()
//     {
//         rb = GetComponent<Rigidbody>();

//         rb.isKinematic = false; // 정지를 위한 Kinemetic False

//         // 시작하자마자 움직이지 않도록 정지
//         rb.linearVelocity = Vector3.zero;
//         rb.angularVelocity = Vector3.zero;

//         // 버튼 누르기 전까지 물리 영향 방지
//         rb.isKinematic = true;
//     }

//     private void FixedUpdate()
//     {
//         // 던진 이후에만 마그누스 효과 적용
//         if (isThrown)
//         {
//             ApplyMagnusEffect(pitchType);
//         }
//     }

//     // UI 버튼에서 이 함수를 연결하면 됨
//     public void ThrowAfterOneSecond()
//     {
//         if (isPreparingThrow || isThrown)
//             return;

//         StartCoroutine(ThrowRoutine());
//     }

//     private IEnumerator ThrowRoutine()
//     {
//         isPreparingThrow = true;

//         yield return new WaitForSeconds(1.0f);

//         ThrowBall();

//         isPreparingThrow = false;
//     }

//     private void ThrowBall()
//     {
//         // 물리 활성화
//         rb.isKinematic = false;

//         // 이전 속도 초기화
//         rb.linearVelocity = Vector3.zero;
//         rb.angularVelocity = Vector3.zero;

//         if (randomPitchOnStart)
//         {
//             SelectRandomPitch();
//         }

//         ApplyPitchSetting();

//         Vector3 targetPosition = Vector3.zero;

//         switch (pitchType)
//         {
//             case PitchType.Fastball:
//                 targetPosition = new Vector3(
//                     Random.Range(-0.5f, 0.5f),
//                     Random.Range(0.3f, 1f),
//                     0f
//                 );
//                 break;

//             case PitchType.Curve:
//                 targetPosition = new Vector3(
//                     Random.Range(-0.5f, 0.5f),
//                     Random.Range(0.3f, 1f),
//                     0f
//                 );
//                 break;

//             case PitchType.Slider:
//                 targetPosition = new Vector3(
//                     Random.Range(-1.4f, -1.1f),
//                     Random.Range(0.3f, 1f),
//                     0f
//                 );
//                 break;
//         }

//         Debug.Log(
//             $"Pitch Type: {pitchType}\n" +
//             $"Start Location: {newPosition.ToString("F3")}\n" +
//             $"Target Position: {targetPosition.ToString("F3")}\n" +
//             $"Transform Position: {transform.position.ToString("F3")}"
//         );

//         Vector3 throwDirection = (targetPosition - rb.position).normalized;

//         rb.linearVelocity = throwDirection * throwSpeed;
//         rb.angularVelocity = spinAxis.normalized * spinSpeed;

//         isThrown = true;
//     }

//     private void SelectRandomPitch()
//     {
//         int pitchCount = System.Enum.GetValues(typeof(PitchType)).Length;
//         int randomIndex = Random.Range(0, pitchCount);

//         pitchType = (PitchType)randomIndex;
//     }

//     private void ApplyPitchSetting()
//     {
//         newPosition = new Vector3(
//             Random.Range(-0.2f, 0.2f),
//             Random.Range(0.5f, 1f),
//             18.199f
//         );

//         rb.position = newPosition;
//         transform.position = newPosition;

//         switch (pitchType)
//         {
//             case PitchType.Fastball:
//                 throwSpeed = Random.Range(15f, 18f);
//                 spinAxis = Vector3.right;
//                 spinSpeed = 12f;
//                 magnusStrength = 0.003f;
//                 break;

//             case PitchType.Slider:
//                 throwSpeed = Random.Range(9f, 11f);
//                 spinAxis = -Vector3.up;
//                 spinSpeed = Random.Range(13f, 15f);
//                 magnusStrength = 0.006f;
//                 break;

//             case PitchType.Curve:
//                 throwSpeed = Random.Range(7f, 10f);

//                 // 아래로 떨어지는 커브를 원하면 -Vector3.right 추천
//                 spinAxis = -Vector3.right;

//                 spinSpeed = Random.Range(15f, 20f);
//                 magnusStrength = 0.013f;
//                 break;
//         }
//     }

//     private void ApplyMagnusEffect(PitchType a)
//     {
//         Vector3 magnusForce =
//             magnusStrength *
//             Vector3.Cross(rb.angularVelocity, rb.linearVelocity);

//         magnusForce.y = Mathf.Clamp(magnusForce.y, -0.2f, 0.01f);

//         rb.AddForce(magnusForce, ForceMode.Force);
//     }

//     private void OnCollisionEnter(Collision collision)
//     {
//         Debug.Log($"공이 충돌한 오브젝트: {collision.gameObject.name}");
//     }

//     public void ResetBall()
//     {
//         if (rb == null)
//             rb = GetComponent<Rigidbody>();

//         isThrown = false;
//         isPreparingThrow = false;

//         rb.isKinematic = false;
//         rb.linearVelocity = Vector3.zero;
//         rb.angularVelocity = Vector3.zero;
//         rb.isKinematic = true;
//     }
// }

using System.Collections;
using UnityEngine;

public class Baseball : MonoBehaviour
{
    public enum PitchType
    {
        Fastball,
        Slider,
        Curve,
    }

    private Rigidbody rb;
    private Check_Location checkLocation;

    [Header("Pitch Type")]
    public PitchType pitchType;

    [Header("Random Setting")]
    public bool randomPitchOnStart = true;

    [Header("Throw Settings")]
    public float throwSpeed = 20f;

    [Header("Spin Settings")]
    public Vector3 spinAxis = Vector3.right;
    public float spinSpeed = 0f;

    [Header("Magnus Effect")]
    public float magnusStrength = 0.05f;

    private Vector3 newPosition = new Vector3();

    private bool isThrown = false;
    private bool isPreparingThrow = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        checkLocation = GetComponent<Check_Location>();
    }

    private void Start()
    {
        ResetBall();
    }

    private void FixedUpdate()
    {
        // 던진 이후에만 마그누스 효과 적용
        if (isThrown)
        {
            ApplyMagnusEffect(pitchType);
        }
    }

    // UI 또는 GameManager에서 이 함수를 호출하면 1초 뒤 투구한다.
    public void ThrowAfterOneSecond()
    {
        if (isPreparingThrow || isThrown)
            return;

        StartCoroutine(ThrowRoutine());
    }

    private IEnumerator ThrowRoutine()
    {
        isPreparingThrow = true;

        yield return new WaitForSeconds(1.0f);

        ThrowBall();

        isPreparingThrow = false;
    }

    private void ThrowBall()
    {
        // 물리 활성화
        rb.isKinematic = false;

        // 이전 속도 초기화
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (randomPitchOnStart)
        {
            SelectRandomPitch();
        }

        ApplyPitchSetting();

        Vector3 targetPosition = Vector3.zero;

        switch (pitchType)
        {
            case PitchType.Fastball:
                targetPosition = new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(0.3f, 1f),
                    0f
                );
                break;

            case PitchType.Curve:
                targetPosition = new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(0.3f, 1f),
                    0f
                );
                break;

            case PitchType.Slider:
                targetPosition = new Vector3(
                    Random.Range(-1.4f, -1.1f),
                    Random.Range(0.3f, 1f),
                    0f
                );
                break;
        }

        Debug.Log(
            $"Pitch Type: {pitchType}\n" +
            $"Start Location: {newPosition.ToString("F3")}\n" +
            $"Target Position: {targetPosition.ToString("F3")}\n" +
            $"Transform Position: {transform.position.ToString("F3")}"
        );

        // ApplyPitchSetting에서 공 위치가 순간 이동하므로,
        // 그 이후부터 targetZ 통과 체크를 시작해야 한다.
        if (checkLocation != null)
            checkLocation.BeginTracking();

        Vector3 throwDirection = (targetPosition - rb.position).normalized;

        rb.linearVelocity = throwDirection * throwSpeed;
        rb.angularVelocity = spinAxis.normalized * spinSpeed;

        isThrown = true;
    }

    private void SelectRandomPitch()
    {
        int pitchCount = System.Enum.GetValues(typeof(PitchType)).Length;
        int randomIndex = Random.Range(0, pitchCount);

        pitchType = (PitchType)randomIndex;
    }

    private void ApplyPitchSetting()
    {
        newPosition = new Vector3(
            Random.Range(-0.2f, 0.2f),
            Random.Range(0.5f, 1f),
            18.199f
        );

        rb.position = newPosition;
        transform.position = newPosition;

        switch (pitchType)
        {
            case PitchType.Fastball:
                throwSpeed = Random.Range(15f, 18f);
                spinAxis = Vector3.right;
                spinSpeed = 12f;
                magnusStrength = 0.003f;
                break;

            case PitchType.Slider:
                throwSpeed = Random.Range(9f, 11f);
                spinAxis = -Vector3.up;
                spinSpeed = Random.Range(13f, 15f);
                magnusStrength = 0.006f;
                break;

            case PitchType.Curve:
                throwSpeed = Random.Range(7f, 10f);
                spinAxis = -Vector3.right;
                spinSpeed = Random.Range(15f, 20f);
                magnusStrength = 0.013f;
                break;
        }
    }

    private void ApplyMagnusEffect(PitchType a)
    {
        Vector3 magnusForce =
            magnusStrength *
            Vector3.Cross(rb.angularVelocity, rb.linearVelocity);

        magnusForce.y = Mathf.Clamp(magnusForce.y, -0.2f, 0.01f);

        rb.AddForce(magnusForce, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"공이 충돌한 오브젝트: {collision.gameObject.name}");
    }

    public void ResetBall()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (checkLocation == null)
            checkLocation = GetComponent<Check_Location>();

        StopAllCoroutines();

        isThrown = false;
        isPreparingThrow = false;

        // velocity를 지울 때는 kinematic이 false인 상태에서 처리해야 한다.
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (checkLocation != null)
            checkLocation.ResetCheck();
    }
}