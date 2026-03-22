using UnityEngine;

public class GamePresenter : MonoBehaviour
{
    public GameObject carPrefab;
    public Transform spawnPoint;
    public TrafficLightPresenter trafficLight;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnCar), 1f, 3f);
    }

    void SpawnCar()
{
    var car = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);

    // Просто убеждаемся, что компонент есть
    car.GetComponent<CarPresenter>();
}
}