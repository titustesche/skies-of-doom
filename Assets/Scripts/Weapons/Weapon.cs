using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay
{
    public class Weapon : MonoBehaviour
    {
        
        // Damage, range and fire rate for the weapon
        public int damage;
        public float range;
        // Yes, this is technically delay between shots
        public double fireRate;
        public double _cooldown;

        public double Cooldown
        {
            get
            {
                return _cooldown;
            }
            set
            {
                if (double.IsInfinity(value)) _cooldown = Double.MaxValue;
                else if (value <= 0) _cooldown = 0;
                else _cooldown = value;
            }
        }

        private GameObject _target;
        public float projectileSpeed;
        public PlayerController parentPlayer;
        public GameObject[] activeEnemies = null;

        // On startup, try to find an enemy to attack
        void Awake()
        {
            damage = Convert.ToInt32(damage * parentPlayer.damageMultplier);
            fireRate *= parentPlayer.AttackSpeedMultiplier;
            
            // Find the nearest one by distance and set it as the target
            _target = FindNearestEnemy(range);
        }

        private void FixedUpdate()
        {
            if (_target == null)
            {
                Destroy(gameObject);
                return;
            }
            
            var targetEnemy = _target.GetComponent<EnemyController>();

            var step = projectileSpeed * Time.fixedDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, step);

            if ((transform.position - _target.transform.position).magnitude <= 0.5f)
            {
                targetEnemy.ReceiveDamage(parentPlayer, Convert.ToInt32(damage * parentPlayer.damageMultplier));
                Destroy(gameObject);
            }
        }

        public bool CanAttackOneOf(GameObject[] enemies)
        {
            return FindNearestEnemy(range, enemies) != null;
        }
        
        private GameObject FindNearestEnemy(float maxRange, GameObject[] enemies = null)
        {
            GameObject nearestEnemy = null;
            float shortestDistance = float.MaxValue;
            
            if (enemies != null)
            {
                foreach (var enemy in enemies)
                {
                    float distance = Vector2.Distance(enemy.transform.position, transform.position);
                    if (distance <= maxRange && distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestEnemy = enemy;
                    }
                }
                return nearestEnemy;
            }
            
            foreach (var enemy in activeEnemies)
            {
                float distance = Vector2.Distance(enemy.transform.position, transform.position);
                if (distance <= maxRange && distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
            return nearestEnemy;
        }
    }
}
