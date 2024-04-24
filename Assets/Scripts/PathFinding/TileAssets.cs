using UnityEngine;
using UnityEngine.Tilemaps;

namespace PathFinding
{
    public class TileAsset : MonoBehaviour
    {
        public static TileBase defaultTile;
        public static TileBase highLightTile;
        public static TileBase pointTile;
        public static TileBase pathTile;
        public static TileBase disableTile;

        // Reference to the Scriptable Object asset
        public static TileAssetData tileAssetData;

        // Method to assign tiles programmatically
        public static void Init()
        {
            tileAssetData = Resources.Load<TileAssetData>("PathFinding/TileAssetData");

            // Example of accessing tiles from the Scriptable Object
            defaultTile = tileAssetData.defaultTile;
            highLightTile = tileAssetData.highLightTile;
            pointTile = tileAssetData.pointTile;
            pathTile = tileAssetData.pathTile;
            disableTile = tileAssetData.disableTile;
        }
    }
}