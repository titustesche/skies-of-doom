using UnityEngine;

namespace Gameplay
{
    public class Weapon : MonoBehaviour
    {
        // Enum for all available weapon types
        public enum WeaponTypes
        {
            Ranged,
            Melee,
            Magic,
        }
        
        // Damage, range and fire rate for the weapon
        public int damage;
        public float range;
        public float fireRate;
        public WeaponTypes type;
        public GameObject target;
        public float projectileSpeed;

        void FixedUpdate()
        {
            try
            {
                var targetEnemy = target.GetComponent<EnemyController>();

                var step = projectileSpeed * Time.fixedDeltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);

                if ((transform.position - target.transform.position).magnitude <= 0.5f)
                {
                    targetEnemy.health -= damage;
                    Destroy(gameObject);
                }
            }

            catch
            {
                Destroy(gameObject);
            }
        }
    }
}
