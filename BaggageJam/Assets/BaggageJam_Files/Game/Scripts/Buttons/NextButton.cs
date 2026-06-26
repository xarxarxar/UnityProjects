namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    
    public class NextButton : ButtonBase
    {
        [SerializeField] private TextMeshProUGUI coinText;
        void Start()
        {
            Button.onClick.AddListener(OnNextButtonClicked);
        }
    
        void OnNextButtonClicked()
        {
            GameData data = GameManager.Instance.data;
    
            UICoinEffect.Instance.CountCoins();
    
            data.levelCount++;
    
            if (data.levelCount >= GameManager.Instance.data.AllSo.LevelDataSO.Levels.Count)
            {
                data.levelCount = Random.Range(1, GameManager.Instance.data.AllSo.LevelDataSO.Levels.Count);
            }
            SaveManager.SaveData(data);
    
            Button.interactable = false;
    
            transform.parent.GetComponent<Animator>().enabled = false;
            transform.DOScale(Vector3.zero, .3f);
        }
    
        void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnWin, OnWin);
        }
        void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnWin, OnWin);
        }
    
        void OnWin()
        {
            coinText.text = GameManager.Instance.data.AllSo.LevelDataSO.Levels[GameManager.Instance.data.levelCount - 1].winMoney.ToString();
        }
    }
    
}
