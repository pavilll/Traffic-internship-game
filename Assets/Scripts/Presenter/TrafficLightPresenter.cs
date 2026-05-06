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
    private bool isSwitching;
    private Coroutine blinkRoutine;

    [Header("Timings")]
    [SerializeField] private float blinkingDuration = 2f;
    [SerializeField] private float yellowDuration = 1f;

    [Header("Renderer Fallback")]
    [SerializeField] private Renderer signalRenderer;
    [SerializeField] private string rendererNameHint = "Traffic";

    [SerializeField] private float intensity = 5f;

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");

    private void Awake()
    {
        if (signalRenderer == null)
        {
            signalRenderer = GetComponent<Renderer>();
        }

        if (signalRenderer == null)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

            foreach (Renderer childRenderer in renderers)
            {
                if (childRenderer.name.Contains(rendererNameHint))
                {
                    signalRenderer = childRenderer;
                    break;
                }
            }

            if (signalRenderer == null && renderers.Length > 0)
            {
                signalRenderer = renderers[0];
            }
        }
    }

    private void Start()
    {
        SetStateVisual(state);
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

        state = TrafficLightState.BlinkingGreen;
        SetStateVisual(state);
        blinkRoutine = StartCoroutine(BlinkGreen());
        yield return new WaitForSeconds(blinkingDuration);

        state = TrafficLightState.Yellow;
        SetStateVisual(state);
        yield return new WaitForSeconds(yellowDuration);

        state = TrafficLightState.Red;
        SetStateVisual(state);

        isSwitching = false;
    }

    private IEnumerator SwitchToGreenSequence()
    {
        isSwitching = true;
        StopBlinking();

        state = TrafficLightState.Yellow;
        SetStateVisual(state);
        yield return new WaitForSeconds(yellowDuration);

        state = TrafficLightState.Green;
        SetStateVisual(state);

        isSwitching = false;
    }

    private void SetStateVisual(TrafficLightState newState)
    {
        if (signalRenderer == null)
        {
            return;
        }

        switch (newState)
        {
            case TrafficLightState.Green:
            case TrafficLightState.BlinkingGreen:
                SetLamp(signalRenderer, true, Color.green);
                break;
            case TrafficLightState.Yellow:
                SetLamp(signalRenderer, true, Color.yellow);
                break;
            default:
                SetLamp(signalRenderer, true, Color.red);
                break;
        }
    }

    private void SetLamp(Renderer lamp, bool active, Color color)
    {
        if (lamp == null) return;

        var mat = lamp.material;

        mat.EnableKeyword("_EMISSION");

        if (active)
        {
            if (mat.HasProperty(BaseColor))
            {
                mat.SetColor(BaseColor, color);
            }
            else if (mat.HasProperty(ColorProperty))
            {
                mat.SetColor(ColorProperty, color);
            }

            if (mat.HasProperty(EmissionColor))
            {
                mat.SetColor(EmissionColor, color * intensity);
            }
        }
        else
        {
            if (mat.HasProperty(EmissionColor))
            {
                mat.SetColor(EmissionColor, Color.black);
            }
        }
    }

    private IEnumerator BlinkGreen()
    {
        float timer = 0f;
        bool on = true;

        while (state == TrafficLightState.BlinkingGreen)
        {
            SetLamp(signalRenderer, on, Color.green);

            on = !on;

            yield return new WaitForSeconds(0.3f);
            timer += 0.3f;

            if (timer >= blinkingDuration)
                break;
        }

        blinkRoutine = null;
    }

    private void StopBlinking()
    {
        if (blinkRoutine == null)
        {
            return;
        }

        StopCoroutine(blinkRoutine);
        blinkRoutine = null;
    }

    private void OnDisable()
    {
        StopBlinking();
    }
}
