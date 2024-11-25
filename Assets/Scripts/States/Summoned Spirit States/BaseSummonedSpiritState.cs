using AdvancedStateHandling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSummonedSpiritState : IState
{
    protected SummonedSpirit spirit; 
    public BaseSummonedSpiritState(SummonedSpirit spirit)
    {
        this.spirit = spirit;
    }

    public virtual void OnExit()
    {
        return;
    }

    public virtual void OnStart()
    {
        return;
    }

    public virtual void Update()
    {
        return;
    }
}
