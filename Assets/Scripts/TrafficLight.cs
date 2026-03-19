using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class TrafficLight : MonoBehaviour, IPointerClickHandler
{
    public TrafficLight[] syncedLights;
    public enum LightState
    {
        Red,
        Yellow,
        Green
    }

    public LightState currentState = LightState.Red;

    public float yellowDuration = 2f;

    private bool switching = false;

    public event Action<LightState> OnLightChanged;

    public Renderer redLamp;
    public Renderer yellowLamp;
    public Renderer greenLamp;

    void Start()
    {
        UpdateVisual();
    }

   public void OnPointerClick(PointerEventData eventData)
{
    if (!switching)
    {
        StartCoroutine(SwitchLight());

        foreach (var light in syncedLights)
        {
            if (light != null)
                light.StartSyncedSwitch();
        }
    }
}

    public void StartSyncedSwitch()
{
    if (!switching)
        StartCoroutine(SwitchLight());
}

    IEnumerator SwitchLight()
    {
        switching = true;

        LightState targetState;

        if (currentState == LightState.Green)
            targetState = LightState.Red;
        else
            targetState = LightState.Green;

        currentState = LightState.Yellow;
        UpdateVisual();
        OnLightChanged?.Invoke(currentState);

        yield return new WaitForSeconds(yellowDuration);

        currentState = targetState;
        UpdateVisual();
        OnLightChanged?.Invoke(currentState);

        switching = false;
    }

    void UpdateVisual()
    {
        SetLamp(redLamp, currentState == LightState.Red, Color.red);
        SetLamp(yellowLamp, currentState == LightState.Yellow, Color.yellow);
        SetLamp(greenLamp, currentState == LightState.Green, Color.green);
    }

    void SetLamp(Renderer lamp, bool active, Color color)
    {
        if (lamp == null) return;

        Material mat = lamp.material;

        if (active)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", color * 5f);
        }
        else
        {
            mat.SetColor("_EmissionColor", Color.black);
        }
    }

    public bool IsGreen()
    {
        return currentState == LightState.Green;
    }
}