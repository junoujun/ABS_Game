using System;
using UnityEngine;

public class Baseball : MonoBehaviour
{
    // 사용할 구종 종류를 열거형으로 정의
    public enum PitchType
    {
        Fastball,   // 직구
        Slider,     // 슬라이더
        Curve,      // 커브
        ChangeUp    // 체인지업
    }

    // 공에 붙어 있는 Rigidbody 컴포넌트를 저장할 변수
    private Rigidbody rb;

    [Header("Pitch Type")]
    // 현재 공의 구종
    public PitchType pitchType;

    [Header("Random Setting")]
    // 게임 시작 시 구종을 랜덤으로 선택할지 여부
    public bool randomPitchOnStart = true;

    [Header("Throw Settings")]
    // 공이 앞으로 날아가는 속도
    public float throwSpeed = 20f;

    [Header("Spin Settings")]
    // 공이 회전하는 축
    public UnityEngine.Vector3 spinAxis = UnityEngine.Vector3.right;

    // 공의 회전 속도
    public float spinSpeed = 0f;

    [Header("Magnus Effect")]
    // 마그누스 효과의 세기
    // 값이 클수록 회전에 의한 궤적 변화가 커짐
    public float magnusStrength = 0.05f;

    // 공의 새 시작 위치를 저장할 변수
    private UnityEngine.Vector3 newPosition = new UnityEngine.Vector3();

    private void Start()
    {
        // 현재 오브젝트에서 Rigidbody 컴포넌트를 가져옴
        rb = GetComponent<Rigidbody>();

        // 현재 공의 시작 위치를 저장
        newPosition = transform.position;

        // randomPitchOnStart가 true라면 시작할 때 구종을 랜덤으로 선택
        if (randomPitchOnStart)
        {
            SelectRandomPitch();
        }

        // 선택된 구종에 맞게 속도, 회전축, 회전속도, 마그누스 효과 값을 설정
        ApplyPitchSetting();

        // 공을 오브젝트의 forward 방향으로 던짐

        UnityEngine.Vector3 targetPosition = new UnityEngine.Vector3();//(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0.3f, 1f), 0f);
        
        switch (pitchType)
        {
            case PitchType.Fastball:
                targetPosition = new UnityEngine.Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0.3f, 1f), 0f);
                break;

            case PitchType.Curve:
                targetPosition = new UnityEngine.Vector3(UnityEngine.Random.Range(-3f, -2f), UnityEngine.Random.Range(0.3f, 1f), 0f);
                break;

            case PitchType.Slider:
                targetPosition = new UnityEngine.Vector3(UnityEngine.Random.Range(-2.6f, -1.9f), UnityEngine.Random.Range(0.3f, 1f), 0f);
                break;

            case PitchType.ChangeUp:
                targetPosition = new UnityEngine.Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0.3f, 1f), 0f);
                break;
            
            default:
                break;
        }

        // 설정된 투구 정보를 Console에 출력
        Debug.Log(
            $"Start Location: {newPosition.ToString("F3")}\n" +
            $"Target Position: {targetPosition.ToString("F3")}\n"+
            $"Transform Position: {transform.position.ToString("F3")}"
        );
        
        
        UnityEngine.Vector3 throwDirection = (targetPosition - rb.position).normalized;

        rb.linearVelocity = throwDirection * throwSpeed;

        // 설정된 회전축과 회전속도에 따라 공을 회전시킴
        rb.angularVelocity = spinAxis.normalized * spinSpeed;
    }

    private void FixedUpdate()
    {
        // 물리 업데이트마다 마그누스 효과를 적용
        ApplyMagnusEffect(pitchType);
    }

    private void SelectRandomPitch()
    {
        // PitchType 열거형에 정의된 구종 개수를 구함
        int pitchCount = System.Enum.GetValues(typeof(PitchType)).Length;

        // 0부터 구종 개수 전까지의 랜덤 인덱스를 생성
        int randomIndex = UnityEngine.Random.Range(0, pitchCount);

        // 랜덤 인덱스를 PitchType으로 변환하여 현재 구종으로 설정
        pitchType = (PitchType)randomIndex;
    }

    private void ApplyPitchSetting()
    {
        // 공의 시작 위치를 랜덤하게 설정
        // x: 좌우 랜덤 위치
        // y: 높이 랜덤 위치
        // z: 투구 시작 위치
        newPosition = new UnityEngine.Vector3(
            UnityEngine.Random.Range(-0.2f, 0.2f),
            UnityEngine.Random.Range(0.5f, 1f),
            18.199f
        );

        // Rigidbody의 위치를 새 위치로 이동
        rb.position = newPosition;

        // Transform의 위치도 새 위치로 이동
        transform.position = newPosition;

        // 선택된 구종에 따라 투구 설정을 다르게 적용
        switch (pitchType)
        {
            case PitchType.Fastball:
                // 직구
                // 속도가 빠르고 궤적 변화가 작음
                throwSpeed = UnityEngine.Random.Range(15f, 18f);
                spinAxis = UnityEngine.Vector3.right;
                spinSpeed = 12f;
                magnusStrength = 0.003f;
                break;

            case PitchType.Slider:
                // 슬라이더
                // 중간 속도이며 좌우 방향 변화가 있음
                throwSpeed = UnityEngine.Random.Range(9f, 11f);
                spinAxis = -UnityEngine.Vector3.up;
                //spinSpeed = UnityEngine.Random.Range(10f, 15f);
                spinSpeed = 25f;
                magnusStrength = 0.006f;
                break;

            case PitchType.Curve:
                // 커브
                // 속도가 느리고 아래로 크게 떨어지는 궤적
                throwSpeed = UnityEngine.Random.Range(6f, 9f);
                spinAxis = UnityEngine.Vector3.right;
                spinSpeed = UnityEngine.Random.Range(10f, 13f);
                magnusStrength = 0.010f;
                break;

            case PitchType.ChangeUp:
                // 체인지업
                // 속도가 느리고 변화량은 비교적 작음
                throwSpeed = UnityEngine.Random.Range(7f, 9f);
                spinAxis = UnityEngine.Vector3.right;
                spinSpeed = UnityEngine.Random.Range(9f, 11f);
                magnusStrength = 0.002f;
                break;
        }
    }

    private void ApplyMagnusEffect(PitchType a)
    {
        // 마그누스 힘 계산
        // 회전 방향과 이동 방향의 외적을 이용해서 휘어지는 방향을 구함
        UnityEngine.Vector3 magnusForce =
            magnusStrength *
            UnityEngine.Vector3.Cross(rb.angularVelocity, rb.linearVelocity);

        // if(a != PitchType.Curve)
        // {
            magnusForce.y = Mathf.Clamp(magnusForce.y, -0.2f, 0.01f);
        // }

        // 계산된 마그누스 힘을 Rigidbody에 적용
        rb.AddForce(magnusForce, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"공이 충돌한 오브젝트: {collision.gameObject.name}");
    }
}