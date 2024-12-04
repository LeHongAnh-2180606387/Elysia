using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{

    [SerializeField] private float timeMultiplier;
    [SerializeField] private float startHour;
    [SerializeField] private TextMeshProUGUI timeText;

    private DateTime currentTime;


    // Start is called before the first frame update
    void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimeOfDay();
    }

    private void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);

        if (timeText != null )
        {
            timeText.text = currentTime.ToString("HH:mm"); // HH gio, mm phut
        }
    }
}
