using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class ExtendedTile
{
    // Unity Tile Object to draw
    public Tile DrawTile;
    // Does the Tile have Collision?
    public bool HasCollision;
    // Is the Tile currently empty
    public bool IsEmpty;
    // Is the Tile part of UI Indicators?
    public bool IsUiTile;
    // Is the Tile a spawner Tile?
    public bool IsSpawnerTile;
    // Is the Tile an indicator for valid placements?
    public bool IsIndicator;
    // Position of the Tile
    public Vector3Int Position;

    public ExtendedTile(Vector3Int position, Tile tile = null, bool isUiTile = false, bool isSpawnerTile = false, bool isEmpty = false, bool isIndicator = false)
    {
        this.DrawTile = tile;
        this.IsUiTile = isUiTile;
        this.IsSpawnerTile = isSpawnerTile;
        this.IsEmpty = isEmpty;
        this.IsIndicator = isIndicator;
        this.Position = position;
    }
}