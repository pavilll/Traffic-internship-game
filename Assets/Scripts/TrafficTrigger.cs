using UnityEngine;

public class TrafficTrigger : MonoBehaviour
{
    public TrafficLight trafficLight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            CarController car = other.GetComponent<CarController>();

            if (trafficLight.IsGreen())
            {
                car.Go();
            }
            else
            {
                car.Stop(trafficLight);
            }
        }
    }
}