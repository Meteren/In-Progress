using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : SingleTon<UIController>
{
    [SerializeField] private GameObject key;
    public void ActivateKey()
    {
        key.SetActive(true); 
    }

    public void DiasbleKey()
    {
        key.SetActive(false);
    }
}
