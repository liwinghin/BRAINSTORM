using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace PathFinding
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] Tilemap tilemap;
        [SerializeField] TileBase[] tiles; // 你的瓦片集

        [SerializeField] int width = 6;
        [SerializeField] int height = 6;
        struct TileInfo
        {
            public Vector3Int position;
            public string coordinate;

            public TileInfo(Vector3Int pos, string coord)
            {
                position = pos;
                coordinate = coord;
            }
        }

        List<TileInfo> tileInfoList = new List<TileInfo>();


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
                    int tileIndex = Random.Range(0, tiles.Length);
                    TileBase tile = tiles[tileIndex];
                    tilemap.SetTile(tilePosition, tile);

                    GameObject tileGameObject = new GameObject("TileCollider");
                    tileGameObject.transform.position = tilemap.GetCellCenterWorld(tilePosition);
                    tileGameObject.AddComponent<BoxCollider2D>().isTrigger = true;
                    tileGameObject.transform.parent = colliderParent.transform;
                    tileGameObject.tag = "Tile";

                    string coordinate = "(" + x + ", " + y + ")";
                    tileInfoList.Add(new TileInfo(tilePosition, coordinate));
                }
            }
        }

        public string GetCoordinateFromPosition(Vector3Int position)
        {
            foreach (TileInfo tileInfo in tileInfoList)
            {
                if (tileInfo.position == position)
                {
                    return tileInfo.coordinate;
                }
            }
            return "Unknown";
        }
    }
}