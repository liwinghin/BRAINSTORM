using UnityEngine;
using UnityEngine.Tilemaps;
using UniRx;
using System.Collections.Generic;
using System.Diagnostics;

namespace PathFinding
{
    public class PathFindingManager : MonoBehaviour
    {
        [SerializeField] TileHighlighter tileHighlighter;
        [SerializeField] MapGenerator mapGenerator;

        [SerializeField] Tilemap tilemap;
        [SerializeField] TileBase pointTile;
        [SerializeField] TileBase pathTile;

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
                .Where(s => !s.Equals(Default.Coordinate))
                .Subscribe(_ => DoPathFinding())
                .AddTo(this);
        }
        private void DoPathFinding()
        {
            path.Clear();
            path = JumpPointSearch.FindPath(mapGenerator, startPoint.Value, endPoint.Value);
            VisualizePath();

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
        void VisualizePath()
        {
            foreach (Coordinate coordinate in path)
            {
                tilemap.SetTile(new Vector3Int(coordinate.x, coordinate.y, 0), pathTile);
            }
        }

        public void SetPoint(Vector3Int pos)
        {
            bool isSelected = mapGenerator.MarkUpCoordinates(pos, ref startPoint, ref endPoint);
            if (isSelected)
            {
                tilemap.SetTile(pos + Default.offset, pointTile);
            }
            else
            {
                tilemap.SetTile(pos + Default.offset, null);
            }
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