using UnityEngine;

[RequireComponent(typeof(CarView))]
public class CarPresenter : MonoBehaviour
{
    private CarView view;
    private TrafficLightState currentLightState;
    private bool isInZone;
    private bool enteredBeforeYellow;

    [SerializeField] private float checkDistance = 6f;
    [SerializeField] private float stopBuffer = 1.2f;
    [SerializeField] private float slowDownDistance = 5f;
    [SerializeField] private float minFollowGap = 1.4f;
    [SerializeField] private LayerMask carLayer = 1 << 3;

    private void Awake()
    {
        view = GetComponent<CarView>();

        if (view == null)
        {
            Debug.LogError("CarPresenter: CarView component is missing.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (view == null)
        {
            return;
        }

        float speedFactor = GetCarAheadSpeedFactor();
        bool blockedByLight = ShouldStopByLight();

        view.SetMove(!blockedByLight);
        view.SetSpeedFactor(blockedByLight ? 0f : speedFactor);
    }

    // вызывается при входе в триггер
    public void OnEnterZone(TrafficLightState state)
    {
        isInZone = true;
        currentLightState = state;

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
                return !enteredBeforeYellow;

            case TrafficLightState.Red:
                return true;
        }

        return false;
    }

    private float GetCarAheadSpeedFactor()
    {
        float frontExtent = view != null ? view.GetForwardExtent() : 1f;
        float sideExtent = view != null ? view.GetSideExtent() : 0.5f;
        float sphereRadius = Mathf.Max(0.35f, sideExtent * 0.6f);
        Vector3 origin = transform.position + Vector3.up * 0.5f + transform.forward * Mathf.Max(0f, frontExtent - sphereRadius);

        if (Physics.SphereCast(origin, sphereRadius, transform.forward, out RaycastHit hit, checkDistance + stopBuffer, carLayer, QueryTriggerInteraction.Ignore))
        {
            CarView otherView = hit.collider.GetComponent<CarView>();

            if (otherView == null)
            {
                otherView = hit.collider.GetComponentInParent<CarView>();
            }

            if (otherView != null && otherView.gameObject != gameObject)
            {
                float gapToOtherCar = hit.distance;
                if (gapToOtherCar <= minFollowGap)
                {
                    return 0f;
                }

                if (gapToOtherCar >= slowDownDistance)
                {
                    return 1f;
                }

                float normalizedGap = Mathf.InverseLerp(minFollowGap, slowDownDistance, gapToOtherCar);
                return Mathf.SmoothStep(0f, 1f, normalizedGap);
            }
        }

        return 1f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawLine(origin, origin + transform.forward * (checkDistance + stopBuffer));
    }
}
