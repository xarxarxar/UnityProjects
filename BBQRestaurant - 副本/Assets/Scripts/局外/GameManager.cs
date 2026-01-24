using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //
    public Button startButton;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(StartLevel);

        LevelManager.OnEndBattle += EndLevel;
    }

    private void StartLevel()
    {
        GameUIManager.Instance.HideMainPanel();
        LevelManager.Instance.Init();//³õÊ¼»¯¹Ø¿¨
    }

    private void EndLevel()
    {
        GameUIManager.Instance.ShowMainPanel();
    }
}
