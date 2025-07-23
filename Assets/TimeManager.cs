using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    // The game Time in Seconds
    private float _totalGameTimeSeconds;
    public float TotalGameTimeSeconds { get => _totalGameTimeSeconds; set => _totalGameTimeSeconds = value > 0 ? value : 0; }
    
    private int _gameTimeSeconds;
    public int GameTimeSeconds { get => _gameTimeSeconds; set => _gameTimeSeconds = value > 0 ? value : 0; }
    
    public event Action<int> MinuteElapsed;
    
    private int _gameTimeMinutes;

    public int GameTimeMinutes
    {
        get => _gameTimeMinutes;
        set
        {
            if (_gameTimeMinutes == value) return;
            _gameTimeMinutes = value > 0 ? value : 0;
            MinuteElapsed?.Invoke(_gameTimeMinutes);
        }
    }

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
        TotalGameTimeSeconds += Time.deltaTime;
        GameTimeMinutes = (int)(_totalGameTimeSeconds / 60);
        GameTimeSeconds = (int)_totalGameTimeSeconds % 60;
    }

    public string GetNiceTime()
    {
        return $"{GameTimeMinutes:D2}:{GameTimeSeconds:D2}";
    }
    
    public void ResetTimer()
    {
        TotalGameTimeSeconds = 0;
        GameTimeSeconds = 0;
        GameTimeMinutes = 0;
        Running = false;
    }
}
