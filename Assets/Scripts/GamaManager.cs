using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private Baseball baseball;

    public void StartGame()
    {
        startPanel.SetActive(false);

        baseball.ThrowAfterOneSecond();
    }
}