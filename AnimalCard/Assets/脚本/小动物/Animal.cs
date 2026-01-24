using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Animal : MonoBehaviour
{
    //可遗传的属性
    public AnimalBaseData BaseData;
    public float Strength;//体力
    public float Hp;//血量
    public float Speed;//速度
    public string Race;//种族是什么

    //不可遗传的属性
    [HideInInspector]public Animal Father;
    [HideInInspector]public Animal Mother;
    public int Age;//当前年龄,按照月来计算，显示的时候可以只显示年或者年月都显示,设置的年龄为0~10岁区间
    public int Gender;//性别--0为母，1为公
    public int BirthMonth;//生日的月份
    
    public int MinBreedAge;//最大繁殖年龄,必须成年才能繁殖
    public int LastBreedAge = 0;//上一次繁殖的年龄
    public int BreedInterval;//繁殖间隔
    public readonly float BreedProb = 0.2f;//繁殖概率，当Partner为null的时候为0，当有Partner的时候为0.2

    //自己控制的
    private Dictionary<string,float> DieProbDict=new Dictionary<string, float>();//死亡概率的字典
    // 计算当前总死亡概率（累乘法）
    public float TotalDeathProbability
    {
        get
        {
            if (DieProbDict.Count == 0) return 0f; // 没有死亡因素时概率为0

            float survival = 1f;
            foreach (var prob in DieProbDict.Values)
            {
                survival *= (1f - Mathf.Clamp01(prob)); // 确保概率在0~1之间
            }
            return 1f - survival;
        }
    }

    //显示相关
    [SerializeField] private SpriteRenderer Render;//显示
    [SerializeField] public int RoomID;//当前所在的房间号


    /// <summary>
    /// 初始化动物，可用于对战中随机一只动物的属性,-1代表随机男女
    /// </summary>
    public void Init(AnimalBaseData baseData,int gender=-1)
    {
        Gender = gender == -1 ? Random.Range(0, 2) : gender;
        Age = Random.Range(1,30);
        Debug.Log($"年龄为{Age}");
        DieProbDict["年龄"] = 0f;
        MinBreedAge = 24;
        BreedInterval = 5;
        if (Render == null)
        {
            Render = transform.Find("显示/本体").GetComponent<SpriteRenderer>();
        }
        BaseData = baseData;
        Render.sprite = Gender == 0 ? BaseData.FemaleSprite : BaseData.MaleSprite;
        AnimalManager.Instance.AllAnimals.Add(this);
    }
    /// <summary>
    /// 这是培育出来的动物初始化，遗传波动为20%，也就是说子女会在父母属性的基础上浮动上下20%
    /// </summary>
    /// <param name="father"></param>
    /// <param name="mother"></param>
    public void Init(Animal father, Animal mother)
    {
        Gender = Random.Range(0, 2); // 结果只可能是 0 或 1
        Debug.Log($"新生儿性别为{Gender}");
        Age = 1;
        DieProbDict["年龄"] = 0f;
        MinBreedAge = 24;
        BreedInterval = 5;
        if(Render==null)
        {
            Render = transform.Find("显示/本体").GetComponent<SpriteRenderer>();
        }
        BaseData=father.BaseData;
        Render.sprite=Gender==0? father.BaseData.FemaleSprite: father.BaseData.MaleSprite;
        AnimalManager.Instance. AllAnimals.Add(this);
    }

    //更新死亡概率字典
    public void UpdateDeathProb()
    {

        DieProbDict["年龄"] = Age <= 36 ? 0 : (Age - 36) * (0.1f)*0.01f; // 36个月之前不会因为年龄死亡,每个月增长0.1%的死亡率
        //DieProbDict["饥饿"] = Hunger * 0.5f;
        //DieProbDict["过度拥挤"] = Density * 0.05f;
    }

    /// <summary>
    /// 查看动物此时是不是应该死亡了
    /// </summary>
    /// <param name="cause"></param>
    /// <returns></returns>
    public bool TryDie(out string cause)
    {
        cause = null; //初始化
        UpdateDeathProb();

        // 累乘法计算总死亡概率

        if (Random.value < TotalDeathProbability)
        {
            // 按权重选择死亡原因
            float total = 0f;
            foreach (var prob in DieProbDict.Values)
                total += prob;

            float r = Random.value * total;
            float cumulative = 0f;
            foreach (var kvp in DieProbDict)
            {
                cumulative += kvp.Value;
                if (r <= cumulative)
                {
                    cause = kvp.Key;
                    break;
                }
            }

            // 如果意外没有选到（总概率=0），给一个默认值
            if (cause == null && DieProbDict.Count > 0)
                cause = DieProbDict.Keys.First();

            AnimalManager.Instance.AllAnimals.Remove(this);
            PoolManager.Instance.AnimalPool.Return(this);
            Debug.Log("animal死了");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 是否能繁殖
    /// </summary>
    /// <param name="currentMonth"></param>
    /// <returns></returns>
    public bool CanBreed()
    {
        
        if (Age<MinBreedAge) return false;//没到生育年龄
        if (GameManager.Instance.CurrentMonth - LastBreedAge < BreedInterval) return false;
        return true;
    }
    /// <summary>
    /// 判断当前在不在points内部
    /// </summary>
    /// <param name="p">要判断的点</param>
    /// <param name="poly">区域</param>
    /// <returns></returns>
    public bool IsPointInPolygon(Vector2 p, List<Vector3> poly)
    {
        bool inside = false;
        for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
        {
            Vector2 pi = poly[i];
            Vector2 pj = poly[j];

            bool intersect =
                ((pi.y > p.y) != (pj.y > p.y)) &&
                (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x);

            if (intersect)
                inside = !inside;
        }
        return inside;
    }

    /// <summary>
    /// 获取范围内一个随机的点
    /// </summary>
    /// <param name="poly"></param>
    /// <returns></returns>
    public Vector3 GetRandomPointInPolygon(List<Vector3> poly)
    {
        float minX = poly[0].x, maxX = poly[0].x;
        float minY = poly[0].y, maxY = poly[0].y;

        foreach (var p in poly)
        {
            minX = Mathf.Min(minX, p.x);
            maxX = Mathf.Max(maxX, p.x);
            minY = Mathf.Min(minY, p.y);
            maxY = Mathf.Max(maxY, p.y);
        }

        // 防止死循环，最多尝试 30 次
        for (int i = 0; i < 30; i++)
        {
            Vector3 candidate = new Vector3(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY),
                poly[0].z
            );

            if (IsPointInPolygon(candidate, poly))
                return candidate;
        }

        // 理论上很少走到这，兜底返回中心
        return poly[0];
    }

}
