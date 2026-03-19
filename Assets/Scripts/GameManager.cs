using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI accidentText;

    public int maxAccidents = 3;

    void Update()
    {
        accidentText.text = "Счетчик ДТП: " + CarController.AccidentCount + " / " + maxAccidents;

        if (CarController.AccidentCount >= maxAccidents)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Time.timeScale = 0f;

        accidentText.text = "GAME OVER\nДТП: " + CarController.AccidentCount;
    }
}