using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Image avatarOfHealthBar;
    public Animator avatarAnimator;

    protected float maxHealth;
    protected float currentHealth;

    private void Start()
    {
        avatarAnimator = avatarOfHealthBar.GetComponent<Animator>();
    }

    public void SetMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        
    }

    public void SetCurrentHealth(float currentHealth)
    {
        this .currentHealth = currentHealth;
        healthSlider.value = currentHealth;
       
    }


}
