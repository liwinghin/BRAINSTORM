using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileAssetData", menuName = "Tile Asset Data")]
public class TileAssetData : ScriptableObject
{
    public TileBase defaultTile;
    public TileBase disableTile;
    public TileBase highLightTile;
    public TileBase pointTile;
    public TileBase pathTile;
}