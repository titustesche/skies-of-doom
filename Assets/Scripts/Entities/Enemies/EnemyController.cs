using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [SerializeField] public float speed;
    private GameObject _targetPlayer;
    private PlayerController _targetPlayerController;
    public int damage = 5;
    public int health = 100;
    private Rigidbody2D _rigidbody;
    public GameObject coinPrefab;
    // private Component targetPlayerController;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _targetPlayer = GameObject.Find("Player");
        _targetPlayerController = _targetPlayer.GetComponent<PlayerController>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var step = speed * Time.fixedDeltaTime;
        var targetPosition = Vector3.MoveTowards(transform.position, _targetPlayer.transform.position, step);

        _rigidbody.MovePosition(targetPosition);
        
        if ((transform.position - _targetPlayer.transform.position).magnitude <= 0.1f)
        {
            _targetPlayerController.health -= damage;
        }

        if (health <= 0)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
