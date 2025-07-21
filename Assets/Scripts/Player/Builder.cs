using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Builder : MonoBehaviour
{
    // The Tilemap for the Player to draw on
    public Tilemap tilemap;
    // The collision tilemap
    public Tilemap collisionTilemap;
    // The Tile that should be used to highlight the currently hovered over Tile
    public Tile highlightedTile;
    // Tile to indicate that a tile cannot be placed
    public Tile invalidTile;
    // The Tile used to indicate enemy spawns
    public Tile spawnerTile;
    // The Enemy Spawner game object that goes along spawner tiles
    public GameObject enemySpawnerPrefab;
    // The position of the previous hovered Tile
    public Vector3Int previousTilePosition;
    // The Tile that was applied to the previously hovered cell
    public ExtendedTile previousTile;
    // Indicator that should be used to mark valid tiles
    public Tile validTileIndicator;
    
    private GameObject _player;
    private PlayerController _playerController;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _player = gameObject;
        _playerController = _player.GetComponent<PlayerController>();
    }
    
    /*
    void FixedUpdate()
    {
        if (_playerController.isInBuildMode)
        {
            // If previous tile position is anything but (0, 0, 0)
            if (previousTilePosition != Vector3Int.zero)
            {
                // Overwrite the tile at this position with the previous tile
                _playerController.tileManager.OverwriteTile(previousTile);
                // Reset the previous tile position to be (0, 0, 0)
                previousTilePosition = Vector3Int.zero;
            }
            
            // Translate Mouse Position to tile position and set it's z index to 0
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var highlightedTilePosition = tilemap.WorldToCell(mousePosition);
            highlightedTilePosition.z = 0;
            
            // Store Tile position and type
            previousTilePosition = highlightedTilePosition;
            ExtendedTile tileAsExtendedTile = _playerController.tileManager.GetTileAt(highlightedTilePosition);
            previousTile = _playerController.tileManager.GetTileAt(previousTilePosition);
            // If the Tile is a valid tile, render highlight
            if (tileAsExtendedTile != null)
            {
                if (tileAsExtendedTile.IsIndicator)
                {
                    if (_playerController.money >= drawTile.Price)
                    {
						_playerController.tileManager.OverwriteTile(new ExtendedTile(highlightedTilePosition, highlightedTile, true, false, true, true));
                        return;
                    }

                    _playerController.tileManager.OverwriteTile(new ExtendedTile(highlightedTilePosition, invalidTile, true, false, true, true));
                }
            }
        }
    }
    */
    
    // Method that handles Building - as the name implies
    public void Build(ExtendedTile tileToBuild)
    {
        // Todo:
        //      For some reason, this works exactly once before completely falling apart
        //      No clue why or what
        var selectedTile = _playerController.tileManager.GetTileAt(tileToBuild.Position);

        if (selectedTile == null) return; // Tile does not exist
        if (!selectedTile.IsUiTile) return; // Tile cannot be built on
        if (_playerController.Money < tileToBuild.Price) return; // Player doesn't have enough moneydw

        tileToBuild.Position = selectedTile.Position;
        _playerController.tileManager.OverwriteTile(tileToBuild);
        // previousTilePosition = Vector3Int.zero;
        _playerController.Money -= tileToBuild.Price;
        _playerController.tileManager.RenderTilesOnOutline(new ExtendedTile(new Vector3Int(0, 0, 0), validTileIndicator, true, false, false, false, true));
    }

    // Render all determined spawner tiles (see TileManager for more info)
    // Accepts predetermined positions as input
    public void RenderSpawnerTiles(List<Vector2Int> positions)
    {
        // Loop through each tile and render it, as well as spawn an enemy spawner at its location
        foreach (var position in positions)
        {
            tilemap.SetTile(new Vector3Int(position.x, position.y, 0), spawnerTile);
            Instantiate(enemySpawnerPrefab, tilemap.CellToWorld(new Vector3Int(position.x, position.y, 0)), Quaternion.identity);
        }
    }

    public void GenerateSpawnPlatform(Vector3 position)
    {
        var tilePosition = tilemap.WorldToCell(position);
        var offsets = (tilePosition.y % 2 == 0) ?
            new List<Vector3Int> {
                new Vector3Int(0, 0, 0),
                new Vector3Int(-1, 1, 0),    // Top left
                new Vector3Int(0, 1, 0),    // Top right
                new Vector3Int(1, 0, 0),   // Right
                new Vector3Int(0, -1, 0),   // Bottom Right
                new Vector3Int(-1, -1, 0),  // Bottom left
                new Vector3Int(-1, 0, 0)    // Left
            }
            :
            new List<Vector3Int> {
                new Vector3Int(0, 0, 0),
                new Vector3Int(0, 1, 0),    // Top
                new Vector3Int(1, 1, 0),    // Top-right
                new Vector3Int(1, 0, 0),    // Bottom-right
                new Vector3Int(1, -1, 0),   // Bottom
                new Vector3Int(0, -1, 0),   // Bottom-left
                new Vector3Int(-1, 0, 0)    // Top-left
            };

        for (var i = 0; i < offsets.Count; i++)
        {
            offsets[i] += tilePosition;
        }
        
        _playerController.tileManager.AddTileArray(offsets, _playerController.availableTiles[0]);
    }
}
