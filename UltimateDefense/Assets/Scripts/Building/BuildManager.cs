using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public Camera mainCamera;
    public GridManager gridManager;

    public GameObject buildMenu; // UI面板
    public Button[] buildButtons; // 每个按钮绑定不同建筑建造事件
    public BuildingBase[] buildingPrefabs; // 对应建筑预制体

    private Grid selectedGrid;

    public static BuildManager instance;

    private void Awake()
    {
        instance=this;
    }

    private void Start()
    {
        buildMenu.SetActive(false);

        // 绑定按钮事件
        for (int i = 0; i < buildButtons.Length; i++)
        {
            int index = i; // 局部变量防闭包
            buildButtons[i].onClick.AddListener(() => BuildOnSelectedGrid(index));
        }
    }

    private void Update()
    {
        
    }

    public void ShowBuildMenu(Vector3 worldPos)
    {
        buildMenu.SetActive(true);

        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
        buildMenu.transform.position = screenPos;
    }

    public void ShowBuildMenu(Grid grid)
    {
        // 存储当前选中的格子
        //currentSelectedGrid = grid;

        // 设置建造菜单的位置到格子上方（如果是 UI 的话）
        buildMenu.SetActive(true);
        selectedGrid=grid;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(grid.transform.position);
        buildMenu.transform.position = screenPos;

        // 根据 grid.GridPos.x/y 决定可建造的内容等
    }

    public  void HideBuildMenu()
    {
        buildMenu.SetActive(false);
        selectedGrid = null;
    }

    private void BuildOnSelectedGrid(int prefabIndex)
    {
        
        if (selectedGrid == null) return;
        Debug.Log($"点击按钮{prefabIndex}");
        BuildingBase building = Instantiate(buildingPrefabs[prefabIndex], selectedGrid.transform.position, Quaternion.identity);
        selectedGrid.Occupy(building);
        building.Init(10);
        HideBuildMenu();
    }
}
