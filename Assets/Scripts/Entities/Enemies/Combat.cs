using UnityEngine;

// Todo: Rework... Everything?

public class Combat : MonoBehaviour
{

    [SerializeField] public int health;
    [SerializeField] public int damage;
    [SerializeField] private GameObject targetPlayer;
    [SerializeField] private PlayerController targetPlayerController;
    [SerializeField] private float lastHit;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetPlayer = GameObject.Find("Player");
        targetPlayerController = targetPlayer.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, targetPlayer.transform.position) < 0.1f && lastHit + 0.2f < Time.time)
        {
            Debug.Log("Attempting to hit player");
            targetPlayerController.Health -= damage;
            lastHit = Time.time;
        }
    }
}
