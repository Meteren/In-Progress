using Cinemachine;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public CinemachineVirtualCamera cam;
    CinemachineBasicMultiChannelPerlin channel;
    public PlayerController controller => GameManager.instance.blackBoard.GetValue("PlayerController",out PlayerController _controller) ? _controller : null;
    void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();

        channel  = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        GameManager.instance.blackBoard.SetValue("Channel", channel);
    }

    private void Update()
    {
        if (controller.isDead)
        {
            channel.m_AmplitudeGain = 0f;
        }
    }

}
