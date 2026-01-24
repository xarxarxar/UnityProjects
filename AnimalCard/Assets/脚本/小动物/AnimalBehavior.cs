using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 控制动物行为，比如每天走来走去之类的
/// </summary>
public class AnimalBehavior : MonoBehaviour
{
    public List<Vector3> points;
    public List<Transform> pointsTransform;
    public Transform Show;       // 显示的模型/精灵
    public float moveSpeed = 2f; // 速度，单位世界单位每秒
    public float walkBobHeight = 0.1f; // 上下晃动幅度
    public float walkBobFrequency = 2f; // 上下晃动频率
    public float MoveInterval;//行动间隔

    private Vector3 _oriPos;//拖拽前的初始位置
    [SerializeField]private bool isMoving=false;
    [SerializeField]private bool isDragging=false;
    private Coroutine MoveCoro;
    private Tween _moveTween;
    private  SortingGroup sortingGroup;

    //数据
    private Animal _animal;
    private TouchHandler2D _touchHandler;


    public void Init()
    {
        isMoving = false;
        isDragging=false;
        if (sortingGroup == null)
            sortingGroup = GetComponent<SortingGroup>();
        if (_animal == null)
        {
            _animal = GetComponent<Animal>();
        }
        if (_touchHandler == null)
        {
            _touchHandler = GetComponent<TouchHandler2D>();
        }

        points.Clear();
        
        for (int i = 0; i < RoomManager.instance.GetRoom(_animal.RoomID).Bounds.Count; i++)
        {
            points.Add(RoomManager.instance.GetRoom(_animal.RoomID).Bounds[i]);
        }

        if (MoveCoro != null)
        {
            StopCoroutine(MoveCoro);
            MoveCoro = null;
        }
        
        MoveCoro = StartCoroutine(KeepMoving());
        //GameManager.OnMonthChanged -= OnMonthChanged;
        //GameManager.OnMonthChanged += OnMonthChanged;

        _touchHandler.OnDragBegin -= OnBeginDrag;
        _touchHandler.OnDragBegin += OnBeginDrag;
        _touchHandler.OnDragging -= OnDragging;
        _touchHandler.OnDragging += OnDragging;
        _touchHandler.OnDragEnd -= OnEndDrag;
        _touchHandler.OnDragEnd += OnEndDrag;
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            SetAnimalRoom(_animal.RoomID  == -1 ? 0 : -1) ;
        }
    }
    //设置该动物的房间
    private void SetAnimalRoom(int id)
    {
        _animal.RoomID = id;
        points.Clear();
        for (int i = 0; i < RoomManager.instance.GetRoom(_animal.RoomID).Bounds.Count; i++)
        {
            points.Add(RoomManager.instance.GetRoom(_animal.RoomID).Bounds[i]);
        }
        if (!_animal.IsPointInPolygon(transform.position,points))//不在内部，就移动过去
        {
            SingleMove();
        }
        
    }

    void LateUpdate()
    {
        // 示例：x靠右，y靠下越大，Order in Layer 越高
        int order = Mathf.RoundToInt(transform.position.y * -100 + transform.position.x * 100);
        sortingGroup.sortingOrder = order;
    }
    //单次移动
    private void SingleMove()
    {
        if(isDragging || isMoving)
        {
            return;
        }
        Vector3 target =_animal.GetRandomPointInPolygon(points);
        MoveTo(target);
    }

    //月份变化，增加年龄,并检测是否死亡
    private void OnMonthChanged()
    {
        _animal.Age++;
        if (_animal.Age % 5 == 0)//每五个月调用一次死亡的逻辑，要不然死亡的概率太高了
        {
            if (_animal.TryDie(out var cause))
            {
                Debug.Log($"死亡。年龄为{_animal.Age}个月,死因为{cause},死亡概率为{_animal.TotalDeathProbability}");
                Destroy(gameObject);
            }
        }
    }

    //移动到某个位置，用dotween实现
    private void MoveTo(Vector3 target)
    {
        _moveTween?.Kill();

        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(startPos, target);
        float duration = distance / moveSpeed;

        isMoving = true;

        _moveTween = DOTween.To(() => 0f, t =>
        {
            transform.position = Vector3.Lerp(startPos, target, t);

            // 上下轻微晃动（只管表现）
            float bobAngle = 5f;
            Show.localRotation = Quaternion.Euler(
                0, 0, Mathf.Sin(Time.time * 10f) * bobAngle
            );
        }, 1f, duration)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            isMoving = false;
            Show.localRotation = Quaternion.identity;
        });
    }

    //快速移动
    private void MoveToQuickly(Vector3 target)
    {
        _moveTween?.Kill();

        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(startPos, target);
        float duration = distance / (moveSpeed*5);

        isMoving = true;

        _moveTween = DOTween.To(() => 0f, t =>
        {
            transform.position = Vector3.Lerp(startPos, target, t);

            // 上下轻微晃动（只管表现）
            float bobAngle = 5f;
            Show.localRotation = Quaternion.Euler(
                0, 0, Mathf.Sin(Time.time * 10f) * bobAngle
            );
        }, 1f, duration)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            isMoving = false;
            Show.localRotation = Quaternion.identity;
        });
    }

    private void OnDestroy()
    {
        //GameManager.OnMonthChanged -= OnMonthChanged;
    }

    // 处理拖拽（带参数）
    private void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        isDragging=true;
        sortingGroup.sortingLayerName = "Dragging";
        _moveTween?.Kill();
        _oriPos =transform.position;//记录初始位置
    }

    private void OnDragging(UnityEngine.EventSystems.PointerEventData eventData)
    {
        // 直接使用 eventData.position 进行坐标转换和位移
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPos.z = _oriPos.z;
        transform.position = worldPos;
    }
    private void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        isDragging=false;
        sortingGroup.sortingLayerName = "Animal";
        Vector3 pos = Camera.main.ScreenToWorldPoint(eventData.position);
        pos.z = _oriPos.z;
        if (RoomManager.instance.IsInRoom(pos,out int roomID))
        {
            transform.position= pos;
            SetAnimalRoom(roomID);
        }
        else
        {
            MoveToQuickly(_oriPos);
        }
    }


    private IEnumerator KeepMoving()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(MoveInterval);
            SingleMove();
            yield return new WaitUntil(() => !isMoving);
            yield return new WaitForSecondsRealtime(MoveInterval);
        }
    }
}
