using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameSuccess : MonoBehaviour
{
    public GameObject UI;
    private void Awake()
    {
        UI.SetActive(false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        { 
            UI.SetActive(true);
        }


    }
}
