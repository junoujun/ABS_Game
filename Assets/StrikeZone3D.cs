using UnityEngine;

/// <summary>
/// 3D 스트라이크 존을 관리하는 스크립트
/// - 스트라이크 존 크기 저장
/// - 특정 위치가 존 안에 있는지 판정
/// - Scene 뷰에서 존을 시각화
/// </summary>
public class StrikeZone3D : MonoBehaviour
{
    // 스트라이크 존 크기
    // x = 가로 폭
    // y = 높이
    // z = 깊이
    [SerializeField]
    private Vector3 size = new Vector3(1.0f, 1.4f, 0.25f);

    /// <summary>
    /// 외부 스크립트에서 현재 존 크기를 읽기 위한 프로퍼티
    /// </summary>
    public Vector3 Size => size;

    /// <summary>
    /// 월드 좌표 기준 점이 스트라이크 존 내부에 있는지 확인
    /// </summary>
    /// <param name="worldPoint">
    /// 판정할 위치 (보통 공의 최종 위치)
    /// </param>
    /// <returns>
    /// 존 안이면 true
    /// 존 밖이면 false
    /// </returns>
    public bool ContainsWorldPoint(Vector3 worldPoint)
    {
        // 월드 좌표를 스트라이크 존 기준 로컬 좌표로 변환
        Vector3 localPoint =
            transform.InverseTransformPoint(worldPoint);

        // 절반 크기 계산
        // 예) 가로가 1.0이면 좌우 각각 0.5
        Vector3 halfSize = size * 0.5f;

        // x, y, z 모두 범위 안에 있으면 스트라이크 존 내부
        return Mathf.Abs(localPoint.x) <= halfSize.x
            && Mathf.Abs(localPoint.y) <= halfSize.y
            && Mathf.Abs(localPoint.z) <= halfSize.z;
    }

    /// <summary>
    /// 스트라이크 존 크기 변경
    /// 나중에 타자 키에 따라 존을 변경할 때 사용
    /// </summary>
    /// <param name="newSize">새로운 존 크기</param>
    public void SetSize(Vector3 newSize)
    {
        size = newSize;
    }

    /// <summary>
    /// Scene 뷰에서 스트라이크 존을 시각화
    /// 게임 실행 중이 아니어도 보임
    /// </summary>
    private void OnDrawGizmos()
    {
        // 현재 오브젝트 기준 좌표계 사용
        Gizmos.matrix = transform.localToWorldMatrix;

        // 반투명 파란색 박스
        Gizmos.color = new Color(0.1f, 0.8f, 1.0f, 0.35f);
        Gizmos.DrawCube(Vector3.zero, size);

        // 외곽선
        Gizmos.color = new Color(0.1f, 0.8f, 1.0f, 1.0f);
        Gizmos.DrawWireCube(Vector3.zero, size);
    }
}