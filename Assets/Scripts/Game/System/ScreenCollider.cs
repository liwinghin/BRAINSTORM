using System.Collections.Generic;
using UnityEngine;

public class ScreenCollider : MonoBehaviour
{
    EdgeCollider2D edgeCollder;
    void Awake()
    {
        edgeCollder = this.GetComponent<EdgeCollider2D>();
        CreateEdgeCollider();
    }
    void CreateEdgeCollider()
    {
        List<Vector2> edges = new List<Vector2>();

        // Calculate screen dimensions with 5-pixel offset
        float xOffset = 5f;
        float yOffset = 5f;
        float screenWidth = Screen.width - xOffset;
        float screenHeight = Screen.height - yOffset;

        edges.Add(Camera.main.ScreenToWorldPoint(new Vector2(xOffset, yOffset)));
        edges.Add(Camera.main.ScreenToWorldPoint(new Vector2(screenWidth, yOffset)));
        edges.Add(Camera.main.ScreenToWorldPoint(new Vector2(screenWidth, screenHeight)));
        edges.Add(Camera.main.ScreenToWorldPoint(new Vector2(xOffset, screenHeight)));
        edges.Add(Camera.main.ScreenToWorldPoint(new Vector2(xOffset, yOffset)));

        edgeCollder.SetPoints(edges);
    }
}