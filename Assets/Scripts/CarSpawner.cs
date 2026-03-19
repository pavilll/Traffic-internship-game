using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject carPrefab;

    [SerializeField]
    private float spawnDelay = 3f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnCar), 1f, spawnDelay);
    }

    void SpawnCar()
    {
        Instantiate(carPrefab, transform.position, transform.rotation);
    }
}