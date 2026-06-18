// using System.Collections;
// using UnityEngine;
// using TMPro;

// public class GamaManager : MonoBehaviour
// {
//     [Header("Panels")]
//     [SerializeField] private GameObject startPanel;
//     [SerializeField] private GameObject judgmentPanel;
//     [SerializeField] private GameObject pitchReviewPanel;
//     [SerializeField] private GameObject resultPanel;

//     [Header("Systems")]
//     [SerializeField] private BatterRandomizer batterRandomizer;
//     [SerializeField] private StrikeZone3D strikeZone;
//     [SerializeField] private Baseball baseball;

//     [Header("Result Text")]
//     [SerializeField] private TMP_Text resultText;
//     [SerializeField] private TMP_Text accuracyText;

//     private int strikeCount;
//     private int ballCount;
//     private int totalCount;
//     private int correctCount;

//     private bool currentPitchIsStrike;

//     private void Start()
//     {
//         startPanel.SetActive(true);
//         judgmentPanel.SetActive(false);
//         pitchReviewPanel.SetActive(false);
//         resultPanel.SetActive(false);
//     }

//     public void StartGame()
//     {
//         strikeCount = 0;
//         ballCount = 0;
//         totalCount = 0;
//         correctCount = 0;

//         startPanel.SetActive(false);
//         judgmentPanel.SetActive(false);
//         pitchReviewPanel.SetActive(false);
//         resultPanel.SetActive(false);

//         batterRandomizer.RandomizeBatter();

//         StartCoroutine(PitchRoutine());
//     }

//     private IEnumerator PitchRoutine()
//     {
//         judgmentPanel.SetActive(false);

//         strikeZone.ResetPitchResult();
//         baseball.ResetBall();

//         // 투구 전 스트라이크존 보여주기
//         strikeZone.ShowZone();

//         yield return new WaitForSeconds(1.5f);

//         // 공 던지기 직전에 스트라이크존 숨기기
//         strikeZone.HideZone();

//         baseball.ThrowAfterOneSecond();

//         // ThrowAfterOneSecond 내부 대기 1초 + 공 이동 시간 여유
//         yield return new WaitForSeconds(4.5f);

//         currentPitchIsStrike = strikeZone.WasLastPitchStrike;

//         judgmentPanel.SetActive(true);
//     }

//     public void OnClickStrike()
//     {
//         Judge(true);
//     }

//     public void OnClickBall()
//     {
//         Judge(false);
//     }

//     private void Judge(bool userSaidStrike)
//     {
//         judgmentPanel.SetActive(false);

//         totalCount++;

//         if (userSaidStrike == currentPitchIsStrike)
//         {
//             correctCount++;
//         }

//         if (currentPitchIsStrike)
//         {
//             strikeCount++;
//         }
//         else
//         {
//             ballCount++;
//         }

//         if (strikeCount >= 3 || ballCount >= 4)
//         {
//             ShowResult();
//         }
//         else
//         {
//             StartCoroutine(PitchRoutine());
//         }
//     }

//     private void ShowResult()
//     {
//         pitchReviewPanel.SetActive(true);
//         resultPanel.SetActive(true);

//         float accuracy = totalCount == 0
//             ? 0f
//             : (float)correctCount / totalCount * 100f;

//         resultText.text = strikeCount >= 3 ? "Strike Out!" : "Walk!";
//         accuracyText.text = $"Call Accuracy: {accuracy:F1}%";
//     }

//     public void RestartGame()
//     {
//         StartGame();
//     }
// }

using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;


public class PitchRecord
{
    // 실제 공이 스트라이크였는지
    public bool actualIsStrike;
    // 사용자가 Strike/Ball 중 무엇을 눌렀는지
    public bool userSaidStrike;
    // 사용자가 아직 판정을 안 눌렀는지 구분하기 위한 값
    public bool hasUserJudgment;
    // 정답 여부
    public bool isCorrect;
    // 공이 스트라이크존 z좌표를 통과한 위치
    public Vector3 arrivalPosition;
}

