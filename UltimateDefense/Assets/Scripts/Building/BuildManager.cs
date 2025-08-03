using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    public Camera mainCamera;
    public GridManager gridManager;

    public BuildingBase[] buildingPrefabs; // 对应建筑预制体

    public Grid SelectedGrid;
    public GameObject ChooseMask;//选中格子时显示的效果
    public Transform buildingParent;//所有建筑的父物体

    public static BuildManager instance;

    private void Awake()
    {
        instance=this;
    }

    private void OnDisable()
    {
        foreach (Transform child in buildingParent)
        {
            Destroy(child.gameObject);
        }
    }

    [SerializeField] private LayerMask buildGridLayerMask;
    void Update()
    {
        Vector2 worldPos = Vector2.zero;
        bool isClick = false;

        // 手机或编辑器点击
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // 如果点到 UI 上就返回
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return;

            worldPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            isClick = true;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {

            // 如果点到 UI 上就返回
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isClick = true;
        }
#endif

        if (isClick)
        {
            // 限定只检测建造格子所在的 Layer
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0, buildGridLayerMask);
            if (hit.collider != null)
            {
                Debug.Log("点击到了：" + hit.collider.name);
                if (hit.collider.CompareTag("BuildingGrid"))
                {
                    ShowBuildMenu(hit.collider.GetComponent<Grid>());
                }
            }
            else
            {
                
                HideBuildMenu();
            }
        }
    }

    public void ShowBuildMenu(Grid grid)
    {
        // 显示建筑面板
        BattleUIManager.Instance.ShowBuildingPanelInBattle();
        SelectedGrid = grid;
        ChooseMask.SetActive(true);
        ChooseMask.transform.position= SelectedGrid.transform.position;

    }

    /// <summary>
    /// 隐藏所有的建筑的菜单面板
    /// </summary>
    public  void HideBuildMenu()
    {
        SelectedGrid = null;
        BattleUIManager.Instance.HideBuildingPanelInBattle();
        ChooseMask.SetActive(false);
    }
}
