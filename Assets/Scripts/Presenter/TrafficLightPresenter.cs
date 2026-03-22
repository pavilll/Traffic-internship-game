using UnityEngine;

public enum TrafficLightState
{
    Red,
    Green
}

public class TrafficLightPresenter : MonoBehaviour
{
    private TrafficLightState state = TrafficLightState.Green;



    public void Switch()
    {
        state = state == TrafficLightState.Red
            ? TrafficLightState.Green 
            : TrafficLightState.Red;

        Debug.Log("Light: " + state);
    }

    public bool IsGreen()
    {
        return state == TrafficLightState.Green;
    }
}