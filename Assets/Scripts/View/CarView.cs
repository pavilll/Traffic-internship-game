using UnityEngine;

public class CarView : MonoBehaviour
{
    public float speed = 5f;
    private bool canMove = true;

    public void SetMove(bool value)
    {
        canMove = value;
    }

    private void Update()
    {
        if (canMove)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }
}