using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealingDot", menuName = "Buffs/HealingDot", order = 2)]
public class HealingDot : DotEffect
{
    [Tooltip("Heal that is applied each time")]
    public float heal;
    [Tooltip("Whenever Heal is applied it will be multiplied by this value")]
    public float healMultiplier = 1;

    public override void OnApply(Unit target)
    {
        target.HealDamage((int)heal);

        heal *= healMultiplier;
    }
}
