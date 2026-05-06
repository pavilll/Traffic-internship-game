using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float maxRayDistance = 300f;

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Mouse.current == null || targetCamera == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = targetCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                var light = hit.collider.GetComponent<TrafficLightPresenter>();

                if (light == null)
                {
                    light = hit.collider.GetComponentInParent<TrafficLightPresenter>();
                }

                if (light != null)
                {
                    light.Switch();
                }
            }
        }
    }
}