public class GamaManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject judgmentPanel;
    [SerializeField] private GameObject pitchReviewPanel;
    [SerializeField] private GameObject resultPanel;

    [Header("Systems")]
    [SerializeField] private BatterRandomizer batterRandomizer;
    [SerializeField] private StrikeZone3D strikeZone;
    [SerializeField] private Baseball baseball;

    [Header("Pitch Flow")]
    [SerializeField] private float zonePreviewTime = 1.5f;
    [SerializeField] private float pitchResultTimeout = 6.0f;
    [SerializeField] private float waitAfterPitchResult = 1.0f;

    [Header("Result Text")]
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text accuracyText;

    private readonly List<PitchRecord> pitchRecords = new List<PitchRecord>();

    private int currentPitchRecordIndex = -1;

    private Check_Location checkLocation;

    private int strikeCount;
    private int ballCount;
    private int totalCount;
    private int correctCount;

    private bool currentPitchIsStrike;
    private bool pitchResultReceived;

    private void Start()
    {
        startPanel.SetActive(true);
        judgmentPanel.SetActive(false);
        pitchReviewPanel.SetActive(false);
        resultPanel.SetActive(false);

        if (baseball != null)
        {
            checkLocation = baseball.GetComponent<Check_Location>();
            InitializeBallChecker();
        }
    }

    public void StartGame()
    {
        strikeCount = 0;
        ballCount = 0;
        totalCount = 0;
        correctCount = 0;

        pitchRecords.Clear();
        currentPitchRecordIndex = -1;

        startPanel.SetActive(false);
        judgmentPanel.SetActive(false);
        pitchReviewPanel.SetActive(false);
        resultPanel.SetActive(false);

        batterRandomizer.RandomizeBatter();

        StartCoroutine(PitchRoutine());
    }

    private IEnumerator PitchRoutine()
    {
        judgmentPanel.SetActive(false);

        pitchResultReceived = false;
        currentPitchIsStrike = false;

        PrepareBallForNextPitch();

        // 투구 전 스트라이크존 보여주기
        strikeZone.ShowZone();

        yield return new WaitForSeconds(zonePreviewTime);

        // 공 던지기 직전에 스트라이크존 숨기기
        strikeZone.HideZone();

        baseball.ThrowAfterOneSecond();

        float timer = 0f;
        while (!pitchResultReceived && timer < pitchResultTimeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (!pitchResultReceived)
        {
            Debug.LogWarning("공이 제한 시간 안에 targetZ를 통과하지 못했습니다. Ball로 처리합니다.");
            Vector3 timeoutPosition = baseball != null
            ? baseball.transform.position
            : Vector3.zero;

            ReceivePitchResult(false, timeoutPosition);

            if (baseball != null)
                baseball.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(waitAfterPitchResult);

        judgmentPanel.SetActive(true);
    }

    private void PrepareBallForNextPitch()
    {
        if (baseball == null)
        {
            Debug.LogWarning("Baseball이 GameManager에 연결되지 않았습니다.");
            return;
        }

        baseball.gameObject.SetActive(true);
        baseball.ResetBall();

        checkLocation = baseball.GetComponent<Check_Location>();
        InitializeBallChecker();
    }

    private void InitializeBallChecker()
    {
        if (checkLocation == null)
        {
            Debug.LogWarning("Baseball 오브젝트에 Check_Location이 없습니다.");
            return;
        }

        if (strikeZone == null)
        {
            Debug.LogWarning("StrikeZone3D가 GameManager에 연결되지 않았습니다.");
            return;
        }

        checkLocation.Initialize(strikeZone.ZoneCollider, this, strikeZone.transform.position.z);
    }

    public void ReceivePitchResult(bool isStrike, Vector3 arrivalPosition)
    {
        if (pitchResultReceived)
            return;

        currentPitchIsStrike = isStrike;
        pitchResultReceived = true;

        PitchRecord record = new PitchRecord
        {
            actualIsStrike = isStrike,
            userSaidStrike = false,
            hasUserJudgment = false,
            isCorrect = false,
            arrivalPosition = arrivalPosition
        };

        Debug.Log($"GameManager가 받은 판정: {(isStrike ? "Strike" : "Ball")}");
    }

    public void OnClickStrike()
    {
        Judge(true);
    }

    public void OnClickBall()
    {
        Judge(false);
    }

    private void Judge(bool userSaidStrike)
    {
        judgmentPanel.SetActive(false);

        totalCount++;

        bool isCorrect = (userSaidStrike == currentPitchIsStrike); //가독성을 위한 괄호

        if (isCorrect)
        {
            correctCount++;
        }

        // 현재 투구 기록에 사용자 판정 저장
        if (currentPitchRecordIndex >= 0 && currentPitchRecordIndex < pitchRecords.Count)
        {
            PitchRecord currentRecord = pitchRecords[currentPitchRecordIndex];

            currentRecord.userSaidStrike = userSaidStrike;
            currentRecord.hasUserJudgment = true;
            currentRecord.isCorrect = isCorrect;
        }

        if (currentPitchIsStrike)
        {
            strikeCount++;
        }
        else
        {
            ballCount++;
        }

        if (strikeCount >= 3 || ballCount >= 4)
        {
            ShowResult();
        }
        else
        {
            StartCoroutine(PitchRoutine());
        }
    }

    private void ShowResult()
    {
        pitchReviewPanel.SetActive(true);
        resultPanel.SetActive(true);

        float accuracy = totalCount == 0
            ? 0f
            : (float)correctCount / totalCount * 100f;

        resultText.text = strikeCount >= 3 ? "Strike Out!" : "Walk!";
        accuracyText.text = $"Call Accuracy: {accuracy:F1}%";
    }

    public void RestartGame()
    {
        StartGame();
    }
}