using UnityEngine;
using DG.Tweening;

public class CameraViewController : MonoBehaviour
{
    [Header("Camera")]
    public Camera cam;

    [Header("Size")]
    public float bigSize = 10f;
    public float smallSize = 5f;

    [Header("Tween")]
    public float tweenDuration = 0.4f;
    public Ease tweenEase = Ease.OutCubic;

    [Header("Drag")]
    public float dragSpeed = 1f;

    //[Header("World Bounds")]
    //最小就是左下角
    //public Vector2 minLimit=> new Vector3(TileManager.Instance.MainBound[2].x - 2, TileManager.Instance.MainBound[2].y - 2, TileManager.Instance.MainBound[0].z);
    public Vector2 minLimit;
    //最大就是右上角
    public Vector2 maxLimit;
    //public Vector2 maxLimit=> new Vector3(TileManager.Instance.MainBound[1].x + 2, TileManager.Instance.MainBound[1].y + 2, TileManager.Instance.MainBound[0].z);

    private Vector3 lastMousePos;
    [SerializeField]private bool canDrag = false;
    private bool isSmallView = false;
    private bool isTweening = false;

    private Tween sizeTween;
    private Tween moveTween;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            ToggleView();
        }
        if (canDrag && !isTweening)
            HandleDrag();
    }

    #region Toggle

    public void ToggleView()
    {
        if (isTweening) return;

        if (isSmallView)
            SwitchToBig();
        else
            SwitchToSmall();
    }

    void SwitchToBig()
    {
        isTweening = true;
        canDrag = false;
        isSmallView = false;

        KillTweens();

        sizeTween = cam.DOOrthoSize(bigSize, tweenDuration)
            .SetEase(tweenEase);

        moveTween = transform.DOMove(Vector3.zero, tweenDuration)
            .SetEase(tweenEase)
            .OnComplete(() =>
            {
                isTweening = false;
            });
    }

    void SwitchToSmall()
    {
        isTweening = true;
        canDrag = false;
        isSmallView = true;

        KillTweens();

        sizeTween = cam.DOOrthoSize(smallSize, tweenDuration)
            .SetEase(tweenEase);

        moveTween = transform.DOMove(Vector3.zero, tweenDuration)
            .SetEase(tweenEase)
            .OnComplete(() =>
            {
                isTweening = false;
                canDrag = true;
            });
    }

    void KillTweens()
    {
        sizeTween?.Kill();
        moveTween?.Kill();
    }

    #endregion

    #region Drag

    void HandleDrag()
    {
        if (!TryGetDragDelta(out Vector2 delta))
            return;

        float factor = cam.orthographicSize * 2f / Screen.height;

        transform.position += new Vector3(
            -delta.x * factor,
            -delta.y * factor,
            0f
        ) * dragSpeed;

        transform.position = ClampPosition(transform.position);
    }

    Vector3 ClampPosition(Vector3 pos)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        // X 方向
        float minX = minLimit.x + camWidth;
        float maxX = maxLimit.x - camWidth;

        if (minX > maxX)
        {
            // 世界比相机还小，锁死到中心
            pos.x = (minLimit.x + maxLimit.x) * 0.5f;
        }
        else
        {
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
        }

        // Y 方向
        float minY = minLimit.y + camHeight;
        float maxY = maxLimit.y - camHeight;

        if (minY > maxY)
        {
            pos.y = (minLimit.y + maxLimit.y) * 0.5f;
        }
        else
        {
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }

        return pos;
    }

    bool TryGetDragDelta(out Vector2 delta)
    {
        delta = Vector2.zero;

#if UNITY_EDITOR || UNITY_STANDALONE
        // ===== PC / 编辑器：鼠标 =====
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
            return false;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 current = Input.mousePosition;
            delta = current - lastMousePos;
            lastMousePos = current;
            return delta.sqrMagnitude > 0.01f;
        }

#elif UNITY_WEBGL || UNITY_IOS || UNITY_ANDROID
    // ===== 手机 / 微信小游戏：单指拖拽 =====
    if (Input.touchCount == 1)
    {
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            lastMousePos = touch.position;
            return false;
        }

        if (touch.phase == TouchPhase.Moved)
        {
            delta = touch.position - (Vector2)lastMousePos;
            lastMousePos = touch.position;
            return delta.sqrMagnitude > 0.01f;
        }
    }
#endif

        return false;
    }


    #endregion
}
