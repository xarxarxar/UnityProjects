// Robot.cs
// 控制机器人手势生成与游戏流程
using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelPlaying : MonoBehaviour
{
    public static LevelPlaying instance;

    public Hand robotHand=>HandControl.instance.otherHand; // 控制机器人手的组件
    //public Text gameCate; // 显示当前游戏类别（相同、相反、RPS）
    public CountDownSlider countDownSlider; // 倒计时条
    public Animator myAnimator;//播放锤击的动画
    public Animator robotAnimator;
    public Text addCoinCount;//获得的金币数量
    public GameObject victoryPanel;//胜利面板
    public Button pkButton;
    public bool isClickPkButton;//是否主动点击pkButton

    public GameObject defeatPanel;//无尽模式失败的面板
    public Text endlessAddCoinCount;//无尽模式获取金币的Text
    public GameObject pkbuttonFinger;

    private readonly string[] categoryNames = { "相同", "相反", "石头剪刀布" };


    private void Awake()
    {
        instance = this;
    }

    public void StartLevel()
    {
        AudioManager.instance.PlayBGM("GameBGM");
        PauseGame(false);
        StartCoroutine(PlayLevel()); // 开始关卡流程
    }

    public void StartEndlessMode()
    {
        AudioManager.instance.PlayBGM("GameBGM");
        PauseGame(false);
        StartCoroutine(EndlessMode()); // 开始关卡流程
    }

    /// <summary>
    /// 开始新手指引
    /// </summary>
    public void StartGuideLevel()
    {
        AudioManager.instance.PlayBGM("GameBGM");
        PauseGame(false);
        StartCoroutine(PlayGuideLevel()); // 开始关卡流程
    }


    public void GameOver()
    {
        countDownSlider.gameObject.SetActive(false);
        
        StopAllCoroutines();
    }

    public static bool isPaused = false; // 控制协程暂停的标志
    public static bool tmpPaused = false; // 控制协程暂停的标志

    public void PauseGame(bool pause)
    {
        isPaused = pause;
    }

    private IEnumerator PlayLevel()
    {
        int failCount = 0;
        int continuousSuccessCount = 0;

        // 等待期间机器人手指随机做动作
        Coroutine robotRandomGesture = StartCoroutine(HandControl.instance.otherHand.RandomGesture());
        int coin = 0;//该关卡获取的金币数量
        pkButton.onClick.AddListener(() =>
        {
            isClickPkButton=true;
        });

        for (int i = 0; i < LevelConfig.instance.RoundCount; i++)
        {
            // 暂停处理
            while (isPaused) yield return null;

            int category = LevelConfig.instance.gestureCategory[i];

            if (i == 3)
            {
                AudioManager.instance.PlaySFX("时间加速");
                while (isPaused) yield return null;
                yield return DynamicText.instance.ScaleTextToNormal("速度加快", 1, 1, new Color32(249, 255, 62, 255)).WaitForCompletion();
            }
            if (i == 6)
            {
                AudioManager.instance.PlaySFX("时间加速", 1.5f);
                while (isPaused) yield return null;
                yield return DynamicText.instance.ScaleTextToNormal("速度更快", 1, 1, new Color32(255, 67, 0, 255)).WaitForCompletion();
            }

            while (isPaused) yield return null;
            yield return DynamicText.instance.ScaleTextToNormal("准备跟我").WaitForCompletion();

            StopCoroutine(robotRandomGesture); // 停止协程

            while (isPaused) yield return null;
            DynamicText.instance.ScaleTextToNormal(categoryNames[category]).WaitForCompletion();

            SetGesture(category); // 设置机器人手势

            while (isPaused) yield return null;
            isClickPkButton = false;
            pkButton.gameObject.SetActive(true);
            countDownSlider.gameObject.SetActive(true);
            yield return StartCoroutine(countDownSlider.CountDown(i));
            pkButton.gameObject.SetActive(false);

            // 判断结果
            bool result = category switch
            {
                0 => HandControl.instance.IsSame(),
                1 => HandControl.instance.IsOpposite(),
                2 => HandControl.instance.IsRPSWin(),
                _ => false
            };

            //连胜次数
            continuousSuccessCount = result ? continuousSuccessCount + 1 : 0;

            //获取金币
            if (result)
            {
                int times = 1;
                if (isClickPkButton)
                {
                    if (countDownSlider.SliderRatioOnInterrupt >= 0.5f)//时间还剩大于一半
                    {
                        times = 3;//金币翻2倍
                    }
                    else
                    {
                        times = 2;//金币翻2倍
                    }
                }
                else
                {
                    times = 1;
                }
                //连胜多少就获取多少金币
                CoinManager.instance.GetCoin(continuousSuccessCount, times);
                coin += continuousSuccessCount* times;
            }

            failCount += result ? 0 : 1;
            LevelControl.instance.SetLightColor(i, result);

            if (result)
            {
                AudioManager.instance.PlaySFX("胜利", 1 + 0.1f * continuousSuccessCount);

                myAnimator.gameObject.SetActive(true);
                HandControl.instance.myHand.gameObject.SetActive(false);

                myAnimator.Play("锤击");

                bool animDone = false;
                StartCoroutine(WaitForAnimation(myAnimator, "锤击", () => animDone = true));

                string successText = GetSuccessText(continuousSuccessCount);
                bool textDone = false;
                DynamicText.instance.ScaleTextToNormal(successText).OnComplete(() => textDone = true);

                while (isPaused) yield return null;
                yield return new WaitUntil(() => animDone && textDone);

                myAnimator.gameObject.SetActive(false);
                HandControl.instance.myHand.gameObject.SetActive(true);
            }
            else
            {
                AudioManager.instance.PlaySFX("失败");

                robotAnimator.gameObject.SetActive(true);
                HandControl.instance.otherHand.gameObject.SetActive(false);

                robotAnimator.Play("锤击");

                bool animDone = false;
                StartCoroutine(WaitForAnimation(robotAnimator, "锤击", () => animDone = true));

                string failText = GetFailText(failCount);
                bool textDone = false;
                DynamicText.instance.ScaleTextToNormal(failText).OnComplete(() => textDone = true);

                while (isPaused) yield return null;
                yield return new WaitUntil(() => animDone && textDone);

                robotAnimator.gameObject.SetActive(false);
                HandControl.instance.otherHand.gameObject.SetActive(true);
            }

            robotRandomGesture = StartCoroutine(HandControl.instance.otherHand.RandomGesture());
        }

        //switch (failCount)
        //{
        //    case 0:
        //        Debug.Log("你获得了三星");
        //        break;
        //    case 1:
        //        Debug.Log("你获得了两星");
        //        break;
        //    case 2:
        //        Debug.Log("你获得了一星");
        //        break;
        //    default:
        //        Debug.Log("你失败了");
        //        break;
        //}
        victoryPanel.SetActive(true);//赢了
        GameManager.instance.gameInfo.Level += 1;//关卡+1
        addCoinCount.text = $"+{coin}";
        GameManager.instance. UpdateCoinText();


        StopAllCoroutines();
    }

    private IEnumerator PlayGuideLevel()
    {
        int continuousSuccessCount = 0;

        // 等待期间机器人手指随机做动作
        Coroutine robotRandomGesture = StartCoroutine(HandControl.instance.otherHand.RandomGesture());
        int coin = 0;//该关卡获取的金币数量
        pkButton.onClick.AddListener(() =>
        {
            isClickPkButton = true;
        });

        for (int i = 0; i < LevelConfig.instance.RoundCount; i++)
        {
            // 暂停处理
            while (isPaused) yield return null;

            int category = LevelConfig.instance.gestureCategory[i];

            while (isPaused) yield return null;
            yield return DynamicText.instance.ScaleTextToNormal("准备跟我").WaitForCompletion();

            StopCoroutine(robotRandomGesture); // 停止协程

            while (isPaused) yield return null;
            DynamicText.instance.ScaleTextToNormal(categoryNames[category]).WaitForCompletion();

            SetGesture(category); // 设置机器人手势

            while (isPaused) yield return null;
            isClickPkButton = false;
            pkButton.gameObject.SetActive(true);
            countDownSlider.gameObject.SetActive(true);

            //此处暂停倒计时
            tmpPaused = true;
            if (i == 0)
            {
                Guide.instance.SetTip("点击下方五个按钮，将下方手势变为与上方相同");
                List<Button> fingerButtons = Guide.instance.SetButtonActive(2, 3, 4, 5, 6);
                foreach (Button button in fingerButtons)
                {
                    button.onClick.AddListener(TmpClick);
                }

                void TmpClick()
                {
                    // 判断结果
                    bool result = category switch
                    {
                        0 => HandControl.instance.IsSame(),
                        1 => HandControl.instance.IsOpposite(),
                        2 => HandControl.instance.IsRPSWin(),
                        _ => false
                    };
                    //如果玩家做对了
                    if (result)
                    {
                        Guide.instance.SetTip("倒计时结束后会进行PK");
                        Guide.instance.SetButtonActive(0,2, 3, 4, 5, 6);//复原按钮不能点击的状态,除了PK按钮
                        tmpPaused = false;
                        //移除这fingerButtons中所有button的TmpClick点击事件
                        foreach (Button button in fingerButtons)
                        {
                            button.onClick.RemoveListener(TmpClick);
                        }
                    }
                }
            }

            if (i == 1)
            {
                Guide.instance.SetTip("点击下方五个按钮，将下方手势变为与上方相反");
                List<Button> fingerButtons = Guide.instance.SetButtonActive(2, 3, 4, 5, 6);
                foreach (Button button in fingerButtons)
                {
                    button.onClick.AddListener(TmpClick);
                }

                void TmpClick()
                {
                    // 判断结果
                    bool result = category switch
                    {
                        0 => HandControl.instance.IsSame(),
                        1 => HandControl.instance.IsOpposite(),
                        2 => HandControl.instance.IsRPSWin(),
                        _ => false
                    };
                    //如果玩家做对了
                    if (result)
                    {
                        Guide.instance.CloseTip();
                        Guide.instance.SetButtonActive(0, 2, 3, 4, 5, 6);
                        tmpPaused = false;

                        foreach (Button button in fingerButtons)
                        {
                            button.onClick.RemoveListener(TmpClick);
                        }

                        StartCoroutine(WaitAndDoTmpFunc());
                    }

                    IEnumerator WaitAndDoTmpFunc()
                    {
                        yield return new WaitForSeconds(1f);

                        tmpPaused = true;
                        Guide.instance.SetTip("点击PK按钮可以提前进行PK\n倒计时过半前PK胜利可以获取三倍金币");
                        List<Button> pkbuttons = Guide.instance.SetButtonActive(1);
                        Button pkButton = pkbuttons[0];
                        pkbuttonFinger.SetActive(true);
                        pkButton.onClick.AddListener(() =>
                        {
                            pkbuttonFinger.SetActive(false);
                            tmpPaused = false;
                            Guide.instance.CloseTip();
                            Guide.instance.SetButtonActive(0, 2, 3, 4, 5, 6);
                        });
                    }
                }
            }

            if (i == 2)
            {
                Guide.instance.SetTip("点击下方五个按钮，赢得比赛");
                List<Button> fingerButtons = Guide.instance.SetButtonActive(2, 3, 4, 5, 6);
                foreach (Button button in fingerButtons)
                {
                    button.onClick.AddListener(TmpClick);
                }

                void TmpClick()
                {
                    // 判断结果
                    bool result = category switch
                    {
                        0 => HandControl.instance.IsSame(),
                        1 => HandControl.instance.IsOpposite(),
                        2 => HandControl.instance.IsRPSWin(),
                        _ => false
                    };
                    //如果玩家做对了
                    if (result)
                    {
                        Guide.instance.SetTip("倒计时过半后提前PK也会获取两倍金币\n倒计时结束后自动PK则不会获取额外金币");
                        Guide.instance.SetButtonActive(0,1, 2, 3, 4, 5, 6);//复原按钮不能点击的状态,除了PK按钮
                        tmpPaused = false;
                        //移除这fingerButtons中所有button的TmpClick点击事件
                        foreach (Button button in fingerButtons)
                        {
                            button.onClick.RemoveListener(TmpClick);
                        }
                    }
                }
            }
            yield return StartCoroutine(countDownSlider.GuideCountDown(5));//设置倒计时为10秒
            Guide.instance.CloseTip();

            pkButton.gameObject.SetActive(false);

            // 判断结果
            bool result = category switch
            {
                0 => HandControl.instance.IsSame(),
                1 => HandControl.instance.IsOpposite(),
                2 => HandControl.instance.IsRPSWin(),
                _ => false
            };

            //连胜次数
            continuousSuccessCount = result ? continuousSuccessCount + 1 : 0;

            //获取金币
            if (result)
            {
                int times = 1;
                if (isClickPkButton)
                {
                    if (countDownSlider.SliderRatioOnInterrupt >= 0.5f)//时间还剩大于一半
                    {
                        times = 3;//金币翻2倍
                    }
                    else
                    {
                        times = 2;//金币翻2倍
                    }
                }
                else
                {
                    times = 1;
                }
                //连胜多少就获取多少金币
                CoinManager.instance.GetCoin(continuousSuccessCount, times);
                coin += continuousSuccessCount * times;
            }

            LevelControl.instance.SetLightColor(i, result);

            if (result)
            {
                AudioManager.instance.PlaySFX("胜利", 1 + 0.1f * continuousSuccessCount);

                myAnimator.gameObject.SetActive(true);
                HandControl.instance.myHand.gameObject.SetActive(false);

                myAnimator.Play("锤击");

                bool animDone = false;
                StartCoroutine(WaitForAnimation(myAnimator, "锤击", () => animDone = true));

                string successText = GetSuccessText(continuousSuccessCount);
                bool textDone = false;
                DynamicText.instance.ScaleTextToNormal(successText).OnComplete(() => textDone = true);

                while (isPaused) yield return null;
                yield return new WaitUntil(() => animDone && textDone);

                myAnimator.gameObject.SetActive(false);
                HandControl.instance.myHand.gameObject.SetActive(true);
            }

            robotRandomGesture = StartCoroutine(HandControl.instance.otherHand.RandomGesture());
        }

        victoryPanel.SetActive(true);//赢了
        GameManager.instance.gameInfo.Level += 1;//关卡+1
        addCoinCount.text = $"+{coin}";
        GameManager.instance.UpdateCoinText();
        StopAllCoroutines();
    }

    /// <summary>
    /// 无尽模式，失败五次以后结束关卡
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndlessMode()
    {
        int failCount = 0;//失败的次数
        int continuousSuccessCount = 0;//连胜的次数

        // 等待期间机器人手指随机做动作
        Coroutine robotRandomGesture = StartCoroutine(HandControl.instance.otherHand.RandomGesture());
        int coin = 0;//该关卡获取的金币数量
        pkButton.onClick.AddListener(() =>
        {
            isClickPkButton = true;
        });

        while (isPaused) yield return null;
        yield return DynamicText.instance.ScaleTextToNormal("失败五次无尽模式结束",1,1.5f).WaitForCompletion();

        //for (int i = 0; i < LevelConfig.instance.RoundCount; i++)
        int index = 0;//回合数
        while(failCount<5)
        {
            // 暂停处理
            while (isPaused) yield return null;

            index++;

            int category = Random.Range(0,3);//玩的种类，每一回合的手势玩法（0=相同, 1=相反, 2=RPS）

            while (isPaused) yield return null;
            yield return DynamicText.instance.ScaleTextToNormal("准备跟我").WaitForCompletion();

            StopCoroutine(robotRandomGesture); // 停止协程

            while (isPaused) yield return null;
            DynamicText.instance.ScaleTextToNormal(categoryNames[category]).WaitForCompletion();

            SetGesture(category); // 设置机器人手势

            while (isPaused) yield return null;
            isClickPkButton = false;
            pkButton.gameObject.SetActive(true);
            countDownSlider.gameObject.SetActive(true);
            float waitTime=LevelConfig.instance.WaitTime-0.1f*index;//无尽模式下的等待时长每回合减少0.1秒
            waitTime=Mathf.Max(waitTime,2.0f);//无尽模式下的等待时长每回合减少0.1秒,最少为2秒
            yield return StartCoroutine(countDownSlider.EndlessCountDown(waitTime));
            pkButton.gameObject.SetActive(false);

            // 判断结果
            bool result = category switch
            {
                0 => HandControl.instance.IsSame(),
                1 => HandControl.instance.IsOpposite(),
                2 => HandControl.instance.IsRPSWin(),
                _ => false
            };

            //连胜次数
            continuousSuccessCount = result ? continuousSuccessCount + 1 : 0;

            //获取金币
            if (result)
            {
                int times = 1;
                if (isClickPkButton)
                {
                    if (countDownSlider.SliderRatioOnInterrupt >= 0.5f)//时间还剩大于一半
                    {
                        times = 3;//金币翻2倍
                    }
                    else
                    {
                        times = 2;//金币翻2倍
                    }
                }
                else
                {
                    times = 1;
                }
                //连胜多少就获取多少金币
                CoinManager.instance.GetCoin(continuousSuccessCount, times);
                coin += continuousSuccessCount * times;
            }

            failCount += result ? 0 : 1;
            if(!result) { LevelControl.instance.SetLightColor(failCount-1, result); }//失败了，红灯+1

            if (failCount >= 5)
            {

            }

            if (result)
            {
                AudioManager.instance.PlaySFX("胜利", 1 + 0.1f * continuousSuccessCount);

                myAnimator.gameObject.SetActive(true);
                HandControl.instance.myHand.gameObject.SetActive(false);

                myAnimator.Play("锤击");

                bool animDone = false;
                StartCoroutine(WaitForAnimation(myAnimator, "锤击", () => animDone = true));

                string successText = GetSuccessText(continuousSuccessCount);
                bool textDone = false;
                DynamicText.instance.ScaleTextToNormal(successText).OnComplete(() => textDone = true);

                while (isPaused) yield return null;
                yield return new WaitUntil(() => animDone && textDone);

                myAnimator.gameObject.SetActive(false);
                HandControl.instance.myHand.gameObject.SetActive(true);
            }
            else
            {
                AudioManager.instance.PlaySFX("失败");

                robotAnimator.gameObject.SetActive(true);
                HandControl.instance.otherHand.gameObject.SetActive(false);

                robotAnimator.Play("锤击");

                bool animDone = false;
                StartCoroutine(WaitForAnimation(robotAnimator, "锤击", () => animDone = true));

                string failText = GetFailText(failCount);
                bool textDone = false;
                DynamicText.instance.ScaleTextToNormal(failText).OnComplete(() => textDone = true);

                while (isPaused) yield return null;
                yield return new WaitUntil(() => animDone && textDone);

                robotAnimator.gameObject.SetActive(false);
                HandControl.instance.otherHand.gameObject.SetActive(true);
            }

            robotRandomGesture = StartCoroutine(HandControl.instance.otherHand.RandomGesture());
        }
        AudioManager.instance.PlaySFX("认输");
        defeatPanel.SetActive(true);//无尽模式结束
        endlessAddCoinCount.text = $"+{coin}";
        GameManager.instance.UpdateCoinText();

        StopAllCoroutines();
    }

    private string GetSuccessText(int count)
    {
        return count switch
        {
            1 => "牛！",
            2 => "好牛！",
            3 => "太牛了！",
            _ => $"{count}连胜！"
        };
    }

    private string GetFailText(int count)
    {
        return count switch
        {
            1 => "菜！",
            2 => "好菜！",
            3 => "太菜了！",
            _ => "不忍直视！"
        };
    }

    private IEnumerator WaitForAnimation(Animator animator, string stateName, System.Action onDone)
    {
        // 等待动画状态切换成功
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(stateName));

        // 等待动画播完一轮（normalizedTime >= 1）
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        onDone?.Invoke();
    }

    private void SetGesture(int category)
    {
        if (category == 0 || category == 1)
        {
            bool[] tmp=new bool[5];
            for (int i = 0; i < 5; i++)
            {
                tmp[i] = Random.value > 0.5f;
            }
            robotHand.SetFingers(tmp); 
        }
        else if (category == 2)
        {
            bool[][] RPS = {
                new[] { false, false, false, false, false }, // 石头
                new[] { false, true, true, false, false },   // 剪刀
                new[] { true, true, true, true, true }       // 布
            };
            robotHand.SetFingers(RPS[Random.Range(0, RPS.Length)]);
        }
    }
}
