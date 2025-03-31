using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public int duration = 15;
    private int timeRemaining;
    private float timeRemainingFloat;
    public TextMeshProUGUI TimerText;

    public void Start()
    {
        Update();
    }

    void Update()
    {
        TimerText.text = timeRemaining.ToString();
        timeRemainingFloat =  timeRemainingFloat - Time.deltaTime;
        timeRemaining = (int)timeRemainingFloat;
    }

    public void reset()
    {
        timeRemaining = duration;
        timeRemainingFloat = (float)duration;
    }


}
