using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitCamera : MonoBehaviour
{

    public GameObject Player;
    // Start is called before the first frame update
    void Start()
    {

    }
    // IEnumerator 
    // Update is called once per frame
    void Update()
    {
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }
    }
    private void LateUpdate()
    {
        if (Player != null)
        {
            transform.position = new Vector3(Player.transform.position.x, 40, Player.transform.position.z);
        }

    }


}