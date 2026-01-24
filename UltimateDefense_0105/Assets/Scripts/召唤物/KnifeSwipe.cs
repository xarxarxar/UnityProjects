using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 水果忍者的小刀
/// </summary>
public class KnifeSwipe : MonoBehaviour
{
    private Camera cam;
    private TrailRenderer trail;

    private float _duration;//持续时长
    public event UnityAction OnNinjaEnd;//水果忍者结束之后
    public static int Count = 0;// 调用这个的数量，只有为0时才能消失

    void Awake()
    {
        cam = Camera.main;
        trail = GetComponent<TrailRenderer>();
        trail.emitting = false;
    }
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="duration"></param>
    public void Init(float duration)
    {
        Count += 1;
        _duration = duration;
        gameObject.SetActive(true);
        TimerUtility.Instance.Timer(duration, () => 
        {
            if (Count >= 1)
            {
                Count -= 1;
            }
            if (Count == 0)
            {
                gameObject.SetActive(false);
            }
            OnNinjaEnd?.Invoke();
        });
    }


    void Update()
    {
#if UNITY_EDITOR
        HandleMouse();
#else
        HandleTouch();
#endif
    }

    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
            StartSwipe(Input.mousePosition);

        if (Input.GetMouseButton(0))
            UpdatePosition(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
            EndSwipe();
    }

    void HandleTouch()
    {
        if (Input.touchCount <= 0) return;

        Touch t = Input.GetTouch(0);

        if (t.phase == TouchPhase.Began)
            StartSwipe(t.position);

        if (t.phase == TouchPhase.Moved)
            UpdatePosition(t.position);

        if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            EndSwipe();
    }

    void StartSwipe(Vector2 screenPos)
    {
        // 1先把刀移动到当前手指位置
        MoveTo(screenPos);

        // 2清空历史轨迹
        trail.Clear();

        // 3再开始绘制
        trail.emitting = true;

        //lastPos = transform.position;
    }

    void UpdatePosition(Vector2 screenPos)
    {
        Vector3 worldPos = cam.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, 10f)
        );
        transform.position = worldPos;
    }

    void EndSwipe()
    {
        trail.emitting = false;
    }

    void MoveTo(Vector2 screenPos)
    {
        Vector3 worldPos = cam.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, 10f)
        );
        transform.position = worldPos;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(false,TowerManager.Instance.CurrentTower.BaseDamage*2);
            //CameraShake();
        }
    }


}
