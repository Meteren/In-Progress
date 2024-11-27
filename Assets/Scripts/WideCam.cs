using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WideCam : MonoBehaviour
{
    CinemachineVirtualCamera wideCam;
    void Start()
    {
        wideCam = GetComponent<CinemachineVirtualCamera>();
        CinemachineBasicMultiChannelPerlin wideChannel = wideCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        GameManager.instance.blackBoard.SetValue("WideChannel", wideCam);
    }

}
