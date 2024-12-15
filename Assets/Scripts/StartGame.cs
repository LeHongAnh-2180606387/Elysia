using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    // Start is called before the first frame update
    async void Awake()
    {
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
        {
            await UnityServices.InitializeAsync();
        }
    }
    void Start()
    {
        SceneManager.LoadScene(1);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
