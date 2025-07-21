using System;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public int value;
    private GameObject _targetPlayer;
    private PlayerController _targetPlayerController;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _targetPlayerController = PlayerController.Instance;
        _targetPlayer = _targetPlayerController.gameObject;
    }

    // Basically, if two coins touch, the one with more value will absorb the one with less; helps immensely with lag
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Coin")) return;
        var controller = other.GetComponent<CoinController>();
        if (controller.value < value) return;
        controller.value += value;
        Destroy(gameObject);
    }

    // Update is called once per frame - sure it is, Unity
    void Update()
    {
        var step = speed * Time.fixedDeltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _targetPlayer.transform.position, step);

        if ((transform.position - _targetPlayer.transform.position).magnitude <= 0.001f)
        {
            _targetPlayerController.Money += value;
            Destroy(gameObject);
        }
    }
}
