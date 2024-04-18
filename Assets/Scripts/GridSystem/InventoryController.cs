using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;

public class InventoryController : MonoBehaviour
{
    public Image blockPrefab;
    public GridManager gridManager;

    Vector2[] possibleDirections = new Vector2[]
    {
        Vector2.up, Vector2.down, Vector2.left, Vector2.right
    };
    public void GenerateRandomBlock()
    {
        int shape = Random.Range(0, 4);
        GenerateCoordinates(shape);
    }
    void GenerateCoordinates(int count)
    {
        List<Vector2> items = new List<Vector2>();
        int randomX = Random.Range(0, gridManager.gridSizeX);
        int randomY = Random.Range(0, gridManager.gridSizeY);

        Vector2 currentCoordinate = new Vector2(randomX, randomY);
        items.Add(currentCoordinate);

        for (int i = 0; i < count; i++)
        {
            List<Vector2> validDirections = possibleDirections
                .Where(direction =>
                {
                    Vector2 nextCoordinate = currentCoordinate + direction;
                    return !items.Contains(nextCoordinate)
                        && nextCoordinate.x >= 0 && nextCoordinate.x < gridManager.gridSizeX
                        && nextCoordinate.y >= 0 && nextCoordinate.y < gridManager.gridSizeY;
                })
                .ToList();

            if (validDirections.Count <= 0) { break;}

            Vector2 chosenDirection = validDirections[Random.Range(0, validDirections.Count)];
            currentCoordinate += chosenDirection;
            items.Add(currentCoordinate);
        }

        gridManager.CreateItem(blockPrefab, items);
    }
}
