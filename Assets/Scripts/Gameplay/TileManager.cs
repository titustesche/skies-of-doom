using System.Collections.Generic;
using System.Linq;
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
        private List<Vector3Int> _checkedTilePositions = new List<Vector3Int>();
        private List<Vector3Int> _validSpawnerTiles = new List<Vector3Int>();
        private readonly Dictionary<Vector3Int, ExtendedTile> _trackedTiles = new Dictionary<Vector3Int, ExtendedTile>();
        public GameObject enemySpawnerPrefab;
        private List<Vector3Int> _outlineTilePositions = new List<Vector3Int>{};
        
        // Render a specified Tile to all positions determined to be an Outline by GenerateOutlineTiles
        public void RenderTilesOnOutline(ExtendedTile tile)
        {
            // Update Outline Tile Positions
            GenerateOutlineTiles(new Vector3Int(0, 0, 0));

            for (var i = 0; i < _outlineTilePositions.Count; i++)
            {
                OverwriteTile(new ExtendedTile(_outlineTilePositions[i], tile.DrawTile, tile.IsUiTile, tile.IsSpawnerTile, tile.IsEmpty, tile.IsIndicator, tile.HasCollision, tile.Price));
            }
        }

        public void ClearOutline()
        {
            DestroyTileArray(_outlineTilePositions);
        }

        public void GenerateOutlineTiles(Vector3 playerPosition)
        {
			// Clear list of checked positions
			_checkedTilePositions = new List<Vector3Int>();
            _outlineTilePositions = new List<Vector3Int>();
            _outlineTilePositions = CheckTilesRecursively(new Vector3Int(0, 0, 0));
        }

        private List<Vector3Int> CheckTilesRecursively(Vector3Int tilePosition)
        {
            _checkedTilePositions.Add(tilePosition);
            
            var validTiles = new List<Vector3Int>(); 
            
            var neighborOffsets = tilePosition.y % 2 == 0 ?
                new Vector3Int[] {
                    new Vector3Int(-1, 1, 0),    // Top left
                    new Vector3Int(0, 1, 0),     // Top right
                    new Vector3Int(1, 0, 0),     // Right
                    new Vector3Int(0, -1, 0),    // Bottom Right
                    new Vector3Int(-1, -1, 0),   // Bottom left
                    new Vector3Int(-1, 0, 0)     // Left
                }
                :
                new Vector3Int[] {
                    new Vector3Int(0, 1, 0),     // Top
                    new Vector3Int(1, 1, 0),     // Top-right
                    new Vector3Int(1, 0, 0),     // Bottom-right
                    new Vector3Int(1, -1, 0),    // Bottom
                    new Vector3Int(0, -1, 0),    // Bottom-left
                    new Vector3Int(-1, 0, 0)     // Top-left
                };

            foreach (var offset in neighborOffsets)
            {
                var neighborPos = tilePosition + offset;
                
                if (!_checkedTilePositions.Contains(neighborPos))
                {
                    var neighborTile = GetTileAt(neighborPos);

                    if (neighborTile != null && !neighborTile.IsUiTile)
                    {
                        // Wenn Nachbar eine walkable Kachel ist, rekursiv weitergehen
                        var walkableTiles = CheckTilesRecursively(neighborPos);
                        validTiles.AddRange(walkableTiles);
                    }
                    else if (GetTileAt(tilePosition) != null && !GetTileAt(tilePosition).IsUiTile)
                    {
                        // Wenn aktuelle Position walkable ist und Nachbar leer (null), 
                        // dann ist Nachbar eine valide Position
                        validTiles.Add(neighborPos);
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

        // Todo: Apparently calling an empty function (this one) is more effective at deleting tiles than calling the function that actually has logic to delete tiles
        //      Most curious
        public void DestroyTile(Vector3Int position)
        {
            var tile = GetTileAt(position);
            if (tile == null) { return; }
            // Remove if exists
        }
        
        // IMPORTANT: If you want to delete multiple tiles at once, use DestroyTileArray()
        private void DestroyTile(ExtendedTile tile)
        {
            // Check if the tile exists
            if (GetTileAt(tile.Position) == null) return;
            // Delete all associated objects
            foreach (var obj in tile.linkedObjects) { Destroy(obj); }
            _trackedTiles.Remove(tile.Position);
            if (tile.IsIndicator) _outlineTilePositions.Remove(tile.Position);
            // Check for which tilemap the tile should be removed from
            if (tile.HasCollision) collisonTilemap.SetTile(tile.Position, null);
            else tilemap.SetTile(tile.Position, null);
        }
        
        public void DestroyTileArray(List<Vector3Int> positions)
        {
            // Has to be copied because the list gets modified in the loop
            var posCopy = new List<Vector3Int>(positions);
            foreach (var position in posCopy)
            {
                var tile = GetTileAt(position);
                // Check if the tile exists
                if (tile == null) continue;
                // Check if the tile has spawner prefabs associated with it
                foreach (var obj in tile.linkedObjects) { Destroy(obj); }
                // Check for which tilemap the tile should be located in
                if (tile.HasCollision) {
                    collisonTilemap.SetTile(tile.Position, null);
                    _outlineTilePositions.Remove(tile.Position);
                    _trackedTiles.Remove(tile.Position);
                    continue;
                }
                tilemap.SetTile(tile.Position, null);
            }
        }
        
        public void AddTileArray(List<Vector3Int> positions, ExtendedTile tile)
        {
            foreach (var position in positions)
            {
                var fixedPosition = position;
                fixedPosition.z = 0;
                tile.Position = fixedPosition;
                AddTile(tile);
            }
        }

        
        public void OverwriteTile(ExtendedTile tile)
        {
            // Exit early if the Tile is empty
            if (tile == null) return;
            // Destroy Tile if a matching Position is present
            if (_trackedTiles.TryGetValue(tile.Position, out var existingTile)) DestroyTile(existingTile);
            // Add the new Tile
            AddTile(tile);
        }
        
        
        // Todo: Something in here is broken with error: Object reference is set to null
        //  I am pretty sure it's the ExtendedTile Object but i have no clue why
        public void AddTile(ExtendedTile tile)
        {
            if (GetTileAt(tile.Position) == null)
            {
                // Tracked it down, it's the appending it to the List thing
                // Apparently the List is null even tho debugger lists it as an empty List
                _trackedTiles.Add(tile.Position, tile);

                foreach (var obj in tile.linkedObjects)
                {
                    var tmpObj = Instantiate(obj, tilemap.CellToWorld(tile.Position), Quaternion.identity);
                    try
                    {
                        obj.GetComponent<DamageEmitter>().parentPlayer = tile.parentPlayer;
                    }

                    catch
                    {
                        // We don't want any error messages here
                    }
                }
                
                // Todo: This is deprecated wit the new spawning mechanic
                /*
                if (tile.IsSpawnerTile)
                {
                    var spawner = Instantiate(enemySpawnerPrefab, tilemap.CellToWorld(tile.Position), Quaternion.identity);
                    tile.linkedObjects.Add(spawner);
                }
                */
                
                // Place in normal tilemap if the tile has no collision
                if (!tile.HasCollision)
                {
                    tilemap.SetTile(tile.Position, tile.DrawTile);
                    return;
                }
                
                // Else place in collision tilemap
                collisonTilemap.SetTile(tile.Position, tile.DrawTile);
            }
        }

        // Returns a tile at a given Position
        public ExtendedTile GetTileAt(Vector3Int position)
        {
            // Return the tile at that position
            if (_trackedTiles.TryGetValue(position, out var tile))
            {
                return tile;
            }
            
            return null;
        }
    }
}