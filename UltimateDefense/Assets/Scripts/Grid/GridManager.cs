using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject cellPrefab; // 预制体，代表一个格子
    public int width = 9;
    public int height = 9;
    public float cellSize = 1f;
    //[SerializeField] private Vector2 gridStartPos = new Vector2(-7f, -3f); // 起始位置，可在 Inspector 中调

    private Grid[,] grid;

    private void Start()
    {
        GenerateGrid();
        transform.localScale = Vector3.one * 1.6875f;
        transform.localPosition=new Vector3(0,1.5f,0);
    }

    private void GenerateGrid()
    {
        grid = new Grid[height, width]; // [行, 列]

        Vector2 gridStartPos = new Vector2(
            -width * cellSize * 0.5f + cellSize * 0.5f,
            -height * cellSize * 0.5f + cellSize * 0.5f
        );

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                Vector2 pos = new Vector2(col * cellSize, row * cellSize) + gridStartPos;
                GameObject cellObj = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                Grid cell = cellObj.GetComponent<Grid>();
                cell.Init(new Vector2Int(row, col)); // (行, 列)
                grid[row, col] = cell;
                if ((row + col) % 2 == 0)
                {
                    cellObj.GetComponent<SpriteRenderer>().color = new Color32(42, 45, 53, 255);
                }
                else
                {
                    cellObj.GetComponent<SpriteRenderer>().color = new Color32(52,55,65,255);

                }
            }
        }
    }


    /// <summary>
    /// 获取格子
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Grid GetGrid(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= height || pos.y < 0 || pos.y >= width)
            return null;

        return grid[pos.x, pos.y]; // x 是行（height），y 是列（width）
    }

    /// <summary>
    /// 用于外部查询格子的index
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public Grid GetGridAtPosition(Vector2 worldPos)
    {
        Vector2 gridStartPos = new Vector2(
            -width * cellSize * 0.5f + cellSize * 0.5f,
            -height * cellSize * 0.5f + cellSize * 0.5f
        );
        Vector2 offset = worldPos - gridStartPos;
        int col = Mathf.FloorToInt(offset.x / cellSize);
        int row = Mathf.FloorToInt(offset.y / cellSize);

        if (row < 0 || row >= height || col < 0 || col >= width) return null;
        return grid[row, col];
    }
}
