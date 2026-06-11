using System.Collections;
using UnityEngine;
using TMPro;

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

    [Header("Result Text")]
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text accuracyText;

    private int strikeCount;
    private int ballCount;
    private int totalCount;
    private int correctCount;

    private bool currentPitchIsStrike;

    private void Start()
    {
        startPanel.SetActive(true);
        judgmentPanel.SetActive(false);
        pitchReviewPanel.SetActive(false);
        resultPanel.SetActive(false);
    }

    public void StartGame()
    {
        strikeCount = 0;
        ballCount = 0;
        totalCount = 0;
        correctCount = 0;

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

        strikeZone.ResetPitchResult();
        baseball.ResetBall();

        // 투구 전 스트라이크존 보여주기
        strikeZone.ShowZone();

        yield return new WaitForSeconds(1.5f);

        // 공 던지기 직전에 스트라이크존 숨기기
        strikeZone.HideZone();

        baseball.ThrowAfterOneSecond();

        // ThrowAfterOneSecond 내부 대기 1초 + 공 이동 시간 여유
        yield return new WaitForSeconds(4.5f);

        currentPitchIsStrike = strikeZone.WasLastPitchStrike;

        judgmentPanel.SetActive(true);
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

        if (userSaidStrike == currentPitchIsStrike)
        {
            correctCount++;
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