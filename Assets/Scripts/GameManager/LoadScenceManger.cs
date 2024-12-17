using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenceManger : MonoBehaviour
{
    private string nameScenceLoad = "City";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChangeScence(nameScenceLoad));
    }

    IEnumerator ChangeScence(string nameScenceLoad)
    {
        Debug.Log("Waiting ...");
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(nameScenceLoad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
