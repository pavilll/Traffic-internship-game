using UnityEngine;

public class StopZone : MonoBehaviour
{
    public TrafficLightPresenter trafficLight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CarPresenter>(out var car))
        {
            car.OnEnterZone(trafficLight.GetState());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<CarPresenter>(out var car))
        {
            car.SetLightState(trafficLight.GetState());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<CarPresenter>(out var car))
        {
            car.OnExitZone();
        }
    }
}