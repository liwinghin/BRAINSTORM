using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UniRx;
using System.Linq;

public class GridManager : MonoBehaviour
{
    [Header("Grid")]
    private GameObject[,] gridCells;
    [SerializeField] private Transform gridsParent;
    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private GameObject blockPrefab;

    public int gridSizeX = 5;
    public int gridSizeY = 5;

    [Header("Item")]
    [SerializeField] private Transform itemsParent;
    [SerializeField] private Transform spawnPoint;

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        gridCells = new GameObject[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                GameObject gridCell = Instantiate(gridCellPrefab, gridsParent, false);
                SetGridCellData(gridCell, x, y);
                gridCells[x, y] = gridCell;
            }
        }
    }

    public void CreateItem(Image item, List<Vector2> vectors)
    {
        GameObject obj = Instantiate(blockPrefab, itemsParent, false);
        obj.transform.position = GetCellPosition((int)vectors[0].x, (int)vectors[0].y);
        obj.transform.localScale = Vector3.one;
        Color color = Random.ColorHSV();

        vectors
            .Where(v => IsValidCoordinate((int)v.x, (int)v.y))
            .ToList()
            .ForEach(v =>
            {
                Image newBlock = Instantiate(item);
                RectTransform blockTransform = newBlock.GetComponent<RectTransform>();
                blockTransform.SetParent(obj.transform);
                blockTransform.localScale = Vector3.one;
                blockTransform.transform.position = GetCellPosition((int)v.x, (int)v.y);
                newBlock.color = color;
            });

        obj.GetComponent<Block>().SetUp(this);
        obj.transform.position = spawnPoint.position;
    }

    public void AddPoints(List<Vector2> colliderPoints, Vector2 point)
    {
        if (!colliderPoints.Contains(point))
        {
            colliderPoints.Add(point);
        }
    }
    public Vector3 GetCellPosition(int x, int y)
    {
        if (IsValidCoordinate(x, y))
        {
            return gridCells[x, y].transform.position;
        }
        return Vector3.zero;
    }
    private bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY;
    }
    private void SetGridCellData(GameObject cell, int x, int y)
    {
        cell.name = $"Grid: {x}, {y}";
        cell.GetComponent<RectTransform>().localScale = Vector3.one;
        cell.GetComponent<GridCell>().SetCoordinate($"{x}, {y}");
    }
}