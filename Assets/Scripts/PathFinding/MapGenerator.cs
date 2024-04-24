using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UniRx;

namespace PathFinding
{
    [System.Serializable]
    public struct Coordinate
    {
        public int x;
        public int y;

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public bool Equals(Coordinate other)
        {
            return x == other.x && y == other.y;
        }
    }
    public static class Default
    {
        public static readonly Vector3Int Vector3Int = new Vector3Int(-1, -1, -1);
        public static readonly Vector3Int offset = new Vector3Int(1, 1, -1);
        public static readonly Coordinate Coordinate = new Coordinate(-1, -1);
    }

    public class MapGenerator : MonoBehaviour
    {
        [System.Serializable]
        struct TileInfo
        {
            public Vector3Int position;
            public Coordinate coordinate;
            public bool isSelected;
            public bool isWalkable;

            public TileInfo(Vector3Int pos, Coordinate coord, bool selected, bool walkable)
            {
                position = pos;
                coordinate = coord;
                isSelected = selected;
                isWalkable = walkable;
            }
        }

        [SerializeField] Tilemap tilemap;

        [SerializeField] int width = 6;
        [SerializeField] int height = 6;

        [SerializeField] List<TileInfo> tileInfoList = new List<TileInfo>();

        float walkableChance = 0.9f;

        private void Awake()
        {
            TileAsset.Init();
        }
        void Start()
        {
            GenerateMap();
        }

        void GenerateMap()
        {
            GameObject colliderParent = new GameObject("TileColliders");

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    Coordinate coordinate = new Coordinate(x, y);

                    bool isWalkable = Random.value < walkableChance;
                    tileInfoList.Add(new TileInfo(tilePosition, coordinate, false, isWalkable));
                    TileBase tile = isWalkable? TileAsset.defaultTile : TileAsset.disableTile;
                    tilemap.SetTile(tilePosition, tile);

                    GameObject tileGameObject = new GameObject("TileCollider");
                    tileGameObject.transform.position = tilemap.GetCellCenterWorld(tilePosition);
                    tileGameObject.AddComponent<BoxCollider2D>().isTrigger = true;
                    tileGameObject.transform.parent = colliderParent.transform;
                    tileGameObject.tag = "Tile";
                }
            }
        }
        public bool IsValidTile(Vector3Int tilePosition)
        {
            int index = tileInfoList.FindIndex(tileInfo => tileInfo.position.Equals(tilePosition));

            if (index != -1 && tileInfoList.Count > index)
            {
                return tileInfoList[index].isWalkable;
            }
            else
            {
                return false;
            }
        }

        public bool IsTileWalkable(Coordinate tile)
        {
            int index = tileInfoList.FindIndex(tileInfo => tileInfo.coordinate.Equals(tile));

            if (index != -1 && tileInfoList.Count > index)
            {
                return tileInfoList[index].isWalkable;
            }
            else
            {
                return false;
            }
        }

        public bool MarkUpCoordinates(Vector3Int position, ref ReactiveProperty<Coordinate> startPoint, ref ReactiveProperty<Coordinate> endPoint)
        {
            int index = tileInfoList.FindIndex(tileInfo => tileInfo.position == position);

            if (index != -1)
            {
                Coordinate coordinate = tileInfoList[index].coordinate;

                if (!startPoint.Value.Equals(Default.Coordinate) && !endPoint.Value.Equals(Default.Coordinate) && !startPoint.Value.Equals(coordinate) && !endPoint.Value.Equals(coordinate))
                    return false;

                tileInfoList[index] = new TileInfo(tileInfoList[index].position, coordinate, !tileInfoList[index].isSelected, tileInfoList[index].isWalkable);

                if (tileInfoList[index].isSelected)
                {
                    if (startPoint.Value.Equals(Default.Coordinate))
                        startPoint.Value = coordinate;
                    else if (endPoint.Value.Equals(Default.Coordinate))
                        endPoint.Value = coordinate;
                }
                else
                {
                    if (startPoint.Value.Equals(coordinate))
                        startPoint.Value = Default.Coordinate;
                    else if (endPoint.Value.Equals(coordinate))
                        endPoint.Value = Default.Coordinate;
                }

                return tileInfoList[index].isSelected;
            }

            return false;
        }
    }
}