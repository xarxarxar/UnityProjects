using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private Text levelText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Image background;
    [SerializeField] private Color lockedColor;
    [SerializeField] private Color unlockedColor;

    [SerializeField] private int levelNumber;
    private Button button;

    public int LevelNumber => levelNumber;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Initialize(int levelNumber, bool isLocked, int bestScore, System.Action onClick)
    {
        this.levelNumber = levelNumber;

        levelText.text = $"L{levelNumber}";
        scoreText.text = bestScore > 0 ? $"BEST: {bestScore}" : "NOT PLAY";

        UpdateLockState(isLocked);
        button.onClick.AddListener(() => onClick?.Invoke());
    }

    public void UpdateLockState(bool isLocked)
    {
        //lockIcon.gameObject.SetActive(isLocked);
        background.color = isLocked ? lockedColor : unlockedColor;
        button.interactable = !isLocked;
    }

    public void Reset()
    {
        button.onClick.RemoveAllListeners();
        levelNumber = 0;
        scoreText.text = "";
    }
}
