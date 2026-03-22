using UnityEngine;

public class CarPresenter : MonoBehaviour
{
    private CarView view;

    private void Awake()
    {
        view = GetComponent<CarView>();
    }
}