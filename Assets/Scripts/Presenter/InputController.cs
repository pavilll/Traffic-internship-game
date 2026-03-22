using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var light = hit.collider.GetComponent<TrafficLightPresenter>();

                if (light != null)
                {
                    light.Switch();
                }
            }
        }
    }
}