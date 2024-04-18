using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using System;
using System.Collections.Generic;
using System.Linq;

public class Block : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private bool isDragging = false;
    private Vector3 offset;

    [SerializeField] private float dragSpeed = 1.5f;
    [SerializeField] private List<GridCell> blocks = new List<GridCell>();
    [SerializeField] private List<BoxCollider2D> blocksColliders = new List<BoxCollider2D>();

    public GridManager gridManager;
    private Vector3 previousPos = Vector3.zero;
    private Subject<Unit> onPointerDownSubject = new Subject<Unit>();
    private Subject<Unit> onPointerUpSubject = new Subject<Unit>();
    private Subject<PointerEventData> onDragSubject = new Subject<PointerEventData>();

    public IObservable<Unit> OnPointerDownAsObservable() => onPointerDownSubject;
    public IObservable<Unit> OnPointerUpAsObservable() => onPointerUpSubject;
    public IObservable<PointerEventData> OnDragAsObservable() => onDragSubject;

    void Start()
    {
        previousPos = transform.position;

        onPointerDownSubject.Subscribe(_ =>
        {
            offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        });

        onDragSubject.Subscribe(eventData =>
        {
            if (isDragging)
            {
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventData.position) + offset;
                transform.position = Vector3.Lerp(transform.position, newPosition, dragSpeed);
            }
        });

        onPointerUpSubject.Subscribe(_ => 
        {
            isDragging = false;
            CheckCollision();
        });
    }

    private bool AreAllCollidersTriggered(out List<GridCell> collidingObjects)
    {
        collidingObjects = new List<GridCell>();

        foreach (BoxCollider2D collider in blocksColliders)
        {
            Collider2D[] results = new Collider2D[1];
            ContactFilter2D filter = new ContactFilter2D() { useTriggers = true };

            if (collider.OverlapCollider(filter, results) <= 0) 
                return false;

            GridCell grid = results[0].gameObject.GetComponent<GridCell>();

            if (grid == null || (grid.root != null && grid.root != this))
                return false;

            collidingObjects.Add(grid);
        }
        return true;
    }

    public void CheckCollision()
    {
        List<GridCell> grids;
        bool allColliding = AreAllCollidersTriggered(out grids);

        if (allColliding)
        {
            SetGrids(null);
            blocks.Clear();
            blocks.AddRange(grids);
            transform.position = blocks[0].transform.position;
            previousPos = transform.position;
            SetGrids(this);
        }
        else
        {
            Vector2 mousePosition = Input.mousePosition;

            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            bool outside = results.Any(r => r.gameObject.tag == "Spawn");
            transform.position = outside ? transform.position : previousPos;
            SetGrids(outside ? null : this);
        }
    }
    private void SetGrids(Block b)
    {
        foreach (GridCell grid in blocks) 
        { 
            grid.SetRoot(b); 
        }
    }
    public void SetUp(GridManager gm)
    {
        gridManager = gm;
        blocksColliders?.AddRange(GetComponentsInChildren<BoxCollider2D>());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDownSubject.OnNext(Unit.Default);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUpSubject.OnNext(Unit.Default);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDragSubject.OnNext(eventData);
    }
}