using UnityEngine;

public class Killswitch : MonoBehaviour
{
    private ParticleSystem system;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        system = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (system && !system.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
