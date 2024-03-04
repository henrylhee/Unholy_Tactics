using UnityEngine;

[CreateAssetMenu(fileName = "DamageDot", menuName = "Buffs/DamageDot", order = 2)]
public class DamageDot : DotEffect
{
    [Tooltip("Damage that is applied each time")]
    public float damage;
    [Tooltip("Whenever damage is applied it will be multiplied by this value")]
    public float damageMultiplier;

    public override void OnApply(Unit target)
    {
        target.TakeDamage((int) damage, false);

        damage *= damageMultiplier;
    }
}
