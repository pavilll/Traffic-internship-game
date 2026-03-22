using UnityEngine;

public class StopZone : MonoBehaviour
{
    public TrafficLightPresenter trafficLight;

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<CarView>(out var car))
        {
            if (trafficLight.IsGreen())
                car.SetMove(true);
            else
                car.SetMove(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<CarView>(out var car))
        {
            car.SetMove(true);
        }
    }
}