using UnityEngine;

public class CarPresenter : MonoBehaviour
{
    private CarView view;

    private TrafficLightState currentLightState;

    private bool isInZone = false;
    private bool enteredBeforeYellow = false;

    [SerializeField] private float checkDistance = 5f;

    private void Awake()
    {
        view = GetComponent<CarView>();
    }

    private void Update()
    {
        bool blockedByCar = IsCarAhead();
        bool blockedByLight = ShouldStopByLight();

        view.SetMove(!(blockedByCar || blockedByLight));
    }

    // вызывается при входе в триггер
    public void OnEnterZone(TrafficLightState state)
    {
        isInZone = true;
        currentLightState = state;

        // ключевая логика
        enteredBeforeYellow = (state == TrafficLightState.Green || 
                               state == TrafficLightState.BlinkingGreen);
    }

    public void OnExitZone()
    {
        isInZone = false;
        enteredBeforeYellow = false;
    }

    public void SetLightState(TrafficLightState state)
    {
        currentLightState = state;
    }

    private bool ShouldStopByLight()
    {
        if (!isInZone) return false;

        switch (currentLightState)
        {
            case TrafficLightState.Green:
            case TrafficLightState.BlinkingGreen:
                return false;

            case TrafficLightState.Yellow:
                // 🔥 ВОТ ТУТ ВЕСЬ СМЫСЛ
                return !enteredBeforeYellow;

            case TrafficLightState.Red:
                return true;
        }

        return false;
    }

    private bool IsCarAhead()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(origin, transform.forward, out RaycastHit hit, checkDistance))
        {
            if (hit.collider.GetComponent<CarView>() != null &&
                hit.collider.gameObject != gameObject)
            {
                return true;
            }
        }

        return false;
    }
}