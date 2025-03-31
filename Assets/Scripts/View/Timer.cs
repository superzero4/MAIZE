using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public int duration = 15;
    private int timeRemaining;
    public TextMeshProUGUI TimerText;

    public void Start()
    {
        Update();
    }

    void Update()
    {
        TimerText.text = timeRemaining.ToString();
        timeRemaining =  timeRemaining - 1;
    }

    public void reset()
    {
        timeRemaining = duration;
    }


}
