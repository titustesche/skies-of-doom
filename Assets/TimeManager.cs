using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    // The game Time in Seconds
    private float _totalGameTimeSeconds;
    public float TotalGameTimeSeconds { get => _totalGameTimeSeconds; set => _totalGameTimeSeconds = value > 0 ? value : 0; }
    
    private int _gameTimeSeconds;
    public int GameTimeSeconds { get => _gameTimeSeconds; set => _gameTimeSeconds = value > 0 ? value : 0; }
    
    private int _gameTimeMinutes;
    public int GameTimeMinutes { get => _gameTimeMinutes; set => _gameTimeMinutes = value > 0 ? value : 0; }
    
    private bool _running;
    public bool Running { get => _running; set => _running = value; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!_running) return;
        _totalGameTimeSeconds += Time.deltaTime;
        _gameTimeMinutes = (int)(_totalGameTimeSeconds / 60);
        _gameTimeSeconds = (int)_totalGameTimeSeconds % 60;
    }

    public string GetNiceTime()
    {
        return $"{_gameTimeMinutes:D2}:{_gameTimeSeconds:D2}";
    }
    
    public void ResetTimer()
    {
        _totalGameTimeSeconds = 0;
        _gameTimeSeconds = 0;
        _gameTimeMinutes = 0;
        _running = false;
    }
}
