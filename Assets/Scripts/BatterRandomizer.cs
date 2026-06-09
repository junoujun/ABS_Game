using UnityEngine;

public class BatterRandomizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform batter;
    [SerializeField] private StrikeZone3D strikeZone;

    [Header("Batter Standing Positions")]
    [SerializeField] private Transform leftBatterPosition;
    [SerializeField] private Transform rightBatterPosition;

    [Header("Batter Height Range (meter)")]
    [SerializeField] private float minHeight = 1.67f;
    [SerializeField] private float maxHeight = 2.03f;

    [Header("ABS Strike Zone Ratio")]
    [SerializeField] private float bottomRatio = 0.27f;
    [SerializeField] private float topRatio = 0.535f;

    [Header("Strike Zone Size")]
    [SerializeField] private float zoneWidth = 0.4318f; // 17 inch
    [SerializeField] private float zoneDepth = 0.05f;

    private void Start()
    {
        RandomizeBatter();
    }

    public void RandomizeBatter()
    {
        // 1. 타자 키 랜덤 생성
        float batterHeight = Random.Range(minHeight, maxHeight);

        // 2. 좌타석 / 우타석 랜덤 선택
        bool isLeftSide = Random.value < 0.5f;
        Transform selectedPosition = isLeftSide ? leftBatterPosition : rightBatterPosition;

        // 3. 타자 위치 배치
        batter.position = selectedPosition.position;

        // Capsule 기본 높이는 2m이므로 Y Scale은 키 / 2
        batter.localScale = new Vector3(
            batter.localScale.x,
            batterHeight / 2f,
            batter.localScale.z
        );

        // Capsule 중심이 가운데에 있으므로 Y 위치를 키의 절반만큼 올림
        batter.position = new Vector3(
            batter.position.x,
            batterHeight / 2f,
            batter.position.z
        );

        // 4. 스트라이크 존 높이 계산
        float zoneBottom = batterHeight * bottomRatio;
        float zoneTop = batterHeight * topRatio;
        float zoneHeight = zoneTop - zoneBottom;
        float zoneCenterY = (zoneBottom + zoneTop) / 2f;

        // 5. 스트라이크 존 크기 변경
        strikeZone.SetSize(new Vector3(zoneWidth, zoneHeight, zoneDepth));

        // 6. 스트라이크 존 위치 변경
        // 홈플레이트 중심 기준이므로 X=0, Z=0 유지
        strikeZone.transform.position = new Vector3(
            0f,
            zoneCenterY,
            0f
        );

        Debug.Log($"타자 키: {batterHeight * 100f:F1}cm / 존 아래: {zoneBottom:F2}m / 존 위: {zoneTop:F2}m");
    }
}