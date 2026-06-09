using UnityEngine;

// 이 스크립트가 붙어있는 오브젝트에
// BoxCollider가 반드시 존재하도록 강제
[RequireComponent(typeof(BoxCollider))]
public class StrikeZone3D : MonoBehaviour
{
    [Header("Strike Zone Size")]

    // 스트라이크 존 크기
    // X : 가로 폭
    // Y : 높이
    // Z : 판의 두께 (0으로 두지 말고 아주 얇게 유지)
    [SerializeField]
    private Vector3 size = new Vector3(1.0f, 1.4f, 0.05f);

    // 스트라이크 존의 Box Collider 참조
    private BoxCollider boxCollider;

    // 외부에서 읽기 전용으로 크기 확인 가능
    public Vector3 Size => size;

    /// <summary>
    /// 게임 시작 시 실행
    /// Collider를 가져오고 초기 설정 수행
    /// </summary>
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        UpdateCollider();
    }

    /// <summary>
    /// Inspector 값이 변경될 때마다 자동 호출
    /// 존 크기 변경 시 Collider도 같이 갱신
    /// </summary>
    private void OnValidate()
    {
        boxCollider = GetComponent<BoxCollider>();
        UpdateCollider();
    }

    /// <summary>
    /// 현재 size 값을 Box Collider에 적용
    /// </summary>
    private void UpdateCollider()
    {
        if (boxCollider == null)
            return;

        // 공을 막지 않고 통과시키면서
        // 진입 여부만 감지하도록 설정
        boxCollider.isTrigger = true;

        // Collider 중심을 오브젝트 중심에 맞춤
        boxCollider.center = Vector3.zero;

        // Collider 크기를 Strike Zone 크기와 동일하게 설정
        boxCollider.size = size;

        UpdateVisual();
    }

    /// <summary>
    /// 외부에서 존 크기를 변경할 때 사용하는 함수
    /// </summary>
    /// <param name="newSize">새로운 존 크기</param>
    public void SetSize(Vector3 newSize)
    {
        // 너무 작은 값이나 음수 방지
        size = new Vector3(
            Mathf.Max(0.01f, newSize.x),
            Mathf.Max(0.01f, newSize.y),
            Mathf.Max(0.01f, newSize.z)
        );

        UpdateCollider();
    }

    /// <summary>
    /// 다른 Collider가 존 안으로 들어왔을 때 호출
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Ball 태그를 가진 오브젝트만 판정
        if (other.CompareTag("Ball"))
        {
            Debug.Log("공이 스트라이크 존을 통과함");
        }
    }

    /// <summary>
    /// Scene 뷰에서 존을 시각적으로 표시
    /// 게임 실행 여부와 상관없이 보임
    /// </summary>
    private void OnDrawGizmos()
    {
        // 오브젝트의 위치와 회전을 기준으로 그리기
        Gizmos.matrix = transform.localToWorldMatrix;

        // 반투명 파란색 내부
        Gizmos.color = new Color(0.1f, 0.8f, 1.0f, 0.35f);
        Gizmos.DrawCube(Vector3.zero, size);

        // 외곽선
        Gizmos.color = new Color(0.1f, 0.8f, 1.0f, 1.0f);
        Gizmos.DrawWireCube(Vector3.zero, size);
    }

    [SerializeField] private Transform visual;

    private void UpdateVisual()
    {
        if (visual == null)
            return;

        visual.localPosition = Vector3.zero;
        visual.localScale = size;
    }
}