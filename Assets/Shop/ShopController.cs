using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance { get; private set; }

    public bool isActive = false;
    public GameObject tileButtonPrefab;
    private Transform _tileButtonContainer;
    
    
    private PlayerController _player;
    private List<ExtendedTile> _tiles;
    private Dictionary<int, GameObject> _tileButtons = new Dictionary<int, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _player = PlayerController.Instance;
        _tileButtonContainer = gameObject.transform.Find("Tiles").transform.Find("Viewport").transform.Find("Content");
    }
    
    // Todo: The next two functions are completely vibe coded, validate and refine
    private void UpdateButton(GameObject button, ExtendedTile tile)
    {
        var btnComponent = button.GetComponent<Button>();
        btnComponent.interactable = tile.Price <= _player.Money;
        button.transform.Find("ItemName").GetComponent<TMP_Text>().text = tile.DisplayName;
        button.transform.Find("ItemPrice").GetComponent<TMP_Text>().text = tile.Price + "";
        btnComponent.onClick.RemoveAllListeners();
        btnComponent.onClick.AddListener(() => { _player.selectedTile = tile; });
    }

    public void UpdateTileShop()
    {
    if (_player == null) return;
    if (_player.availableTiles == null) return;

    _tiles = _player.availableTiles;

    // Update or create buttons for each tile
    foreach (var tile in _tiles)
    {
        if (_tileButtons.TryGetValue(tile.GetHashCode(), out GameObject existingButton))
        {
            UpdateButton(existingButton, tile);
        }
        else
        {
            GameObject newButton = Instantiate(tileButtonPrefab, _tileButtonContainer);
            UpdateButton(newButton, tile);
            _tileButtons[tile.GetHashCode()] = newButton;
        }
    }

    // Remove buttons for tiles that no longer exist
    List<int> buttonsToRemove = new List<int>();
    foreach (var kvp in _tileButtons)
    {
        if (!_tiles.Exists(t => t.GetHashCode() == kvp.Key))
        {
            Destroy(kvp.Value);
            buttonsToRemove.Add(kvp.Key);
        }
    }
    foreach (var key in buttonsToRemove)
    {
        _tileButtons.Remove(key);
        }
    }
    
    public void IncreaseDamage(float percentage)
    {
        if (_player.Money < 10) return;
        _player.damageMultplier *= (1 + (percentage / 100));
        _player.Money -= 10;
    }
    
    public void IncreaseAttackSpeed(float percentage)
    {
        // Example: percentage = 30
        //          added to multiplier: 0,3
        //          new attack speed: 1,0 * 0.3 = 1,3
        //          Note: Since it's time in seconds, we finally divide by that - yeah, im sure this won't cause any confusion down the road
        if (_player.Money < 10) return;
        
        _player.AttackSpeedMultiplier *= (1 + (percentage / 100));
        _player.Money -= 10;
    }
}
