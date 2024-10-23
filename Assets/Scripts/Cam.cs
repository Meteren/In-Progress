using Cinemachine;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public CinemachineVirtualCamera cam;
    void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();

        CinemachineBasicMultiChannelPerlin channel = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        GameManager.instance.blackBoard.SetValue("Channel", channel);
    }

}
