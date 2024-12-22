using DG.Tweening;
using UnityEngine;

public class GuideLine : MonoBehaviour
{
    // 用于绘制轨迹线的材质
    public Material lineMat;

    // 四个 LineRenderer 组件，分别用于绘制四条轨迹线
    public LineRenderer lr0;
    public LineRenderer lr1;
    public LineRenderer lr2;
    public LineRenderer lr3;
    // 轨迹线的最大距离
    public float distance = 10f;
    // 用于控制射线碰撞检测的不同图层（LayerMask），用于过滤碰撞对象
    private LayerMask _mask0;
    private LayerMask _mask1;
    private LayerMask _mask2;
    private LayerMask _mask3;
    LayerMask _hit2Mask;
    // 触发条件的位置，用于控制射线的碰撞逻辑
    private float _triggerPos = 0.5f;
    // 存储控制射线的“点”对象（Dots）
    public Transform[] dots;
    // 是否显示射击轨迹
    public bool isLine = false;


    #region // 发射引导线关闭

    // 关闭射击引导线的显示
    public void GuidelineOff()
    {
        transform.GetChild(0).gameObject.SetActive(false);// 隐藏引导线
    }

    #endregion

    #region // 发射引导线开启
    // 开启射击引导线的显示
    public void GuidelineOn()
    {
        transform.GetChild(0).gameObject.SetActive(true);// 显示引导线
    }

    #endregion
    // 初始化方法，设置初始状态
    void Awake()
    {
        // 禁用 lr2 和 lr3，表示初始时不显示这两条轨迹
        lr2.enabled = false;
        lr3.enabled = false;
        // 设置不同图层的碰撞层，控制射线碰撞的范围
        _mask0 = (1 << LayerMask.NameToLayer("RayTriggerL")) |
                (1 << LayerMask.NameToLayer("RayTriggerT") |
                 (1 << LayerMask.NameToLayer("RayTriggerR"))); //| (1 << LayerMask.NameToLayer("Block"))));
        _mask1 = (1 << LayerMask.NameToLayer("RayTriggerT")) |
                (1 << LayerMask.NameToLayer("RayTriggerL")); //| (1 << LayerMask.NameToLayer("Block")));
        _mask2 = (1 << LayerMask.NameToLayer("RayTriggerT")) |
                (1 << LayerMask.NameToLayer("RayTriggerR")); //| (1 << LayerMask.NameToLayer("Block")));
        _mask3 = (1 << LayerMask.NameToLayer("RayTriggerL")) |
                (1 << LayerMask.NameToLayer("RayTriggerR")); //| (1 << LayerMask.NameToLayer("Block")));
        // 初始化 LineRenderer 的位置点数量
        lr0.positionCount = 4;
        lr1.positionCount = 2;
        lr2.positionCount = 2;
        lr3.positionCount = 2;
    }


    //public void SetRayTrigger (float pos) {
    //    triggerPos = ball.radius;
    //    top.transform.DOLocalMoveY(-pos*0.5f, 0f).SetRelative(true);
    //    left.transform.DOLocalMoveX(pos*0.5f, 0f).SetRelative(true);
    //    right.transform.DOLocalMoveX(-pos*0.5f, 0f).SetRelative(true);
    //}

