using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UniRx;

namespace PathFinding
{
    public class TileHighlighter : MonoBehaviour
    {
        [SerializeField] MapGenerator mapGenerator;
        [SerializeField] Tilemap tilemap;
        [SerializeField] TileBase defaultTile;
        [SerializeField] TileBase hoverTile;

        [SerializeField] ReactiveProperty<Vector3Int> hoveredTilePosition = new ReactiveProperty<Vector3Int>(new Vector3Int(-1, -1, -1));
        public Vector3Int CurrentHoveredTilePos => hoveredTilePosition.Value;

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
                .Where(c => mapGenerator.IsValidTile(c))
                .DistinctUntilChanged()
                .Subscribe(tilePosition => hoveredTilePosition.Value = tilePosition)
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
                return Default.Vector3Int;
            }
        }

        void SetTile(Vector3Int tilePosition, TileBase tile)
        {
            if (tilePosition != Default.Vector3Int)
            {
                tilemap.SetTile(tilePosition, tile);
            }
        }
    }
}