using UnityEngine;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance { get; private set; }
    private PlayerController _player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        this._player = PlayerController.Instance;
    }

    public void IncreaseDamage(float percentage)
    {
        if (_player.Money < 10) return;
        _player.damageMultplier *= (1 + (percentage / 100));
        _player.Money -= 10;
    }
    
    public void IncreaseAttackSpeed(float percentage)
    {
        // Example: percentage = 30
        //          added to multiplier: 0,3
        //          new attack speed: 1,0 * 0.3 = 1,3
        //          Note: Since it's time in seconds, we finally divide by that - yeah, im sure this won't cause any confusion down the road
        if (_player.Money < 10) return;
        
        _player.AttackSpeedMultiplier *= (1 + (percentage / 100));
        _player.Money -= 10;
    }
}
