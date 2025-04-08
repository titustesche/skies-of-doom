using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Gameplay
{
    public class TileManager : MonoBehaviour
    {
        public Tilemap tilemap;
        public Tilemap collisonTilemap;
        public Builder builder;
        private Tile _highlightTile;
        private Tile _addTileIndicator;
        private List<Vector3Int> _checkedTilePositions = new List<Vector3Int>();
        private List<Vector3Int> _validSpawnerTiles = new List<Vector3Int>();
        private List<ExtendedTile> _trackedTiles = new List<ExtendedTile>();
        public GameObject enemySpawnerPrefab;
        private bool _highlightTilesVisible = false;
        
        public void GenerateOutlineTiles(Vector3 playerPosition)
        {
            // ValidSpawnerTiles = new List<Vector3Int>();
            _highlightTile = builder.highlightedTile;
            var tileCoordinates = tilemap.WorldToCell(playerPosition);
            tileCoordinates.z = 0;
            var validTilePositions = CheckTilesRecursively(tileCoordinates);
            for (var i = 0; i < validTilePositions.Count; i++)
            {
                AddTile(new ExtendedTile(validTilePositions[i], _addTileIndicator, true, false, true, true));
            }
            _checkedTilePositions = new List<Vector3Int>();
        }

        private List<Vector3Int> CheckTilesRecursively(Vector3Int tilePosition)
        {
            
            _checkedTilePositions.Add(tilePosition);
            
            // List to store all the valid tiles
            var validTiles = new List<Vector3Int>(); 
            
            // Determine Neighbor offsets
            var neighborOffsets = tilePosition.y % 2 == 0 ?
                new Vector3Int[] {
                    new Vector3Int(-1, 1, 0),    // Top left
                    new Vector3Int(0, 1, 0),    // Top right
                    new Vector3Int(1, 0, 0),   // Right
                    new Vector3Int(0, -1, 0),   // Bottom Right
                    new Vector3Int(-1, -1, 0),  // Bottom left
                    new Vector3Int(-1, 0, 0)    // Left
                }
                :
                new Vector3Int[] {
                    new Vector3Int(0, 1, 0),    // Top
                    new Vector3Int(1, 1, 0),    // Top-right
                    new Vector3Int(1, 0, 0),    // Bottom-right
                    new Vector3Int(1, -1, 0),   // Bottom
                    new Vector3Int(0, -1, 0),   // Bottom-left
                    new Vector3Int(-1, 0, 0)    // Top-left
                };

            // Loop through all Neighbors
            foreach (var offset in neighborOffsets)
            {
                // If the neighbor hasn't already been checked
                if (!_checkedTilePositions.Contains(tilePosition + offset))
                {
                    // Get the neighboring Tile
                    var tile = GetTileAt(tilePosition + offset);

                    // If the tile is occupied
                    if (tile != null)
                    {
                        // If the tile isn't an indicator
                        if (!tile.IsUiTile)
                        {
                            // Add all it's neighbors
                            var walkableTiles = CheckTilesRecursively(tilePosition + offset);
                            validTiles.AddRange(walkableTiles);
                        }
                    }
                    
                    else
                    {
                        validTiles.Add(tilePosition + offset);
                    }
                }
            }
            return validTiles;
        }
        public List<Vector3Int> GenerateSpawnerTiles(Vector3 playerPosition)
        {
            // Todo:
            //      Get Player location as tile coordinates - done
            //      Apply offsets - done
            //      Check every Tile in the determined region - done
            //      Have an x percent chance that the tile gets selected as a spawner tile - done
            //          - If it is empty - accounted for
            //          - Until x Tiles are determined to be spawner tiles - currently uses a probability of 2% per tile

            // Reset Spawner Tiles Buffer
            _validSpawnerTiles = new List<Vector3Int>();
            
            // Translate player (collider center) coordinates to tile coordinates
            var tileCoordinates = tilemap.WorldToCell(playerPosition);
            
            // Array to store the actual region markers
            var regionMarkers = new Vector2Int[]
            {
                // Making it a 40x40 area (technically 41x41)
                new Vector2Int(tileCoordinates.x - 20, tileCoordinates.y - 20),
                new Vector2Int(tileCoordinates.x + 20, tileCoordinates.y + 20)
            };
            
            // Loop through all x positions
            for (var x = regionMarkers[0].x; x < regionMarkers[1].x; x++)
            {
                // For every x, move along y from bottom to top
                for (var y = regionMarkers[0].y; y < regionMarkers[1].y; y++)
                {
                    // Get the tile at position x, y
                    var tile = GetTileAt(new Vector3Int(x, y, 0));

                    // If the tile isn't occupied...
                    if (tile == null)
                    {
                        // ...have a 2% chance of filling it with a spawner tile 
                        if (Random.Range(0f, 1f) < 0.02f)
                        {
                            _validSpawnerTiles.Add(new Vector3Int(x, y, 0));
                        }
                    }
                }
            }
            // Return all the determined valid spawner tiles
            return _validSpawnerTiles;
        }

        // Destroys a tile at a given position
        public void DestroyTile(Vector3Int position)
        {
            if (GetTileAt(position) != null)
            {
                // Destroy if exists
            }
        }

        // Important: Do NOT call with new ExtendedTile()
        public void DestroyTile(ExtendedTile tile)
        {
            if (GetTileAt(tile.Position) != null)
            {
                // Destroy if exists
            }
        }

        public void UpdateHighlightTiles()
        {
            if (_highlightTilesVisible)
            {
                foreach (var tile in _trackedTiles)
                {
                    if (tile.IsIndicator)
                    {
                        collisonTilemap.SetTile(tile.Position, builder.validTileIndicator);
                    }
                }
            }
        }

        public void EnableHighlightVisibility()
        {
            _highlightTilesVisible = true;
            foreach (var tile in _trackedTiles)
            {
                if (tile.IsIndicator)
                {
                    collisonTilemap.SetTile(tile.Position, tile.DrawTile);
                }
            }
        }

        public void DisableHighlightVisibility()
        {
            _highlightTilesVisible = false;
            foreach (var trackedTile in _trackedTiles)
            {
                if (trackedTile.IsIndicator)
                {
                    collisonTilemap.SetTile(trackedTile.Position, null);
                }
            }
        }
        
        public void AddTileArray(List<Vector3Int> positions, ExtendedTile tile)
        {
            foreach (var position in positions)
            {
                var fixedPosition = position;
                fixedPosition.z = 0;
                AddTile(new ExtendedTile(fixedPosition, tile.DrawTile, tile.IsUiTile, tile.IsSpawnerTile, tile.IsEmpty, tile.IsIndicator));
            }
        }

        // Todo: Integrate with new ExtendedTile
        public void OverwriteTile(ExtendedTile tile)
        {
            for (var i = 0; i < _trackedTiles.Count; i++)
            {
                if (_trackedTiles[i].Position == tile.Position)
                {
                    _trackedTiles[i] = tile;
                    if (!tile.HasCollision)
                    {
                        collisonTilemap.SetTile(tile.Position, null);
                        tilemap.SetTile(tile.Position, tile.DrawTile);
                        if (tile.IsSpawnerTile)
                        {
                            Instantiate(enemySpawnerPrefab, tilemap.CellToWorld(tile.Position), Quaternion.identity);
                            return;
                        }

                        return;
                    }

                    collisonTilemap.SetTile(tile.Position, tile.DrawTile);
                }
            }
        }
        
        
        // Todo: Something in here is broken with error: Object reference is set to null
        //  I am pretty sure it's the ExtendedTile Object but i have no clue why
        public bool AddTile(ExtendedTile tile)
        {
            if (GetTileAt(tile.Position) == null)
            {
                // Tracked it down, it's the appending it to the List thing
                // Apparently the List is null even tho debugger lists it as an empty List
                _trackedTiles.Add(tile);
                if (!tile.HasCollision)
                {
                    tilemap.SetTile(tile.Position, tile.DrawTile);
                    if (tile.IsSpawnerTile)
                    {
                        Instantiate(enemySpawnerPrefab, tilemap.CellToWorld(tile.Position), Quaternion.identity);
                        return true;
                    }
                    return true;
                }
                
                collisonTilemap.SetTile(tile.Position, tile.DrawTile);
                return true;

            }

            return false;
        }
        
        // Returns a tile at a given Position
        public ExtendedTile GetTileAt(Vector3Int position)
        {
            // Return the tile at that position
            foreach (var tile in _trackedTiles)
            {
                if (tile.Position == position)
                {
                    return tile;
                }
            }
            
            return null;
        }
    }
}
