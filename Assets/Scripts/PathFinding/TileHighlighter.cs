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

        [SerializeField] ReactiveProperty<Vector3Int> highlightedTilePosition = new ReactiveProperty<Vector3Int>(new Vector3Int(-1, -1, -1));
        public Vector3Int CurrentHoveredTilePos => highlightedTilePosition.Value;

        void Start()
        {
            highlightedTilePosition
                .Pairwise()
                .Subscribe(pair =>
                {
                    SetTile(pair.Previous, TileAsset.defaultTile);
                    SetTile(pair.Current, TileAsset.highLightTile);
                })
                .AddTo(this);

            Observable.EveryUpdate()
                .Select(_ => GetHighlightedTilePosition())
                .DistinctUntilChanged()
                .Where(c => mapGenerator.IsValidTile(c))
                .Subscribe(pos => SetPosition(pos))
                .AddTo(this);
        }

        Vector3Int GetHighlightedTilePosition()
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

        void SetPosition(Vector3Int pos)
        {
            TileBase tile = tilemap.GetTile(pos);

            if (tile != TileAsset.defaultTile)
                return;

            highlightedTilePosition.Value = pos;
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