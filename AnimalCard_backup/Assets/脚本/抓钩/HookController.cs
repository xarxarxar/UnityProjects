using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class HookController : MonoBehaviour
{
    [Header("Refs")]
    public Transform hook;                 // 抓钩（带 Collider2D / Trigger）
    public SpriteRenderer ropeRenderer;    // 绳子（DrawMode = Tiled）
    public Sprite CloseHook;
    public Sprite OpenHook;

    public Transform hookParent;
    [Header("Rotate")]
    public float rotateAngle = 30f;
    public float rotateDuration = 1.2f;

    private Tween _rotateTween;
    private bool _isLaunching;

    [Header("Config")]
    public float extendSpeed = 6f;
    public float retractSpeed = 8f;
    public float maxLength = 8f;

    private Vector2 _direction;
    [HideInInspector]public  bool _extending;
    [HideInInspector] public bool _retracting;
    [HideInInspector] public FlyingObject _caughtTarget;
    private SpriteRenderer _hookRender;

    // 初始状态下 ropeRoot → hook 的距离（非常关键）
    private float _initialDistance;

    public event UnityAction<SingleCard> OnCatchCard;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        // 记录“静止状态”下绳子的真实长度
        _initialDistance = hook.localPosition.magnitude;
        ResetHook();

        hook.GetComponent<Hook>().OnCatchFlyObj -= OnCatchFlyObj;
        hook.GetComponent<Hook>().OnCatchFlyObj += OnCatchFlyObj;
        StartRotate();
    }

    private void Update()
    {

        if (_extending)
            Extend();

        if (_retracting)
            Retract();

        UpdateRopeVisual();
    }

    #region Launch

    public void Launch()
    {
        if (_extending || _retracting)
            return;

        if (_isLaunching)
            return;

        _isLaunching = true;
        // 停止旋转，但保留当前角度
        _rotateTween?.Kill(false);
        _rotateTween = null;

        // 使用 hook 的本地 up 方向（支持旋转）
        _direction = Vector3.up;

        Debug.Log($"_direction is {_direction}");
        //Time.timeScale = 0;

        _extending = true;
        _retracting = false;
        _caughtTarget = null;
        if (_hookRender == null)
        {
            _hookRender = hook.GetComponent<SpriteRenderer>();
        }
        SetHookSprite(true); //发射时张开
    }
    /// <summary>
    /// 设置钩子的开合状态，true为开，false为闭
    /// </summary>
    /// <param name="state"></param>
    public void SetHookSprite(bool state)
    {
        if (state)
        {
            _hookRender.sprite = OpenHook;
        }
        else
        {
            _hookRender.sprite = CloseHook;
        }
    }

    #endregion

    #region Extend / Retract

    /// <summary>
    /// 待机左右摆动
    /// </summary>
    void StartRotate()
    {
        float currentZ = NormalizeAngle(hookParent.localEulerAngles.z);

        _rotateTween?.Kill(false);

        _rotateTween = DOTween.Sequence()
            .Append(
                hookParent.DOLocalRotate(
                    new Vector3(0, 0, rotateAngle),
                    rotateDuration * Mathf.Abs((rotateAngle - currentZ) / (rotateAngle * 2f))
                ).SetEase(Ease.Linear)
            )
            .Append(
                hookParent.DOLocalRotate(
                    new Vector3(0, 0, -rotateAngle),
                    rotateDuration
                ).SetEase(Ease.Linear)
            )
            .Append(
                hookParent.DOLocalRotate(
                    new Vector3(0, 0, rotateAngle),
                    rotateDuration
                ).SetEase(Ease.Linear)
            )
            .SetLoops(-1);
    }

    private void Extend()
    {
        hook.localPosition += (Vector3)(_direction * extendSpeed * Time.deltaTime);

        float currentDistance = hook.localPosition.magnitude;
        float extraLength = currentDistance - _initialDistance;

        if (extraLength >= maxLength)
        {
            SetHookSprite(false);   //回到原点再张开
            StartRetract();
        }
    }

    /// <summary>
    /// 钩子回到原点后调用
    /// </summary>
    public void OnReturnToOrigin()
    {
        _isLaunching = false;

        // 从当前角度继续摆动
        StartRotate();
    }
    /// <summary>
    /// 将 0~360 转为 -180~180
    /// </summary>
    float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;
        return angle;
    }

    private void Retract()
    {
        hook.localPosition = Vector3.MoveTowards(
            hook.localPosition,
            Vector3.zero,
            retractSpeed * Time.deltaTime
        );

        if (_caughtTarget != null)
        {
            UpdateCaughtTargetPosition();
        }

        if (hook.localPosition.magnitude <= _initialDistance + 0.01f)
        {
            SetHookSprite(true);   //回到原点再张开
            OnReturnToOrigin();//继续旋转
            ResetHook();
        }
    }

    private void StartRetract()
    {
        _extending = false;
        _retracting = true;
    }

    private void ResetHook()
    {
        _extending = false;
        _retracting = false;

        hook.localPosition = Vector3.up * _initialDistance;

        if (_caughtTarget != null)
        {
            _caughtTarget.ReturnPool();
            if (_caughtTarget.GetComponent<SingleCard>())
            {
                OnCatchCard?.Invoke(_caughtTarget.GetComponent<SingleCard>());
            }
        }

        _caughtTarget = null;

        UpdateRopeVisual();
    }

    #endregion

    #region Rope Visual（核心逻辑）

    private void UpdateRopeVisual()
    {
        float currentDistance = hook.localPosition.magnitude;

        // 只取“比初始多出来的部分”
        float extraLength = Mathf.Max(0f, currentDistance - _initialDistance);

        SetRopeLength(extraLength);
        UpdateRopeRotation();
    }

    private void SetRopeLength(float extraLength)
    {
        float finalLength = _initialDistance + extraLength;

        // DrawMode.Tiled 下修改 size
        ropeRenderer.size = new Vector2(
            ropeRenderer.size.x,
            finalLength
        );

        // 2计算 ropeRoot → hook 的【局部方向】
        Vector3 dir = hook.localPosition;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        dir.Normalize();

        // 3绳子中心点 = 沿着方向走一半长度
        ropeRenderer.transform.localPosition =
            dir * (finalLength * 0.5f);
    }

    private void UpdateRopeRotation()
    {
        Vector3 dir = hook.localPosition;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        ropeRenderer.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    #endregion

    #region Catch
    private void OnCatchFlyObj(FlyingObject flyingObject)
    {
        if (!_extending || _caughtTarget != null)
            return;
        if (flyingObject == null) return;
        SetHookSprite(false); //抓到的一瞬间闭合
        _caughtTarget= flyingObject;
        _caughtTarget.OnCaught();
        StartRetract();
    }
    //更新钩子和抓住的物体之间的偏差
    private void UpdateCaughtTargetPosition()
    {
        Vector3 dir = -_direction;
        if (dir.sqrMagnitude < 0.0001f)
            return;

        dir.Normalize();

        Vector3 offset = dir * 0.5f;
        _caughtTarget.transform.position = hook.position + offset;
    }


    #endregion
}
