using UnityEngine;

public class CarController : MonoBehaviour
{
    public static int AccidentCount = 0;
    private bool crashed = false;
    public float maxSpeed = 6f;
    public float acceleration = 3f;
    private float currentSpeed = 0f;
    public float detectionDistance = 3f;
    private bool stoppedByLight = false;
    private TrafficLight currentLight;

    void Update()
    {
        bool blockedByCar = CheckCarAhead();

        if (!stoppedByLight && !blockedByCar)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration * Time.deltaTime);
        }

        if (currentSpeed > 0f)
        {
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        }
    }

    bool CheckCarAhead()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionDistance))
        {
            if (hit.collider.CompareTag("Car"))
            {
                return true;
            }
        }

        return false;
    }

    public void Stop(TrafficLight light)
    {
        if (light.currentState == TrafficLight.LightState.Yellow)
            return;

        stoppedByLight = true;

        currentLight = light;
        currentLight.OnLightChanged += CheckLight;
    }

    void CheckLight(TrafficLight.LightState state)
    {
        if (state == TrafficLight.LightState.Green)
        {
            Go();
        }
    }

    public void Go()
    {
        stoppedByLight = false;

        if (currentLight != null)
        {
            currentLight.OnLightChanged -= CheckLight;
        }
    }

    void OnCollisionEnter(Collision collision)
{
    if (collision.collider.CompareTag("Car"))
    {
        CarController otherCar = collision.collider.GetComponent<CarController>();

        if (otherCar == null) return;

        if (crashed || otherCar.crashed) return;

        crashed = true;
        otherCar.crashed = true;

        AccidentCount++;

        Debug.Log("Accidents: " + AccidentCount);
    }
}
}