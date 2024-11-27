using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    bool isVulnerable { get; set; }
    void OnDamage();
}
