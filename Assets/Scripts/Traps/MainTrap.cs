using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTrap : MonoBehaviour
{
    // Start is called before the first frame update
    protected PlayerController controller => 
        GameManager.instance.blackBoard.GetValue("PlayerController", out PlayerController _controller) ? _controller : null;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
