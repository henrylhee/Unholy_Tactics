
using UnityEngine;

public class EnemyPlayerUnit : PlayerUnit
{
    [Header("XP")]
    [SerializeField]
    private int xp;

    protected override void OnDeath()
    {
        base.OnDeath();
    }
}
