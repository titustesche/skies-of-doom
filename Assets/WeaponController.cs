using System;
using System.Linq;
using Gameplay;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject[] weapons;
    private GameObject[] _activeEnemies = {};
    private GameObject _targetEnemy = null;
    private float _lastShot = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var weapon in weapons)
        {
            var weaponScript = weapon.GetComponent<Weapon>();
            foreach (var enemy in _activeEnemies)
            {
                float[] distances = {};
                var distance = Vector2.Distance(enemy.transform.position, transform.position);
                if (distance <= weaponScript.range && Time.time - _lastShot >= weaponScript.fireRate)
                {
                    distances.Append(distance);
                    _lastShot = Time.time;
                    weaponScript.target = enemy;
                    Instantiate(weapon, gameObject.transform.position, Quaternion.identity);
                    return;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        
    }
}
