using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class ExtendedTile
{
    // Display Name (for the shop)
    public string DisplayName;
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
    // All game objects the tile has spawned
    public List<GameObject> linkedObjects = new List<GameObject>();
    // Price of the Tile
    public int Price;
    // The player who placed the tile
    public PlayerController parentPlayer;

    public ExtendedTile(Vector3Int position, Tile tile = null, bool isUiTile = false, bool isSpawnerTile = false, bool isEmpty = false, bool isIndicator = false, bool hasCollision = false, int price = 0)
    {
        DrawTile = tile;
        IsUiTile = isUiTile;
        IsSpawnerTile = isSpawnerTile;
        IsEmpty = isEmpty;
        IsIndicator = isIndicator;
        Position = position;
        HasCollision = hasCollision;
        Price = price;
    }
    
    
}