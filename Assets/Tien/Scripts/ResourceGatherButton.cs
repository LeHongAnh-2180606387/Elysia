using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceGatherButton : MonoBehaviour
{
    [SerializeField] private Button gatherButton;
    [SerializeField] private AudioSource pickUpSound;

    private EventService eventService;

    public void Init(EventService eventService)
    {
        this.eventService = eventService;

        gatherButton.onClick.AddListener(OnGatherButtonClicked);
    }

    private void OnGatherButtonClicked()
    {
        eventService.OnResourceGather.InvokeEvent();
        pickUpSound.Play();
    }
}
