using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance = null;

    public SpriteDuplicator parallaxLayer1;
    public SpriteDuplicator parallaxLayer2;

    public Transform farest;

    private float camMinX, camMaxX;

    private Vector3 oldPosition;

    private Vector3 dragOrigin;//, lastMousePos;
    private DateTime lastTimeDrag;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        var delTaX = transform.position.x - GameManager.instance.myGate.position.x;
        camMinX = transform.position.x;
        camMaxX = GameManager.instance.enemyGate.position.x - delTaX;
    }

    public void Restart()
    {
        transform.position = new Vector3(camMinX, transform.position.y, transform.position.z);

        oldPosition = transform.position;

        FarestFollowCam();

        parallaxLayer1.ReStart();
        parallaxLayer2.ReStart();
    }

    void Update()
    {


        // if (Application.isEditor)
        // {

        // }
        // else
        // {
        //     ProcessTouch();
        // }
    }

    void LateUpdate()
    {
        var delDragTime = DateTime.Now - lastTimeDrag;

        if (!GameManager.instance.IsPlaying()) return;

        ProcessMouseInput();

        if (GameManager.instance.IsPlaying() && delDragTime.TotalSeconds >= 1f)
        {
            var allEntities = GameManager.instance.AllEntities;
            var keys = allEntities.Keys;
            var maxX = float.MinValue;
            EntityBase entity = null;
            for (int i = 0; i < keys.Count; i++)
            {
                var entityTmp = allEntities[keys.ElementAt(i)];
                if (entityTmp != null && entityTmp.currentTeam == Team.My)
                {
                    if (maxX < entityTmp.transform.position.x)
                    {
                        maxX = entityTmp.transform.position.x;
                        entity = entityTmp;
                    }
                }
            }

            Vector3 target = transform.position;
            if (entity != null)
            {
                var nextX = entity.transform.position.x + 2;
                target = new Vector3(nextX, target.y, target.z);

                if (target.x <= camMinX)
                    target.x = camMinX;
                if (target.x >= camMaxX)
                    target.x = camMaxX;
            }

            if (target != transform.position)
                transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime);

            MoveLayers();

            // Debug.Log(oldPosition + " curPos: " + transform.position + " delta:" + (transform.position.x - oldPosition.x));

            oldPosition = transform.position;
        }
    }

    private void FarestFollowCam()
    {
        var farestTmp = farest.position;
        farestTmp.x = transform.position.x;
        farest.position = farestTmp;
    }

    private void MoveLayers()
    {
        FarestFollowCam();

        var delX = transform.position.x - oldPosition.x;
        if (Mathf.Abs(delX) > 0.005f)
        {
            parallaxLayer1.Move(delX);
            parallaxLayer2.Move(delX);
        }
    }

    float countTime = 0f;

    void ProcessMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            // Debug.Log("dragOrigin:" + dragOrigin);
            // Debug.Log("dragOrigin world:" + GetWorldPosition(dragOrigin));
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        lastTimeDrag = DateTime.Now;

        // if (Math.Abs(Input.mousePosition.x - lastMousePos.x) <= 20f) return;
        // if (Input.mousePosition == lastMousePos)
        // {
        //     return;
        // }
        // Debug.Log("Input.mousePosition:" + Input.mousePosition);

        var mouseWorldPos = GetWorldPosition(dragOrigin);
        if (mouseWorldPos.y < parallaxLayer1.bottomY) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * 7, 0, 0);

        // lastMousePos = Input.mousePosition;

        dragOrigin = Input.mousePosition;

        // Debug.Log("pos:" + pos + " Input.mousePosition:" + Input.mousePosition + " mouseWorld:" + GetWorldPosition(Input.mousePosition) + " dragOrigin:" + dragOrigin + " lastMousePos:" + lastMousePos);

        CamTranslateByDrag(move);

        // FarestFollowCam();
        MoveLayers();

        oldPosition = transform.position;
    }

    public void MoveToStartPos()
    {
        StartCoroutine(MoveIE());
        IEnumerator MoveIE()
        {
            float speed = (transform.position.x + 6) / 1.8f;

            while (true)
            {
                yield return null;
                if (transform.position.x > -6f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(-6f, transform.position.y, transform.position.z), Time.deltaTime * speed);
                    MoveLayers();
                    oldPosition = transform.position;
                }
                else
                {
                    var tmpPos = transform.position;
                    tmpPos.x = -6;
                    transform.position = tmpPos;
                    MoveLayers();
                    oldPosition = transform.position;
                    break;
                }
            }
        }
    }

    // void ProcessTouch()
    // {
    //     int i = 0;

    //     while (i < Input.touchCount)
    //     {
    //         // Debug.Log("touch:" + i);

    //         Touch t = Input.GetTouch(i);
    //         var touchPos = GetWorldPosition(t.position);

    //         lastTimeDrag = DateTime.Now;

    //         if (t.phase == TouchPhase.Began)
    //         {
    //             dragOrigin = touchPos;
    //         }
    //         else if ((t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary))
    //         {
    //             if (Vector2.Distance(touchPos, lastMousePos) <= 0.01f) return;

    //             Vector3 distance = touchPos - dragOrigin;
    //             Vector3 move = new Vector3(distance.x * 0.3f, 0, 0);

    //             CamTranslateByDrag(move);

    //             MoveLayers();

    //             lastMousePos = touchPos;

    //             oldPosition = transform.position;
    //         }
    //         else if (t.phase == TouchPhase.Ended)
    //         {
    //         }
    //         i++;
    //     }
    // }

    private void CamTranslateByDrag(Vector3 move)
    {
        if (move.x > 0f)
        {
            if (this.transform.position.x > camMinX)
            {
                transform.Translate(-move, Space.World);
                if (transform.position.x < camMinX)
                {
                    var tmpPos = transform.position;
                    tmpPos.x = camMinX;
                    transform.position = tmpPos;
                }
            }
        }
        else
        {
            if (this.transform.position.x < camMaxX)
            {
                transform.Translate(-move, Space.World);

                if (transform.position.x > camMaxX)
                {
                    var tmpPos = transform.position;
                    tmpPos.x = camMaxX;
                    transform.position = tmpPos;
                }
            }
        }
    }

    Vector3 GetWorldPosition(Vector3 screenPos)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 100));
    }
}
