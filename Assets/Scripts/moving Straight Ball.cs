using UnityEngine;

public class Baseball : MonoBehaviour
{
    public enum PitchType
    {
        Fastball,
        Slider,
        Curve,
        ChangeUp
    }

    private Rigidbody rb;

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
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (randomPitchOnStart)
        {
            SelectRandomPitch();
        }

        ApplyPitchSetting();

        //Debug.Log($"ThrowSpeed: {throwSpeed} \nSpinSpeed:  {spinSpeed} \nStart Location: {newPosition}");
        Debug.Log(
            $"ThrowSpeed: {throwSpeed:F2}\n" +
            $"SpinSpeed: {spinSpeed:F2}\n" +
            $"Start Location: {newPosition.ToString("F3")}\n" +
            $"Transform Position: {transform.position.ToString("F3")}"
        );

        rb.linearVelocity = transform.forward * throwSpeed;
        rb.angularVelocity = spinAxis.normalized * spinSpeed;
    }

    private void FixedUpdate()
    {
        ApplyMagnusEffect();
    }

    private void SelectRandomPitch()
    {
        int pitchCount = System.Enum.GetValues(typeof(PitchType)).Length;
        int randomIndex = Random.Range(0, pitchCount);

        pitchType = (PitchType)randomIndex;
    }

    private void ApplyPitchSetting()
    {
        newPosition = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.5f, 1.5f), 0);
        rb.position = newPosition;
        transform.position = newPosition;

        switch (pitchType)
        {
            case PitchType.Fastball:
                // 빠르고 거의 직선
                //throwSpeed = 26f;
                throwSpeed = Random.Range(25f, 28f);
                spinAxis = Vector3.right;
                spinSpeed = 12f;
                magnusStrength = 0.003f;
                break;

            case PitchType.Slider:
                // 중간 속도, 좌우 변화
                //throwSpeed = 23f;
                throwSpeed = Random.Range(19f, 22f);
                spinAxis = -Vector3.up;
                //spinSpeed = 15f;
                spinSpeed = Random.Range(10f, 15f);
                magnusStrength = 0.006f;
                break;

            case PitchType.Curve:
                // 느리고 아래로 크게 떨어짐
                //throwSpeed = 15f;
                throwSpeed = Random.Range(13f, 16f);
                spinAxis = Vector3.right;
                //spinSpeed = 10f;
                spinSpeed = Random.Range(10f, 13f);
                magnusStrength = 0.010f;
                break;

            case PitchType.ChangeUp:
                // 느리고 변화 적음
                //throwSpeed = 15f;
                throwSpeed = Random.Range(14f, 16f);
                spinAxis = Vector3.right;
                //spinSpeed = 10f;
                spinSpeed = Random.Range(9f, 11f);
                magnusStrength = 0.002f;
                break;
        }
    }

    private void ApplyMagnusEffect()
    {
        Vector3 magnusForce =
            magnusStrength *
            Vector3.Cross(rb.angularVelocity, rb.linearVelocity);

        rb.AddForce(magnusForce, ForceMode.Force);
    }

    
}