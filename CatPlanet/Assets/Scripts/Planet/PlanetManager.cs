using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlanetManager : MonoBehaviour
{
    public static PlanetManager Instance;
    public static float marryWish = 0.9f;//结婚意愿
    public static float getBabyWish = 0.9f;//生育意愿
    public static float isTwins = 0.1f;//双胞胎概率
    public static float isTriplets = 0.05f;//三胞胎概率
    public static float babyDieProb = 0.08f;//儿童时期死亡率
    public static float dieProb = 0.03f;//成年死亡率
    public static float dieIncreaseProb = 0.01f;//大于60岁之后的死亡率增加速率
    public static float timeDelta = 0.5f;//普朗克时间
    public static float oneYearTime = 2.0f;//一年时间
    public static Bindable<int> CurrentYear = new Bindable<int>();//当前年份

    public CatBase CatBabyPrefab;
    public List<CatBase> maleCats = new List<CatBase>();//所有活着的猫咪-公
    public List<CatBase> femaleCats = new List<CatBase>();//所有活着的猫咪-母
    public List<(CatBase male, CatBase female, int lastBirthYear)> coupleList = new List<(CatBase, CatBase, int)>(); // 夫妻+最近生育年份
    public UnityAction<int> OnMaleCatCountChanged; // 监听公猫咪数量变化事件
    public UnityAction<int> OnFemaleCatCountChanged; // 监听公猫咪数量变化事件
    public Bindable<int> MaleCatCount = new Bindable<int>(); // 当前猫咪数量
    public Bindable<int> FemaleCatCount = new Bindable<int>(); // 当前猫咪数量


    public Text YearText;//当前年份
    public Text CatCountText;//公猫咪总数
    public Text MaleCatCountText;//公猫咪总数
    public Text FemaleCatCountText;//母猫咪总数

    private void Awake()
    {
        Instance = this;    
    }

    private void Start()
    {
        CurrentYear.OnValueChanged += (value) =>
        {
            YearText.text="Year:"+value.ToString();
        };
        MaleCatCount.OnValueChanged += (value) =>
        {
            MaleCatCountText.text = "Male:" + value.ToString();
            CatCountText.text = $"Total:{MaleCatCount.Value+ FemaleCatCount.Value}";
        };
        FemaleCatCount.OnValueChanged += (value) =>
        {
            FemaleCatCountText.text = "Female:" + value.ToString();
            CatCountText.text = $"Total:{MaleCatCount.Value + FemaleCatCount.Value}";
        };
        CurrentYear.Value = 1;
        SpawnInitialCats(25, 25); // 生成初始猫咪
        StartCoroutine(YearIncrease());
    }

    /// <summary>
    /// 生成初始猫咪
    /// </summary>
    /// <param name="maleCount">公猫数量</param>
    /// <param name="femaleCount">母猫数量</param>
    private void SpawnInitialCats(int maleCount, int femaleCount)
    {
        // 生成公猫
        for (int i = 0; i < maleCount; i++)
        {
            float x = Random.Range(-20f, 20f);
            float y = Random.Range(-4f, 4f);
            Vector3 spawnPos = new Vector3(x, y, 0);

            CatBase newCat = Instantiate(CatBabyPrefab, spawnPos, Quaternion.identity);
            newCat.Init(true, 18);
            maleCats.Add(newCat);
        }

        // 生成母猫
        for (int i = 0; i < femaleCount; i++)
        {
            float x = Random.Range(-20f, 20f);
            float y = Random.Range(-4f, 4f);
            Vector3 spawnPos = new Vector3(x, y, 0);

            CatBase newCat = Instantiate(CatBabyPrefab, spawnPos, Quaternion.identity);
            newCat.Init(false, 18);
            femaleCats.Add(newCat);
        }

        MaleCatCount.Value = maleCats.Count;
        FemaleCatCount.Value = femaleCats.Count;
    }

    private IEnumerator YearIncrease()
    {
        while (true)
        {
            yield return new WaitForSeconds(oneYearTime);
            CurrentYear.Value++;
            GetMarry();
            DoBirth();
            DoDie();
        }
    }

    /// <summary>
    /// 根据概率判断事件是否发生
    /// </summary>
    /// <param name="probability">发生的概率，取值范围 0~1，例如 0.1 表示 10%</param>
    /// <returns>true 表示事件发生，false 表示未发生</returns>
    public static bool IsEventHappen(float probability)
    {
        // 防御性检查
        probability = Mathf.Clamp01(probability); // 确保在 0~1 之间

        // 生成一个 0~1 之间的随机数
        float randomValue = Random.value;

        // 如果随机数小于概率，就认为事件发生
        return randomValue < probability;
    }

    // 安排猫咪结婚（加上年龄限制）
    public void GetMarry()
    {
        List<CatBase> availableMales = maleCats.FindAll(c => !c.IsMarried && c.age >= 18 && c.age <= 42);
        List<CatBase> availableFemales = femaleCats.FindAll(c => !c.IsMarried && c.age >= 18 && c.age <= 42);

        if (availableMales.Count == 0 || availableFemales.Count == 0) return;

        Shuffle(availableMales);
        Shuffle(availableFemales);

        int pairCount = Mathf.Min(availableMales.Count, availableFemales.Count);

        for (int i = 0; i < pairCount; i++)
        {
            bool maleAgree = IsEventHappen(marryWish);
            bool femaleAgree = IsEventHappen(marryWish);

            if (maleAgree && femaleAgree)
            {
                availableMales[i].IsMarried = true;
                availableFemales[i].IsMarried = true;
                coupleList.Add((availableMales[i], availableFemales[i], -999));
                Debug.Log($"{availableMales[i].name} 和 {availableFemales[i].name} 成功结婚！");
            }
        }
    }

    // 夫妻生育逻辑
    public void DoBirth()
    {
        foreach (var couple in coupleList)
        {
            CatBase male = couple.male;
            CatBase female = couple.female;

            if (male == null || female == null) continue;
            if (male.age < 18 || male.age > 42) continue;
            if (female.age < 18 || female.age > 42) continue;
            if (male.age - male.lastProcreateAge <= 1 || female.age - female.lastProcreateAge <= 1) continue;

            if (IsEventHappen(getBabyWish))
            {
                int babiesCount = 1;
                if (IsEventHappen(isTriplets)) babiesCount = 3;
                else if (IsEventHappen(isTwins)) babiesCount = 2;

                for (int i = 0; i < babiesCount; i++)
                {
                    bool gender = Random.value < 0.5f;
                    float x = Random.Range(-20f, 20f);
                    float y = Random.Range(-4f, 4f);
                    Vector3 spawnPos = new Vector3(x, y, 0);

                    CatBase newCat = Instantiate(CatBabyPrefab, spawnPos, Quaternion.identity);
                    newCat.Init(gender);

                    if (!gender) maleCats.Add(newCat);
                    else femaleCats.Add(newCat);

                    MaleCatCount.Value = maleCats.Count;
                    FemaleCatCount.Value = femaleCats.Count;
                }

                male.lastProcreateAge = male.age;
                female.lastProcreateAge = female.age;
            }
        }
    }

    //死亡逻辑
    public void DoDie()
    {
        // 公猫死亡
        for (int i = maleCats.Count - 1; i >= 0; i--)
        {
            CatBase cat = maleCats[i];
            if (cat == null) continue;

            // 计算死亡概率
            float finaldDieProb = 0f;
            if (cat.age <= 5)//儿童时期
            {
                finaldDieProb = babyDieProb;
            }
            else if (cat.age > 5 && cat.age<=60)
            {
                finaldDieProb = dieProb;
            }
            else
            {
                finaldDieProb = dieProb + (cat.age - 60) * dieIncreaseProb;
            }

            finaldDieProb = Mathf.Clamp(finaldDieProb, 0f, 0.9999f);

            if (IsEventHappen(finaldDieProb))
            {
                cat.StopAllCoroutines();
                Destroy(cat.gameObject);
                maleCats.RemoveAt(i);
                MaleCatCount.Value = maleCats.Count;
                CatBase.OnCatDie?.Invoke(cat);
            }
        }

        // 母猫死亡
        for (int i = femaleCats.Count - 1; i >= 0; i--)
        {
            CatBase cat = femaleCats[i];
            if (cat == null) continue;

            // 计算死亡概率
            float finaldDieProb = 0f;
            if (cat.age <= 5)//儿童时期
            {
                finaldDieProb = babyDieProb;
            }
            else if(cat.age > 5 && cat.age <= 60)
            {
                finaldDieProb = dieProb;
            }
            else
            {
                finaldDieProb = dieProb + (cat.age - 60) * dieIncreaseProb;
            }

            finaldDieProb = Mathf.Clamp(finaldDieProb, 0f, 0.9999f);

            if (IsEventHappen(finaldDieProb))
            {
                cat.StopAllCoroutines();
                Destroy(cat.gameObject);
                femaleCats.RemoveAt(i);
                FemaleCatCount.Value = femaleCats.Count;
                CatBase.OnCatDie?.Invoke(cat);
            }
        }

        // 移除夫妻关系
        coupleList.RemoveAll(c => c.male == null || c.female == null);
    }

    // 随机打乱列表
    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
    
    //增加猫咪到列表里
    private void AddCat(CatBase catBase)
    {
        if (!catBase.CatGender)//公
        {
            maleCats.Add(catBase);
        }
        else
        {
            femaleCats.Add(catBase);
        }
    }

    //从列表移除猫咪
    private void RemoveCats(CatBase catBase)
    {
        if (!catBase.CatGender)//公
        {
            maleCats.Remove(catBase);
        }
        else
        {
            femaleCats.Remove(catBase);
        }
    }
}
