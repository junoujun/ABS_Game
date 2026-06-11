using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject judgmentButtons;
    [SerializeField] private BatterRandomizer batterRandomizer;

    public void StartGame()
    {
        startPanel.SetActive(false);
        judgmentButtons.SetActive(true);

        batterRandomizer.RandomizeBatter();

        Debug.Log("게임 시작");
    }
}