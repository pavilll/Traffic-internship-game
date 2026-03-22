using UnityEngine;

public class CarPresenter : MonoBehaviour
{
    private CarView view;

    private bool blockedByLight = false;

    [SerializeField] private float checkDistance = 5f;

    private void Awake()
    {
        view = GetComponent<CarView>();
    }

    private void Update()
    {
        bool blockedByCar = IsCarAhead();

        // если есть хоть одна причина остановки → стоим
        bool shouldStop = blockedByLight || blockedByCar;

        view.SetMove(!shouldStop);
    }

    public void SetBlockedByLight(bool value)
    {
        blockedByLight = value;
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