using System;
using System.Collections.Generic;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    // Public
    #region Base Stats
    // Health
    private int _health;
    public int Health
    {
        get { return _health; } 
        set { 
            _health = value <= 0 ? 0 : value; 
            UIController.Instance.UpdateStats(this);
        }
    }
    private int _maxHealth;
    public int MaxHealth
    {
        get { return _maxHealth; }
        set
        {
            _maxHealth = value <= 0 ? 0 : value;
            Health = _maxHealth;
        }
    }
    
    // Money
    private int _money;

    public int Money
    {
        get { return _money; } 
        set {
            _money = value <= 0 ? 0 : value;
            UIController.Instance.UpdateStats(this);
            if (UIController.Instance.Gamestate == UIController.Gamestates.Shop) ShopController.Instance.UpdateTileShop();
        }
    }
    
    // Movement Speed
    public float moveSpeed = 5f;
    #endregion
    #region Modifiers
    public double damageMultplier = 1;
    public double healthMultiplier = 1;
    public double speedMultiplier = 1;
    private double _attackSpeedMultiplier = 1;
    public double AttackSpeedMultiplier
    {
        get { return _attackSpeedMultiplier; }
        set
        {
            _attackSpeedMultiplier = value < 0 ? 0 : value;
            UIController.Instance.UpdateStats(this);
        }
    }
    
    #endregion
    #region Player States
    
    public bool isInBuildMode;
    public List<ExtendedTile> availableTiles;
    
    // Track Total scores for Stats Overlay
    private int _kills = 0;
    public int Kills { get { return _kills; } set { _kills = value <= 0 ? 0 : value; UIController.Instance.UpdateStats(this);} }
    private int _totalDamage = 0;
    public int TotalDamage { get { return _totalDamage; } set { _totalDamage = value <= 0 ? 0 : value; UIController.Instance.UpdateStats(this);} }
    #endregion
    #region Foreign Objects
    public TileManager tileManager;
    private Rigidbody2D _rigidbody;
    private PlayerInput _playerInput;
    private PlayerControls _playerControls;
    private GameObject _player;
    private Builder _builder;
    private CircleCollider2D _collider;
    #endregion
    
    public ExtendedTile selectedTile;
    public List<Weapon> weapons;
    public Vector3 tileCheckCoordinates = Vector3.zero;

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
        selectedTile = availableTiles[0];
        UIController.Instance.SetGameState(UIController.Gamestates.Running);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _money = 50;
        _health = 100;
        
        _playerInput = GetComponent<PlayerInput>();
        
        _playerControls = new PlayerControls();
        _playerControls.Gameplay.Enable();
        _playerControls.Gameplay.MousePress.performed += ScreenTouched;
        _playerControls.Gameplay.EnterShopMode.performed += ShopToggled;
		_playerControls.Gameplay.TimeControl.performed += TimeControl;
        _playerControls.Gameplay.PauseGame.performed += EscapePressed;
        _playerControls.Gameplay.ShowStatsMenu.started += ShowStatsMenu;
        _playerControls.Gameplay.ShowStatsMenu.canceled += ShowStatsMenu;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        Health -= other.gameObject.GetComponent<EnemyController>().damage;
        Destroy(other.gameObject);
    }

    private void ShowStatsMenu(InputAction.CallbackContext context)
    {
        if (context.started) UIController.Instance.ShowStatsOverlay();
        if (context.canceled) UIController.Instance.HideStatsOverlay();
    }

    public void TimeControl(InputAction.CallbackContext context) {
		Debug.Log("Time wants to be controlled");
	}

    public void ShopToggled(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (UIController.Instance.Gamestate)
            {
                case UIController.Gamestates.Running:
                    UIController.Instance.SetGameState(UIController.Gamestates.Shop);
                    tileManager.GenerateOutlineTiles(new Vector3Int(0, 0, 0));;
                    tileManager.RenderTilesOnOutline(new ExtendedTile(new Vector3Int(0, 0, 0), _builder.validTileIndicator, true, false, false, false, true));
                    break;
                case UIController.Gamestates.Shop:
                    UIController.Instance.SetGameState(UIController.Gamestates.Running);
                    tileManager.ClearOutline();
                    break;
            }
        }
    }

    public void ScreenTouched(InputAction.CallbackContext context)
    {
        if (context.performed && SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Game"))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            var position = Camera.main.ScreenToWorldPoint(_playerControls.Gameplay.MousePosition.ReadValue<Vector2>());
            var tilePosition = _builder.tilemap.WorldToCell(position);
            tilePosition.z = 0;

            var tileToBuild = selectedTile;
            tileToBuild.Position = tilePosition;
            tileToBuild.parentPlayer = this;
            _builder.Build(tileToBuild);
        }
    }
    
    // Apply Movement in FixedUpdate
    void FixedUpdate()
    {
        if (Health <= 0) UIController.Instance.SetGameState(UIController.Gamestates.Dead);
        var movement = _playerControls.Gameplay.Move.ReadValue<Vector2>() * moveSpeed;
        _rigidbody.MovePosition(_rigidbody.position + movement * Time.fixedDeltaTime);
        // Todo: This does absolutely NOT need to be called in FixedUpdate
        Attack();
    }
    
    public void EscapePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            switch (UIController.Instance.Gamestate)
            {
                case UIController.Gamestates.Running:
                    UIController.Instance.SetGameState(UIController.Gamestates.Pause);
                    break;
                case UIController.Gamestates.Pause:
                    UIController.Instance.SetGameState(UIController.Gamestates.Running);
                    break;
                case UIController.Gamestates.Shop:
                    ShopToggled(context);
                    break;
            }
        }
    }

    // Gets called on each iteration of FixedUpdate
    void Attack()
    {
        var activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (Weapon weapon in weapons)
        {
            // Some preparation:
            //      - Subtract the elapsed time from the cooldown
            //      - Set the parent player to this PlayerController Instance
            //      - Set the active enemies to the enemies currently on the screen
            // Note: Could be done once on Start(), but oh well why not waste resources?
            weapon.Cooldown -= Time.deltaTime;
            weapon.parentPlayer = this;
            weapon.activeEnemies = activeEnemies;
            
            if (weapon.Cooldown <= 0 && weapon.CanAttackOneOf(activeEnemies))
            {
                weapon.Cooldown = weapon.fireRate / AttackSpeedMultiplier;
                // Instantiate a new Weapon and associate it with this PlayerController Instance
                Instantiate(weapon, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            }
        }
    }
}
