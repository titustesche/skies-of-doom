using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageEmitter : MonoBehaviour
{
    public float damageInterval;
    public int damage;
    public ParticleSystem pSystem;
    
    private CircleCollider2D _collider;
    private float _cooldown;
    private readonly List<GameObject> _objectsInRange = new List<GameObject>();
    public PlayerController parentPlayer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _collider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_cooldown > 0) { _cooldown -= Time.deltaTime; return; }
        Instantiate(pSystem, transform.position, Quaternion.identity);
        _cooldown = damageInterval;
        foreach (var obj in _objectsInRange.ToList())
        {
            if (obj == null) continue;
            if (obj.CompareTag("Enemy"))
            {
                if (obj == null) continue;
                obj.GetComponent<EnemyController>().ReceiveDamage(parentPlayer, damage);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (_objectsInRange.Contains(other.gameObject)) _objectsInRange.Remove(other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_objectsInRange.Contains(other.gameObject)) _objectsInRange.Add(other.gameObject);
    }
}
