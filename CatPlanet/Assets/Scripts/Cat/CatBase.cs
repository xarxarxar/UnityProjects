using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting.APIUpdating;

public class CatBase : MonoBehaviour
{
    public bool CatGender = false;//false为男，true为女
    public bool IsMarried=false;//是否已婚
    public int age = 1;//年龄
    public int lastProcreateAge = 0;//生育年龄

    public static UnityAction<CatBase> OnCatDie;//猫咪死亡

    private float _moveProb=0.3f;//移动的概率

    private void OnEnable()
    {
        PlanetManager.CurrentYear.OnValueChanged += OnYearChanged;
        StartCoroutine(LiveCoro());
    }

    private void OnDisable()
    {
        PlanetManager.CurrentYear.OnValueChanged -= OnYearChanged;
    }

    /// <summary>
    /// 初始化猫咪，此方法用于生育出的猫咪
    /// </summary>
    /// <param name="gender"></param>
    public void Init(bool gender)
    {
        CatGender=gender;
        if (gender)
        {
            GetComponent<SpriteRenderer>().color=Color.blue;//男性
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.red;//女性
        }
        IsMarried = false;
        age = 1;
        transform.localScale= Vector3.one/2;
    }

    /// <summary>
    /// 初始化猫咪，此方法用于关卡开始时默认存在的猫咪
    /// </summary>
    /// <param name="gender"></param>
    public void Init(bool gender,int age)
    {
        CatGender = gender;
        if (gender)
        {
            GetComponent<SpriteRenderer>().color = Color.blue;//男性
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.red;//女性
        }
        IsMarried = false;
        this.age = age;
        if (this.age >= 18)
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            transform.localScale = Vector3.one/2;
        }
    }


    //生活，主要是结婚和生子
    private IEnumerator LiveCoro()
    {
        while(true)
        {
            yield return new WaitForSeconds(PlanetManager.timeDelta);
            if (PlanetManager.IsEventHappen(_moveProb))
            {
                Move();
            }
        }
    }

    private void Move()
    {
        // 随机方向向量，范围 -1~1
        float dx = Random.Range(-1f, 1f);
        float dy = Random.Range(-1f, 1f);
        Vector3 direction = new Vector3(dx, dy, 0).normalized; // 归一化为单位向量

        // 当前的位置
        Vector3 pos = transform.position;

        // 目标位置，移动1单位
        Vector3 targetPos = pos + direction;

        // 限制范围
        targetPos.x = Mathf.Clamp(targetPos.x, -20f, 20f);
        targetPos.y = Mathf.Clamp(targetPos.y, -12f, 12f);

        // 应用移动
        transform.position = targetPos;
    }

    //是否死亡
    private void IsDie()
    {
        float dieProb = 0;
        if (age >= 60)
        {
            dieProb = PlanetManager.dieProb + (age - 60) * PlanetManager.dieIncreaseProb;
        }
        else
        {
            dieProb = PlanetManager.dieProb;
        }
        
        dieProb = Mathf.Clamp(dieProb, 0f, 0.9999f);
        if (PlanetManager.IsEventHappen(dieProb))//若死亡
        {
            OnCatDie?.Invoke(this);
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    private void OnYearChanged(int year)
    {
        age++;
        if(age >= 18)
        {
            transform.localScale = Vector3.one;
        }
    }
}
