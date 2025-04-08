using System.Collections.Generic;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public int health = 100;
    public int money = 5;
    public float moveSpeed = 5f;
    private Rigidbody2D _rigidbody;
    public bool isInBuildMode = false;
    public Vector3 tileCheckCoordinates = Vector3.zero;
    private PlayerInput _playerInput;
    private PlayerControls _playerControls;
    private GameObject _player;
    private Builder _builder;
    private CircleCollider2D _collider;
    public List<ExtendedTile> availableTiles;
    public TileManager tileManager;
    public TMP_Text moneyText;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tileCheckCoordinates = transform.position - new Vector3(0, 0.6f, 0);
        _rigidbody = GetComponent<Rigidbody2D>();
        
        _player = gameObject;
        _builder = _player.GetComponent<Builder>();
        _collider = GetComponent<CircleCollider2D>();
        
        _builder.GenerateSpawnPlatform(tileCheckCoordinates);
        
        // Randomly distribute spawner tiles
        var spawnerTiles = tileManager.GenerateSpawnerTiles(tileCheckCoordinates);
        tileManager.AddTileArray(spawnerTiles, availableTiles[2]);

        tileManager.GenerateOutlineTiles(tileCheckCoordinates);
    }

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        
        _playerControls = new PlayerControls();
        _playerControls.Gameplay.Enable();
        _playerControls.Gameplay.MousePress.performed += ScreenTouched;
        _playerControls.Gameplay.EnterShopMode.performed += EnterShop;
		_playerControls.Gameplay.TimeControl.performed += TimeControl;
    }

	public void TimeControl(InputAction.CallbackContext context) {
		Debug.Log("Time wants to be controlled");
	}

    public void EnterShop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _builder.drawTile = availableTiles[0].DrawTile;
            _builder.ToggleBuildMode();
        }
    }
    
    public void ScreenTouched(InputAction.CallbackContext context)
    {
        if (true)
        {
            var position = Camera.main.ScreenToWorldPoint(_playerControls.Gameplay.MousePosition.ReadValue<Vector2>());
            position.z = 0;
            var tilePosition = _builder.tilemap.WorldToCell(position);
            _builder.Build(new ExtendedTile(tilePosition, availableTiles[0].DrawTile, false, false, false, false));
        }
    }
    
    // Apply Movement in FixedUpdate
    void FixedUpdate()
    {
        var movement = _playerControls.Gameplay.Move.ReadValue<Vector2>() * moveSpeed;
        _rigidbody.MovePosition(_rigidbody.position + movement * Time.fixedDeltaTime);
        moneyText.SetText(money.ToString());
    }
    
    void Update()
    {
        // Todo:
        //  - Revamp to work with Unity InputSystem for Cross Platform support
        //  - Input mapping
        
        /*
        // Enter build Mode when 1-3 key is pressed
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // If the player isn't in build mode, enter and return
            if (!isInBuildMode)
            {
                _builder.ToggleBuildMode();
            }
            // If build mode is already active, just change the tile
            _builder.drawTile = availableTiles[0];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!isInBuildMode)
            {
                _builder.ToggleBuildMode();
            }
            _builder.drawTile = availableTiles[1];
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (!isInBuildMode)
            {
                _builder.ToggleBuildMode();
            }
            _builder.drawTile = availableTiles[2];
        }
        */
    }

    public void LeaveGame()
    {
        // Todo:
        //  - Pause the game - done (Tile manager still works but I pretend I don't know)
        //  - Show pause menu - tbd
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        Application.Quit();
    }
}
