using UnityEngine;

public class CoinController : MonoBehaviour
{
    [SerializeField] public float speed;
    private GameObject _targetPlayer;
    private PlayerController _targetPlayerController;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _targetPlayer = GameObject.Find("Player");
        _targetPlayerController = _targetPlayer.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        var step = speed * Time.fixedDeltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _targetPlayer.transform.position, step);

        if ((transform.position - _targetPlayer.transform.position).magnitude <= 0.01f)
        {
            _targetPlayerController.money += 1;
            Destroy(gameObject);
        }
    }
}
