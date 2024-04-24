using UnityEngine;
using UnityEngine.Tilemaps;
using UniRx;
using System.Collections.Generic;
using System.Linq;

namespace PathFinding
{
    public class PathFindingManager : MonoBehaviour
    {
        [SerializeField] TileHighlighter tileHighlighter;
        [SerializeField] MapGenerator mapGenerator;

        [SerializeField] Tilemap tilemap;

        [SerializeField] private ReactiveProperty<Coordinate> startPoint = new(Default.Coordinate);
        [SerializeField] private ReactiveProperty<Coordinate> endPoint = new(Default.Coordinate);

        [SerializeField] private List<Coordinate> path = new List<Coordinate>();

        void Start()
        {
            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0))
                .Select(_ => GetClickedTilePosition())
                .Where(tilePosition => IsValidTilePosition(tilePosition))
                .Subscribe(tilePosition => SetPoint(tilePosition))
                .AddTo(this);

            endPoint            
                .SkipLatestValueOnSubscribe()
                .Subscribe(_ => DoPathFinding())
                .AddTo(this);
        }
        private void DoPathFinding()
        {
            if (!endPoint.Value.Equals(Default.Coordinate))
            {
                path = JumpPointSearch.FindPath(mapGenerator, startPoint.Value, endPoint.Value);
                path.RemoveAt(path.Count - 1);
                VisualizePath(TileAsset.pathTile);
            }
            else
            {
                VisualizePath(TileAsset.defaultTile);
                path.Clear();
            }


            //Compare two Path Finding method
            //Stopwatch astarStopwatch = Stopwatch.StartNew();
            //AStar.FindPath(mapGenerator, startPoint.Value, endPoint.Value);
            //astarStopwatch.Stop();
            //UnityEngine.Debug.Log($"A* Algorithm Time: {astarStopwatch.ElapsedMilliseconds} ms");

            //Stopwatch jpsStopwatch = Stopwatch.StartNew();
            //JumpPointSearch.FindPath(mapGenerator, startPoint.Value, endPoint.Value);
            //jpsStopwatch.Stop();
            //UnityEngine.Debug.Log($"Jump Point Search Algorithm Time: {jpsStopwatch.ElapsedMilliseconds} ms");

        }
        void VisualizePath(TileBase pathTile)
        {
            path.Select(coordinate => new Vector3Int(coordinate.x, coordinate.y, 0))
                .ToList()
                .ForEach(pos => tilemap.SetTile(pos, pathTile));
        }

        public void SetPoint(Vector3Int pos)
        {
            bool isSelected = mapGenerator.MarkUpCoordinates(pos, ref startPoint, ref endPoint);
            tilemap.SetTile(pos + Default.offset, isSelected ? TileAsset.pointTile : null);
        }

        Vector3Int GetClickedTilePosition()
        {
            return tileHighlighter.CurrentHoveredTilePos;
        }

        bool IsValidTilePosition(Vector3Int tilePosition)
        {
            return tilePosition != Default.Vector3Int;
        }
    }


}