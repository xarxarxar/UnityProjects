using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MergeBeast;

public class DragBattle : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // Start is called before the first frame update
    private Vector2 leftPos = new Vector2(-297, 253);
    private Vector2 rightPos = new Vector2(324, 244);
    private Vector2 centerPos = new Vector2(394, 361);

    [SerializeField] List<BattleItem> listItem;
    [SerializeField] Text txtModeName;
    [SerializeField] Button btnEnter;
    private int currentMode = -1;
    private string[] modeName = { "Arena", "Tile Monster", "Crush" };

    private bool allowClick = true;

    private float startX, endX;

    private void OnEnable()
    {
        if (currentMode == -1)
            GetCurrentMode(listItem.Count - 1);
    }

    private void Start()
    {
        btnEnter.onClick.AddListener(() => { StartCoroutine(GoToMode()); });
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startX = eventData.position.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //neu ko co ham nay thi loi cmn luon
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        endX = eventData.position.x;

        if (endX > startX)
        {
            DragRight();
        }
        else
        {
            DragLeft();
        }
    }

    void DragLeft()
    {
        foreach (var item in listItem)
        {
            item.ChangeIndex(1, GetCurrentMode);
        }
    }

    void DragRight()
    {
        foreach (var item in listItem)
        {
            item.ChangeIndex(-1, GetCurrentMode);
        }
    }

    void GetCurrentMode(int index)
    {
        currentMode = index;
        txtModeName.text = modeName[index];
        btnEnter.gameObject.SetActive(index != 0);
    }

    IEnumerator GoToMode()
    {
        if (!allowClick) yield break; ;
        allowClick = false;
        Debug.Log("currentMode:" + currentMode);

        if (currentMode == 1)
        {
            //tile monster
            Scene scene = SceneManager.GetSceneByName(StringDefine.SCENE_TILE);
            if (scene.IsValid())
            {
                var loaded = SceneManager.SetActiveScene(scene);
                if (!loaded)
                    SceneManager.LoadScene(StringDefine.SCENE_TILE, LoadSceneMode.Additive);
            }
            else
            {
                SceneManager.LoadScene(StringDefine.SCENE_TILE, LoadSceneMode.Additive);
            }
            allowClick = true;
            SoundManager.Instance.MuteMainGame(true);
        }
        else if (currentMode == 2)
        {
            //crush
            Scene scene = SceneManager.GetSceneByName(StringDefine.SCENE_MAP);
            if (scene.IsValid())
            {
                SceneManager.SetActiveScene(scene);
                allowClick = true;
            }
            else
            {
                var asyncLoad = SceneManager.LoadSceneAsync(StringDefine.SCENE_MAP, LoadSceneMode.Additive);
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }
                allowClick = true;
            }
        }
        else
        {
            //coming soon
            Debug.Log("comming soon");
        }
    }
}