    bool isOff = false;
    // 每帧的物理更新，计算射线的反射与轨迹线的绘制
    void FixedUpdate()
    {
        if (isOff) return;

        //Material Anim
        //float offset = Time.time * -3f;
        //liseMat.mainTextureScale = new Vector3(3f, 1, 1);
        //liseMat.SetTextureOffset("_MainTex", new Vector2(offset, 0f));

        // 获取当前对象的 Transform 信息
        var myTransform = transform;
        var myTransformPosition = myTransform.position;
        var up = myTransform.up;// 获取物体的上方向
        Vector2 dir = (myTransformPosition - (up * -distance));// 计算射线的初始方向

        // 使用 Physics2D.Raycast 发射第一条射线，检测碰撞
        RaycastHit2D hit = Physics2D.Raycast(myTransformPosition, up, distance, _mask0);

        // 如果射线碰撞到了物体
        if (hit)
        {
            var position = transform.position;
            Vector2 reflectFirstPos = Vector2.Reflect(hit.point - (Vector2)position, hit.normal);
            Vector2 firstPosition = hit.point;// 第一个碰撞点

            // 计算第一条射线的距离
            float raydistance = ((Vector2)position - hit.point).magnitude;
            float resultRay = distance - raydistance;

            // 更新第一条反射线的位置
            firstPosition += (reflectFirstPos.normalized * resultRay);

            // 根据碰撞的物体，选择适当的 LayerMask
            if (hit.collider.name != "TriggerReal T")
            {
                if (hit.point.x > 0)
                {
                    //resultPos.x = resultPos.x - 0.1f;
                    _hit2Mask = _mask1;
                }
                else
                {
                    //resultPos.x = resultPos.x + 0.1f;
                    _hit2Mask = _mask2;
                }
            }
            else
            {
                if (hit.collider.CompareTag("Block"))
                {
                    _hit2Mask = _mask0;
                }
                else
                {
                    _hit2Mask = _mask3;
                }
            }
            // 射击第二条射线，进行进一步碰撞检测
            RaycastHit2D hit2 = Physics2D.Raycast(hit.point, reflectFirstPos, resultRay, _hit2Mask);
            if (hit2)
            {
                // 计算第二次碰撞的反射方向
                float raydistance2 = (hit.point - hit2.point).magnitude;
                Vector2 reflectSecondPos = Vector2.Reflect(hit2.point - hit.point, hit2.normal);
                Vector2 secondPosition = hit2.point;
                float distanceRay2 = (resultRay - raydistance2);
                secondPosition += reflectSecondPos.normalized * distanceRay2;
                // 更新 LineRenderer 的绘制模式
                //LineMode2(hit.point, hit2.point, secondPosition);
                LineMode1(hit.point, hit2.point);
            }
            else
            {
                LineMode1(hit.point, firstPosition);
            }

        }
        else
        {
            // 如果射线没有碰撞，绘制默认的轨迹
            LineMode0(dir);
        }
    }


    // 第一种模式：射线没有碰撞，直接绘制到最大距离
    void LineMode0(Vector3 dir)
    {
        if (_modecount != 0)
        {
            _modecount = 0;

            if (!isLine) LineColor(0);

            lr0.positionCount = 4;
            lr1.positionCount = 2;
            lr2.positionCount = 0;
            lr3.positionCount = 0;
        }

        if (dots[0].gameObject.activeSelf) dots[0].gameObject.SetActive(false);
        if (dots[1].gameObject.activeSelf) dots[1].gameObject.SetActive(false);

        var position = transform.position;
        lr0.SetPosition(0, position);
        lr0.SetPosition(1, dir);
        lr0.SetPosition(2, dir);
        lr0.SetPosition(3, dir);

        lr1.SetPosition(0, position);
        lr1.SetPosition(1, dir);
    }

    // 第二种模式：射线碰撞，绘制反射轨迹
    void LineMode1(Vector3 hit, Vector3 dir)
    {
        if (_modecount != 1)
        {
            _modecount = 1;
            if (!isLine) LineColor(1);
            lr0.positionCount = 3;
            lr1.positionCount = 2;
            lr2.positionCount = 2;
            lr3.positionCount = 0;
        }

        lr0.SetPosition(0, transform.position);
        lr0.SetPosition(1, hit);
        lr0.SetPosition(2, dir);

        if (!dots[0].gameObject.activeSelf) dots[0].gameObject.SetActive(true);
        if (dots[1].gameObject.activeSelf) dots[1].gameObject.SetActive(false);

        dots[0].transform.position = hit;


        lr1.SetPosition(0, transform.position);
        lr1.SetPosition(1, hit);
        lr2.SetPosition(0, hit);
        lr2.SetPosition(1, dir);
    }

    // 第三种模式：多次碰撞，绘制多次反射轨迹
    int _modecount = -1;

