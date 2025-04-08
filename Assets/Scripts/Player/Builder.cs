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
    // The Tile the Player draws with
    public Tile drawTile;
    // List to store the valid tiles
    public List<Vector3Int> validTiles = new List<Vector3Int>();
    // The position of the previous hovered Tile
    public Vector3Int previousTilePosition;
    // The Tile that was applied to the previously hovered cell
    public TileBase previousTile;
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
    
    // Called every 0.02 Seconds
    void FixedUpdate()
    {
        if (_playerController.isInBuildMode)
        {
            // If the previous tile was not undefined
            if (previousTilePosition != Vector3Int.zero)
            {
                // Fill the previous spot with the original tile
                tilemap.SetTile(previousTilePosition, previousTile);
                previousTilePosition = Vector3Int.zero;
            }
            
            // Translate Mouse Position to tile position
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var highlightedTilePosition = tilemap.WorldToCell(mousePosition);
            highlightedTilePosition.z = 0;
            
            // Store Tile position and type
            previousTilePosition = highlightedTilePosition;
            var tileAsExtendedTile = _playerController.tileManager.GetTileAt(highlightedTilePosition);
            previousTile = tilemap.GetTile<TileBase>(highlightedTilePosition);
            // If the Tile is a valid tile, render highlight
            if (tileAsExtendedTile != null)
            {
                if (tileAsExtendedTile.IsIndicator)
                {
                    if (_playerController.money >= 5)
                    {
                        tilemap.SetTile(highlightedTilePosition, highlightedTile);
                        return;
                    }

                    tilemap.SetTile(highlightedTilePosition, invalidTile);
                }
            }
        }
    }
    
    // Method that handles Building - as the name implies
    public void Build(ExtendedTile tile)
    {
        // This eats performance like a champ when the list of valid tiles gets larger
        // Todo: Create a completely new, serializable Tile Class
        //  Requirements:
        //      - Tile Preis - Price, float, public
        //      - Tile Bezeichnung - Name, string, public
        //      - Wurde das Tile bereits freigeschaltet - Unlocked, bool, public
        //      - IsValidTile() - bool, public: should be more efficient than to check the List every frame
        //  Maybe as part of TileManager

        var currentTile = _playerController.tileManager.GetTileAt(tile.Position);
        
        if (currentTile != null)
        {
            if (currentTile.IsIndicator /*&& _playerController.money >= drawTile.Price*/)
            {
                Debug.Log("Found tile to be valid");
                _playerController.tileManager.OverwriteTile(tile);
                previousTilePosition = Vector3Int.zero;
                // _playerController.money -= drawTile.Price;
                _playerController.tileManager.GenerateOutlineTiles(tile.Position);
                _playerController.tileManager.UpdateHighlightTiles();
                return;
            }
            
            Debug.Log("Tile is null");
            return;
        }
        
        Debug.Log("Tile is invalid");
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

    public void ToggleBuildMode()
    {
        if (_playerController.isInBuildMode)
        {
            // Reset build mode variable for the player controller
            _playerController.isInBuildMode = false;
            _playerController.tileManager.GenerateOutlineTiles(_playerController.tileCheckCoordinates);
            _playerController.tileManager.DisableHighlightVisibility();
        }

        else
        {
            // Set build mode variable for the player controller
            _playerController.isInBuildMode = true;
            // Determine valid/collision tiles
            _playerController.tileManager.GenerateOutlineTiles(_playerController.tileCheckCoordinates);
            _playerController.tileManager.EnableHighlightVisibility();
            _playerController.tileManager.UpdateHighlightTiles();
        }
    }
}
