using UnityEngine;

public class StopZone : MonoBehaviour
{
    public TrafficLightPresenter trafficLight;

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<CarPresenter>(out var car))
        {
            bool stop = !trafficLight.IsGreen();
            car.SetBlockedByLight(stop);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<CarPresenter>(out var car))
        {
            car.SetBlockedByLight(false);
        }
    }
}