using UnityEngine;
using UnityEngine.UI;

public class DashBar : MonoBehaviour
{
    [SerializeField] private Slider dashBarSlider;
    [SerializeField] private PlayerController playerController;
    float maxVaue = 1f;
    float currentValue = 1f;
    float fillSpeed = 1f;


    private void Start()
    {
        dashBarSlider.maxValue = maxVaue;
        dashBarSlider.value = currentValue;
    }

    private void Update()
    {
        if (playerController.dashInCoolDown)
        {
            IncrementDashBar();
           
        }
    }

    private void IncrementDashBar()
    {
        if(currentValue >= maxVaue)
        {
            playerController.dashInCoolDown = false;
            return;
        }
        currentValue += Time.deltaTime / fillSpeed;
        dashBarSlider.value = currentValue;
    }

    public void SetDashBarToZero()
    {
        currentValue = 0f;
        dashBarSlider.value = currentValue;
    }
}
