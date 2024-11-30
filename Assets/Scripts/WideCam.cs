using Cinemachine;
using UnityEngine;

public class WideCam : MonoBehaviour
{
    CinemachineVirtualCamera wideCam;
    public PlayerController controller => 
        GameManager.instance.blackBoard.GetValue("PlayerController", out PlayerController _controller) ? _controller : null;
    CinemachineBasicMultiChannelPerlin wideChannel;
    void Start()
    {
        wideCam = GetComponent<CinemachineVirtualCamera>();
        wideChannel = wideCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        GameManager.instance.blackBoard.SetValue("WideChannel", wideChannel);
    }

    private void Update()
    {
        if (controller.isDead)
        {
            wideChannel.m_AmplitudeGain = 0f;
        }
    }

}
