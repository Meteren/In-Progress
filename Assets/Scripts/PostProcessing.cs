using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessing : MonoBehaviour
{
    public PostProcessVolume volume;
    void Start()
    {
        GameManager.instance.blackBoard.SetValue("PostProcessVolume", volume);
    }

}
