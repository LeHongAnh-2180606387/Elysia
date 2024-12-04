using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    [SerializeField] private Texture2D skyboxNight;
    [SerializeField] private Texture2D skyboxSunrise;
    [SerializeField] private Texture2D skyboxDay;
    [SerializeField] private Texture2D skyboxSunset;
    [SerializeField] private Texture2D skyboxRainy;

    [SerializeField] private ParticleSystem rainParticleSystem;
    [SerializeField] private float timeMultiplier;
    [SerializeField] private float startHour;
    [SerializeField] private TextMeshProUGUI timeText;

    private DateTime currentTime;
    private bool isRaining = false;
    private bool isSunny = true;
    private bool isTransitioning = false;


    private bool hasTransitionedToSunrise = false;
    private bool hasTransitionedToDay = false;
    private bool hasTransitionedToSunset = false;
    private bool hasTransitionedToNight = false;


    public void Update()
    {
        UpdateTimeOfDay();
    }


    private IEnumerator LerpSkybox(Texture2D a, Texture2D b, float duration, float initialBlend = 0f)
    {
        // Đặt texture skybox ban đầu và đích đến
        RenderSettings.skybox.SetTexture("_Texture1", a);
        RenderSettings.skybox.SetTexture("_Texture2", b);
        RenderSettings.skybox.SetFloat("_Blend", initialBlend);

        isTransitioning = true;

        for (float t = initialBlend * duration; t < duration; t += Time.deltaTime)
        {
            RenderSettings.skybox.SetFloat("_Blend", t / duration);
            yield return null;
        }

        RenderSettings.skybox.SetTexture("_Texture1", b);
        RenderSettings.skybox.SetFloat("_Blend", 0);

        isTransitioning = false;
    }

    void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
        StartCoroutine(WeatherCycle());
        SetInitialSkyboxBlend();
    }

    private void SetInitialSkyboxBlend()
    {
        float blend = 0f;
        Texture2D initialSkybox = skyboxNight;
        Texture2D targetSkybox = skyboxNight;  // Mặc định là skybox đêm

        // Chỉnh sửa cách tính blend dựa trên giờ và phút liên tục
        if (currentTime.Hour >= 5 && currentTime.Hour < 7)
        {
            if (!hasTransitionedToSunrise) // Chỉ chuyển đổi khi chưa chuyển sang sáng
            {
                initialSkybox = skyboxNight;
                targetSkybox = skyboxSunrise;
                blend = (float)(currentTime.Hour - 5 + currentTime.Minute / 60f) / 2f;
                StartCoroutine(LerpSkybox(initialSkybox, targetSkybox, 10f, blend));
                hasTransitionedToSunrise = true;
            }
        }
        else if (currentTime.Hour >= 7 && currentTime.Hour < 16)
        {
            if (!hasTransitionedToDay) // Chỉ chuyển đổi khi chưa chuyển sang ngày
            {
                initialSkybox = skyboxSunrise;
                targetSkybox = skyboxDay;
                blend = (float)(currentTime.Hour - 7 + currentTime.Minute / 60f) / 2f;
                StartCoroutine(LerpSkybox(initialSkybox, targetSkybox, 10f, blend));
                hasTransitionedToDay = true;
            }
        }
        else if (currentTime.Hour >= 16 && currentTime.Hour < 18)
        {
            if (!hasTransitionedToSunset) // Chỉ chuyển đổi khi chưa chuyển sang hoàng hôn
            {
                initialSkybox = skyboxDay;
                targetSkybox = skyboxSunset;
                blend = (float)(currentTime.Hour - 16 + currentTime.Minute / 60f) / 2f;
                StartCoroutine(LerpSkybox(initialSkybox, targetSkybox, 10f, blend));
                hasTransitionedToSunset = true;
            }
        }
        else if (currentTime.Hour >= 18)
        {
            if (!hasTransitionedToNight) // Chỉ chuyển đổi khi chưa chuyển sang đêm
            {
                initialSkybox = skyboxSunset;
                targetSkybox = skyboxNight;
                blend = (float)(currentTime.Hour - 18 + currentTime.Minute / 60f) / 2f;
                StartCoroutine(LerpSkybox(initialSkybox, targetSkybox, 10f, blend));
                hasTransitionedToNight = true;
            }
        }

        // Thiết lập skybox và blend ban đầu mà không bắt đầu chuyển đổi nữa
        RenderSettings.skybox.SetTexture("_Texture1", initialSkybox);
        RenderSettings.skybox.SetTexture("_Texture2", targetSkybox);
        RenderSettings.skybox.SetFloat("_Blend", blend);
    }



    private void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);

        if (timeText != null)
        {
            timeText.text = currentTime.ToString("HH:mm");
        }

        if (!isRaining && !isTransitioning)
        {
            float currentBlend = RenderSettings.skybox.GetFloat("_Blend");

            int currentHour = currentTime.Hour;
            int currentMinute = currentTime.Minute;

            if (currentHour == 5 && !hasTransitionedToSunrise)
            {
                StartCoroutine(LerpSkybox(skyboxNight, skyboxSunrise, 10f, currentBlend));
                hasTransitionedToSunrise = true;  // Đảm bảo rằng chỉ chuyển đổi 1 lần
                hasTransitionedToDay = false;
            }
            else if (currentHour == 7 && !hasTransitionedToDay)
            {
                StartCoroutine(LerpSkybox(skyboxSunrise, skyboxDay, 10f, currentBlend));
                hasTransitionedToDay = true;
                hasTransitionedToSunset = false;
            }
            else if (currentHour == 16 && !hasTransitionedToSunset)
            {
                StartCoroutine(LerpSkybox(skyboxDay, skyboxSunset, 10f, currentBlend));
                hasTransitionedToSunset = true;
                hasTransitionedToNight = false;
            }
            else if (currentHour == 18 && !hasTransitionedToNight)
            {
                StartCoroutine(LerpSkybox(skyboxSunset, skyboxNight, 10f, currentBlend));
                hasTransitionedToNight = true;
                hasTransitionedToSunrise = false;
            }
        }
    }



    private IEnumerator WeatherCycle()
    {
        while (true)
        {
            // Chờ 5 giờ (hoặc thời gian tương ứng)
            yield return new WaitForSeconds(5 * 60 * 60 / timeMultiplier);

            // Xác suất mưa (30% cơ hội mưa) và kiểm tra giờ
            if (UnityEngine.Random.value < 0.3f && !(currentTime.Hour >= 4 && currentTime.Hour < 8))
            {
                if (!isRaining) // Chỉ bắt đầu mưa nếu chưa có mưa
                {
                    isRaining = true;
                    isSunny = false;
                    StartCoroutine(StartRainEvent()); // Bắt đầu sự kiện mưa
                }
            }
            else
            {
                if (isRaining) // Chỉ dừng mưa nếu đang mưa
                {
                    isSunny = true;
                    isRaining = false;
                    StopRain(); // Dừng mưa
                }
            }
        }
    }


    private IEnumerator StartRainEvent()
    {
        if (isRaining && !isTransitioning)
        {
            Texture2D fromSkybox = currentTime.Hour >= 5 && currentTime.Hour < 7 ? skyboxSunrise :
                                   currentTime.Hour >= 7 && currentTime.Hour < 16 ? skyboxDay :
                                   currentTime.Hour >= 16 && currentTime.Hour < 18 ? skyboxSunset :
                                   skyboxNight;

            StartCoroutine(LerpSkybox(fromSkybox, skyboxRainy, 10f));
        }

        rainParticleSystem.Play();
        var rainEmission = rainParticleSystem.emission;
        rainEmission.rateOverTime = 100f;

        float startRate = 100f;
        float targetRate = 1000f;
        float duration = 10f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float currentRate = Mathf.Lerp(startRate, targetRate, t / duration);
            rainEmission.rateOverTime = currentRate;
            yield return null;
        }

        rainEmission.rateOverTime = targetRate;
        yield return new WaitForSeconds(2 * 60 * 60 / timeMultiplier);

        isRaining = false;
        isSunny = true;
        StopRain();
    }


    private void StopRain()
    {
        rainParticleSystem.Stop(); // Dừng mưa

        // Kiểm tra xem trời có đang mưa không trước khi thay đổi skybox
        if (!isRaining && !isTransitioning)
        {
            if (currentTime.Hour >= 5 && currentTime.Hour < 8)
            {
                StartCoroutine(LerpSkybox(skyboxRainy, skyboxSunrise, 5f));
            }
            else if (currentTime.Hour >= 7 && currentTime.Hour < 16)
            {
                StartCoroutine(LerpSkybox(skyboxRainy, skyboxDay, 5f));
            }
            else if (currentTime.Hour >= 16 && currentTime.Hour < 18)
            {
                StartCoroutine(LerpSkybox(skyboxRainy, skyboxSunset, 5f));
            }
            else if (currentTime.Hour >= 18 || currentTime.Hour < 6)
            {
                StartCoroutine(LerpSkybox(skyboxRainy, skyboxNight, 5f));
            }
        }
    }

}
