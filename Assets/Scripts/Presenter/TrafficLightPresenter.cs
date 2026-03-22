using UnityEngine;
using System.Collections;

public enum TrafficLightState
{
    Green,
    BlinkingGreen,
    Yellow,
    Red
}

public class TrafficLightPresenter : MonoBehaviour
{
    private TrafficLightState state = TrafficLightState.Red;
    private bool isSwitching = false;

    [Header("Timings")]
    public float blinkingDuration = 2f;
    public float yellowDuration = 1f;

    [Header("Lamps")]
    [SerializeField] private Renderer redLamp;
    [SerializeField] private Renderer yellowLamp;
    [SerializeField] private Renderer greenLamp;

    [SerializeField] private float intensity = 5f;

    private void Start()
    {
        SetStateVisual(TrafficLightState.Red);
    }

    public TrafficLightState GetState()
    {
        return state;
    }

    public void Switch()
    {
        if (isSwitching) return;

        if (state == TrafficLightState.Green)
        {
            StartCoroutine(SwitchToRedSequence());
        }
        else if (state == TrafficLightState.Red)
        {
            StartCoroutine(SwitchToGreenSequence());
        }
    }

    private IEnumerator SwitchToRedSequence()
    {
        isSwitching = true;

        // мигающий зелёный
        state = TrafficLightState.BlinkingGreen;
        StartCoroutine(BlinkGreen());
        yield return new WaitForSeconds(blinkingDuration);

        // жёлтый
        state = TrafficLightState.Yellow;
        SetStateVisual(state);
        yield return new WaitForSeconds(yellowDuration);

        // красный
        state = TrafficLightState.Red;
        SetStateVisual(state);

        isSwitching = false;
    }

    private IEnumerator SwitchToGreenSequence()
    {
        isSwitching = true;

        // жёлтый
        state = TrafficLightState.Yellow;
        SetStateVisual(state);
        yield return new WaitForSeconds(yellowDuration);

        // зелёный
        state = TrafficLightState.Green;
        SetStateVisual(state);

        isSwitching = false;
    }

    private void SetStateVisual(TrafficLightState newState)
    {
        SetLamp(redLamp, newState == TrafficLightState.Red, Color.red);
        SetLamp(yellowLamp, newState == TrafficLightState.Yellow, Color.yellow);
        SetLamp(greenLamp, newState == TrafficLightState.Green, Color.green);
    }

    private void SetLamp(Renderer lamp, bool active, Color color)
    {
        if (lamp == null) return;

        var mat = lamp.material;

        mat.EnableKeyword("_EMISSION");

        if (active)
        {
            mat.SetColor("_EmissionColor", color * intensity);
        }
        else
        {
            mat.SetColor("_EmissionColor", Color.black);
        }
    }

    private IEnumerator BlinkGreen()
    {
        float timer = 0f;
        bool on = true;

        while (state == TrafficLightState.BlinkingGreen)
        {
            SetLamp(greenLamp, on, Color.green);

            on = !on;

            yield return new WaitForSeconds(0.3f);
            timer += 0.3f;

            if (timer >= blinkingDuration)
                break;
        }
    }
}