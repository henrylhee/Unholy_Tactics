using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class QueenInfoPanel : MonoBehaviour
{
    [SerializeField]
    private HealthBar healthBar;
    [SerializeField]
    private HealthBar manaBar;

    private MainUnit main;
    private Party p;

    public void SetUp(MainUnit unit)
    {
        main = unit;
        
        List<Party> parties = new List<Party>();
        parties.AddRange(Combat.GetInActiveParties());
        parties.Add(Combat.GetActiveParty());
        foreach(Party party in parties)
        {
            if(party == null)
            {
                continue;
            }

            if(party.IsInParty(main))
            {
                p = party;
            }
        }


        main.health.DamageTaken.AddListener(UpdateHealthBar);
        p.ManaUse.AddListener(UpdateManaBar);
        p.ManaGained.AddListener(UpdateManaBar);

        manaBar.UpdateHealth(p.manaCount, p.maxMana);

        UpdateManaBar();
        UpdateHealthBar(0);
    }

    private void UpdateHealthBar(int amount)
    {
        healthBar.UpdateHealth(main.health.CurrentHp, main.health.MaxHp);
    }

    public void UpdateManaBar()
    {
        manaBar.UpdateHealth(p.manaCount, p.maxMana);
    }
}
