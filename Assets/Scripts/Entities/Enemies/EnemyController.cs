using System;
using Gameplay;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{

    [SerializeField] public float speed;
    private GameObject _targetPlayer;
    private PlayerController _targetPlayerController;
    public int damage = 5;
    public int health = 100;
    private Rigidbody2D _rigidbody;
    public GameObject coinPrefab;
    public GameObject bloodParrticleSystem;
    // private Component targetPlayerController;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _targetPlayer = GameObject.Find("Player");
        _targetPlayerController = _targetPlayer.GetComponent<PlayerController>();
        _rigidbody = GetComponent<Rigidbody2D>();
        
        // Randomize Strength
        damage = Random.Range(2, 6);
        health = Random.Range(5, 30);
        speed = Random.Range(1.7f, 2.3f);
    }

    public void ReceiveDamage(PlayerController player, int amount)
    {
        if (health > amount) { health -= amount; player.TotalDamage += amount; }
        
        // Moved from fixedUpdate so it doesn't get checked every .2 seconds
        if (health <= amount)
        {
            var coin = Instantiate(coinPrefab, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity);
            coin.GetComponent<CoinController>().value = Random.Range(1, 5);
            Instantiate(bloodParrticleSystem, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            Destroy(gameObject);
            player.Kills += 1;
            player.TotalDamage += health;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var step = speed * Time.fixedDeltaTime;
        var targetPosition = Vector3.MoveTowards(transform.position, _targetPlayer.transform.position, step);

        _rigidbody.MovePosition(targetPosition);
        
        if ((transform.position - _targetPlayer.transform.position).magnitude <= 1.0f)
        {
            Debug.Log("Attempting to hit player");
            _targetPlayerController.Health -= damage;
        }
    }
}
