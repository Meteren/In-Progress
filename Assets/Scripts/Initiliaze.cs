using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initiliaze : MonoBehaviour
{
    public void ActivateCanvas()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            GameManager.instance.init = true;
        }
            
            
    }
}
