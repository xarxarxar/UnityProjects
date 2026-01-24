using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SerializableDictionary.Scripts;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 回答数学问题
/// </summary>
public class AnswerMathQues : BasePanel
{
    public SerializableDictionary<int, Button> NumberButton = new SerializableDictionary<int, Button>();
    public Button deleteButton;
    public Button ConfirmButton;
    public MySlider mySlider;
    public Text FirstNumberText;
    public Text SignText;//加减乘除符号
    public Text SecondNumberText;
    public Text AnswerText;//答案
    public Text CoinCountText;//金币数量
    public GameObject Mask;//遮挡玩家不要点击
    public event UnityAction<int> OnAnswerEnd;//答题结束

    private string currentAnswer;//当前玩家的输入
    private int answer;
    public float maxTime=30;
    private int currentCoinCount;
    public int maxCoinCount = 10000;

    protected override void InitPanel()
    {
        Debug.Log("初始化答题面板");
        foreach (var item in NumberButton.Dictionary)
        {
            item.Value.onClick.RemoveAllListeners();
            item.Value.onClick.AddListener(() => OnNumberButton(item.Key));
        }
        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(OnDelete);

        ConfirmButton.onClick.RemoveAllListeners();
        ConfirmButton.onClick.AddListener(OnConfirmButton);

        mySlider.OnCountDownEnd -= OnEnded;
        mySlider.OnCountDownEnd += OnEnded;

        CoinCountText.text = "10";
        currentCoinCount = 10;
        UIUtils.PlayNumberAnimation(CoinCountText, currentCoinCount,0.5f);
        SetMask(false);
        SetQuestion();
        mySlider.StartCountDown(maxTime, false);//开始倒计时，不使用游戏速度
    }

    //出一道题
    private void SetQuestion()
    {
        SetTextColor(Color.white);
        AnswerText.text = "";
        currentAnswer = "";

        float random = Random.Range(0, 1.0f);
        int firstNumber;
        int secondNumber;
        if (random < 0.5f)//题目设置为加法
        {
            firstNumber = Random.Range(0, 100);
            secondNumber = Random.Range(0, 100);
            SignText.text = "＋";
            answer = firstNumber + secondNumber;
        }
        else//题目设置为乘法
        {
            firstNumber = Random.Range(0, 100);
            secondNumber = Random.Range(0, 10);
            SignText.text = "×";
            answer = firstNumber * secondNumber;
        }
        FirstNumberText.text = firstNumber.ToString();
        SecondNumberText.text = secondNumber.ToString();
    }
    /// <summary>
    /// 确认按钮
    /// </summary>
    private void OnConfirmButton()
    {
        Debug.Log($"是否正确{IsAnswerCorrect()}");
        if (IsAnswerCorrect())
        {
            SetTextColor(Color.green);
            AnswerText.color=Color.green;
            mySlider.SetValue( Mathf.Clamp(mySlider.CurrentValue+(2/ maxTime),0,1));
            currentCoinCount = Mathf.Min(maxCoinCount,Mathf.RoundToInt(currentCoinCount*1.5f));
            UIUtils.PlayNumberAnimation(CoinCountText, currentCoinCount, 0.5f);

            SetMask(true);
            StartCoroutine(DelaySecond(0.5f,() =>
            {
                SetMask(false);
                SetTextColor(Color.white);
                //AnswerText.color = Color.white;
                SetQuestion();
            }));
        }
        else
        {
            SetTextColor(Color.red);
            //AnswerText.color=Color.red;
            SetMask(true);
            StartCoroutine(DelaySecond(2.0f,() =>
            {
                OnEnded();
            }));
            
        }
    }

    /// <summary>
    /// 数字按钮调用（0~9）
    /// </summary>
    private void OnNumberButton(int number)
    {
        currentAnswer += number.ToString();
        RefreshAnswerText();
    }

    /// <summary>
    /// 删除按钮
    /// </summary>
    public void OnDelete()
    {
        if (currentAnswer.Length <= 0) return;

        currentAnswer = currentAnswer.Substring(0, currentAnswer.Length - 1);
        RefreshAnswerText();
    }

    //答案是否正确
    private bool IsAnswerCorrect()
    {
        int value = 0;

        if (int.TryParse(currentAnswer, out value))
        {
            // value 就是正确的整数
            return value==answer;
        }
        else//无法转换
        {
            return false;
        }
    }

    /// <summary>
    /// 刷新答案显示
    /// </summary>
    private void RefreshAnswerText()
    {
        AnswerText.text = string.IsNullOrEmpty(currentAnswer) ? "" : currentAnswer;
    }

    //设置遮罩是否显示
    private void SetMask(bool show)
    {
        Mask.SetActive(show);
    }

    public override void OnCloseButton()
    {
        OnAnswerEnd?.Invoke(currentCoinCount);
        base.OnCloseButton();
    }

    private void OnEnded()
    {
        OnCloseButton();
    }

    IEnumerator DelaySecond(float delay,UnityAction callback)
    {
        yield return new WaitForSecondsRealtime(delay);

        callback?.Invoke();
        yield break;
    }


    private void SetTextColor(Color color)
    {
        FirstNumberText.color = color;
        SignText.color = color;
        SecondNumberText.color = color;
        AnswerText.color = color;
    }
}
