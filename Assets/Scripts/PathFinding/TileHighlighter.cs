using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UniRx;

namespace PathFinding
{
    public class TileHighlighter : MonoBehaviour
    {
        public Tilemap tilemap;

        [SerializeField] Vector3Int offset = new Vector3Int(1, 1, -1);

        [SerializeField] TileBase defaultTile;
        [SerializeField] TileBase hoverTile;
        [SerializeField] TileBase pointTile;

        [SerializeField] ReactiveProperty<Vector3Int> hoveredTilePosition = new ReactiveProperty<Vector3Int>(new Vector3Int(-1, -1, -1));

        void Start()
        {
            hoveredTilePosition
                .Pairwise()
                .Subscribe(pair =>
                {
                    SetTile(pair.Previous, defaultTile);
                    SetTile(pair.Current, hoverTile);
                })
                .AddTo(this);

            Observable.EveryUpdate()
                .Select(_ => GetHoveredTilePosition())
                .DistinctUntilChanged()
                .Subscribe(tilePosition => hoveredTilePosition.Value = tilePosition)
                .AddTo(this);

            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0)) // 检测鼠标左键点击
                .Select(_ => GetClickedTilePosition())
                .Where(tilePosition => IsValidTilePosition(tilePosition))
                .Subscribe(tilePosition => tilemap.SetTile(tilePosition + offset, pointTile))
                .AddTo(this);
        }

        Vector3Int GetHoveredTilePosition()
        {
            RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Tile"))
            {
                return tilemap.WorldToCell(hit.point);
            }
            else
            {
                return new Vector3Int(-1, -1, -1);
            }
        }

        void SetTile(Vector3Int tilePosition, TileBase tile)
        {
            if (tilePosition.x >= 0 && tilePosition.y >= 0 && tilePosition.z >= 0)
            {
                tilemap.SetTile(tilePosition, tile);
            }
        }
        Vector3Int GetClickedTilePosition()
        {
            return hoveredTilePosition.Value;
        }

        bool IsValidTilePosition(Vector3Int tilePosition)
        {
            return tilePosition.x >= 0 && tilePosition.y >= 0 && tilePosition.z >= 0;
        }
    }
}