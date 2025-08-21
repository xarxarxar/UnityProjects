using UnityEngine;

public class HPSliderFor2D : MonoBehaviour
{
    [SerializeField]private SpriteRenderer spriteRenderer;
    private Bindable<int> maxBlood = new Bindable<int>();
    private Vector2 spriteSize=Vector2.zero;
    private float defaultWidth = 0;
    private float currentBlood = 0;

    private void Start()
    {
        spriteSize = spriteRenderer.size;
        defaultWidth = spriteRenderer.size.x;
    }

    /// <summary>
    /// 初始化血条
    /// </summary>
    public void Init(Bindable<int> maxBlood,float currentBlood= -1)
    {
        spriteSize = spriteRenderer.size;
        defaultWidth = spriteRenderer.size.x;

        // 解绑旧事件
        if (this.maxBlood != null)
            this.maxBlood.OnValueChanged -= OnMaxHpChanged;

        // 替换引用并绑定新事件
        this.maxBlood = maxBlood;
        this.maxBlood.OnValueChanged += OnMaxHpChanged;

        if (currentBlood >= 0) 
        {
            SetBlood(currentBlood);
        }

    }


    /// <summary>
    /// 设置血量
    /// </summary>
    /// <param name="blood"></param>
    public void SetBlood(float blood)
    {
        blood = Mathf.Clamp(blood, 0, maxBlood.Value);

        currentBlood =blood;
        spriteSize.x= (blood/maxBlood.Value)*defaultWidth;
        spriteRenderer.size= spriteSize;
    }

    private void OnMaxHpChanged(int maxHp)
    {
        //更新血条
        SetBlood(currentBlood);
    }
}