    void LineMode2(Vector3 hit1, Vector3 hit2, Vector3 dir)
    {
        if (_modecount != 2)
        {
            _modecount = 2;
            lr0.positionCount = 4;
            lr1.positionCount = 2;
            lr2.positionCount = 2;
            lr3.positionCount = 2;
        }

        if (!isLine) LineColor(2);

        lr0.SetPosition(0, transform.position);
        lr0.SetPosition(1, hit1);
        lr0.SetPosition(2, hit2);
        lr0.SetPosition(3, dir);

        if (!dots[0].gameObject.activeSelf) dots[0].gameObject.SetActive(true);
        if (!dots[1].gameObject.activeSelf) dots[1].gameObject.SetActive(true);

        dots[0].transform.position = hit1;
        dots[1].transform.position = hit2;

        lr1.SetPosition(0, transform.position);
        lr1.SetPosition(1, hit1);
        lr2.SetPosition(0, hit1);
        lr2.SetPosition(1, hit2);
        lr3.SetPosition(0, hit2);
        lr3.SetPosition(1, dir);
    }


    #region - LinerAlpha
    // 控制轨迹线的透明度和渐变
    int _lineType = -1;
    // 更新轨迹线的渐变效果
    void LineColor(int type = 0)
    {
        if (_lineType == type) return;

        _lineType = type;

        if (type == 0)
        {
            Gradient gradient = lr1.colorGradient;
            GradientAlphaKey[] alpha = gradient.alphaKeys;

            alpha[0].alpha = 1f;
            alpha[0].time = 0f;
            alpha[1].alpha = 0f;
            alpha[1].time = 1f;

            gradient.alphaKeys = alpha;
            lr1.colorGradient = gradient;

            //width
            //AnimationCurve curve = new AnimationCurve();
            //curve.AddKey(0.0f, 1.0f); curve.AddKey(1.0f, 0.0f);
            //lr1.widthCurve = curve;

        }
        else if (type == 1)
        {
            Gradient gradient = lr1.colorGradient;
            GradientAlphaKey[] alpha = gradient.alphaKeys;
            alpha[0].alpha = 1f;
            alpha[0].time = 0f;
            alpha[1].alpha = 0.8f;
            alpha[1].time = 1f;

            gradient.alphaKeys = alpha;
            lr1.colorGradient = gradient;

            alpha[0].alpha = 0.8f;
            alpha[0].time = 0f;
            alpha[1].alpha = 0f;
            alpha[1].time = 1f;

            gradient.alphaKeys = alpha;
            lr2.colorGradient = gradient;

            //width
            //AnimationCurve curve1 = new AnimationCurve();
            //curve1.AddKey(0.0f, 1.0f); curve1.AddKey(0.0f, 1.0f);
            //lr1.widthCurve = curve1;

            //AnimationCurve curve2 = new AnimationCurve();
            //curve2.AddKey(0.0f, 1.0f); curve2.AddKey(1.0f, 0.0f);
            //lr2.widthCurve = curve2;

        }
        else if (type == 2)
        {
            Gradient gradient = lr1.colorGradient;
            GradientAlphaKey[] alpha = gradient.alphaKeys;
            alpha[0].alpha = 1f;
            alpha[0].time = 0f;
            alpha[1].alpha = 1f;
            alpha[1].time = 1f;

            gradient.alphaKeys = alpha;
            lr1.colorGradient = gradient;

            alpha[0].alpha = 1f;
            alpha[0].time = 0f;
            alpha[1].alpha = 0.6f;
            alpha[1].time = 1f;

            gradient.alphaKeys = alpha;
            lr2.colorGradient = gradient;

            alpha[0].alpha = 0.6f;
            alpha[0].time = 0f;
            alpha[1].alpha = 0f;
            alpha[1].time = 1f;

            gradient.alphaKeys = alpha;
            lr3.colorGradient = gradient;

            //width
            //AnimationCurve curve1 = new AnimationCurve();
            //curve1.AddKey(0.0f, 1.0f); curve1.AddKey(1.0f, 1.0f);
            //lr1.widthCurve = curve1;

            //AnimationCurve curve2 = new AnimationCurve();
            //curve2.AddKey(0.0f, 1.0f); curve2.AddKey(1.0f, 1.0f);
            //lr2.widthCurve = curve2;

            //AnimationCurve curve3 = new AnimationCurve();
            //curve3.AddKey(0.0f, 1.0f); curve3.AddKey(1.0f, 0.0f);
            //lr3.widthCurve = curve2;
        }
    }

    #endregion

}



