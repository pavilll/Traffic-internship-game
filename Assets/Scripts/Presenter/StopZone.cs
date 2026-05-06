using UnityEngine;

public class StopZone : MonoBehaviour
{
    [SerializeField] private TrafficLightPresenter trafficLight;

    private void Awake()
    {
        if (trafficLight == null)
        {
            Debug.LogError("StopZone: trafficLight is not assigned.", this);
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (trafficLight == null)
        {
            return;
        }

        if (other.TryGetComponent<CarPresenter>(out var car))
        {
            car.OnEnterZone(trafficLight.GetState());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (trafficLight == null)
        {
            return;
        }

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

    private void OnDrawGizmos()
    {
        BoxCollider zone = GetComponent<BoxCollider>();
        if (zone == null)
        {
            return;
        }

        Gizmos.color = new Color(1f, 0.9f, 0.2f, 0.35f);
        Matrix4x4 previousMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(zone.center, zone.size);
        Gizmos.matrix = previousMatrix;
    }
}
