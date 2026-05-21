using UnityEngine;

public class Baseball : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Throw Settings")]
    public float throwSpeed = 20f;

    [Header("Spin Settings")]
    public Vector3 spinAxis = Vector3.right;
    public float spinSpeed = 0f;

    [Header("Magnus Effect")]
    public float magnusStrength = 0.05f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 공 발사
        rb.linearVelocity = transform.forward * throwSpeed;

        // 공 회전
        rb.angularVelocity = spinAxis.normalized * spinSpeed;
    }

    private void FixedUpdate()
    {
        ApplyMagnusEffect();
    }

    private void ApplyMagnusEffect()
    {
        Vector3 magnusForce =
            magnusStrength *
            Vector3.Cross(rb.angularVelocity, rb.linearVelocity);

        rb.AddForce(magnusForce, ForceMode.Force);
    }
}