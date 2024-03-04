using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Windows;

public class AIParty : Party
{
    private List<AIUnit> unusedUnits = new List<AIUnit>();
    private List<UnitSpawner> spawners = new List<UnitSpawner>();

    private Inputs input;

    public override void SetUp(Inputs input)
    {
        this.input = input;
        manaCount = int.MaxValue;
        base.SetUp(input);
        SetUpSpawners(input);
    }

    private void AfterFirstMove(AIUnit unit)
    {
        unit.Act(GetAllies(), GetEnemies());
    }

    private void AfterAction(AIUnit unit)
    {
        unit.SecondMovement(GetAllies(), GetEnemies());
    }

    private void AfterSecondMove(AIUnit unit)
    {
        StartCoroutine(delay());
        IEnumerator delay()
        {
            yield return new WaitForFixedUpdate();
            unit.Deselect();
            unusedUnits.Remove(unit);

            if (unusedUnits.Count > 0)
            {
                unusedUnits[0].Select();
                unusedUnits[0].FirstMovement(GetAllies(), GetEnemies());
            }
            else
            {
                Combat.Instance.EndTurn();
                Combat.EnableCombatCamera();
            }
        }
     
    }

    private List<Unit> GetAllies()
    {
        return units;
    }
    private List<Unit> GetEnemies()
    {
        List<Unit> result = new List<Unit>();

        foreach(Party p in Combat.GetOtherParties(this))
        {
            result.AddRange(p.GetUnits());
        }

        return result;
    }

    #region Overrides
    public override void SetUpUnit(Unit toRegister)
    {
        base.SetUpUnit(toRegister);

        ((AIUnit)toRegister).finishedActing.AddListener(AfterAction);
        ((AIUnit)toRegister).finishedFirstMovement.AddListener(AfterFirstMove);
        ((AIUnit)toRegister).finishedSecondMovement.AddListener(AfterSecondMove);
    }

    public override void SetActive()
    {
        input.Turn.End.Disable();
        Combat.DisableCombatCamera();

        StartCoroutine(delay());

        IEnumerator delay()
        {
            foreach (UnitSpawner spawn in spawners)
            {
                spawn.CountDown();
                while (spawn.IsSpawning())
                {
                    yield return null;
                }
            }

            foreach (UnitSpawner spawn in spawners)
            {
                if (spawn.AboutToSpawn())
                {
                    while (spawn.IsPreviewing())
                    {
                        yield return null;
                    }
                }
            }

            if (units.Count <= 0)
            {
                input.Turn.End.Enable();
                Combat.Instance.EndTurn();
                yield break;
            }

            base.SetActive();

            foreach (AIUnit u in units)
            {
                unusedUnits.Add(u);
            }

            yield return new WaitForSeconds(0.5f);
            yield return new WaitForFixedUpdate();

            unusedUnits[0].Select();
            unusedUnits[0].FirstMovement(GetAllies(), GetEnemies());
        }
    }

    public override void SetInactive()
    {
        input.Turn.End.Enable();
        base.SetInactive();
    }

    protected override void OnUnitDie(Unit Unit, int souls)
    {
        if (units.Count <= 1 && SomethingToSpawn())
        {
            while (!SpawnSoon())
            {
                CountDownSpawners();
            }
        }

        if (SpawnSoon() && SomethingToSpawn())
        {
            units.Remove(Unit);
            return;
        }

        base.OnUnitDie(Unit, souls);
    }
    #endregion

    private void CountDownSpawners()
    {
        foreach(UnitSpawner spawner in spawners)
        {
            spawner.CountDown();
        }
    }

    private bool SpawnSoon()
    {
        foreach(UnitSpawner spawner in spawners)
        {
            if(spawner.turnsUntilSpawn <= 2)
            {
                return true;
            }
        }

        return false;
    }

    private bool SomethingToSpawn()
    {
        if(spawners.Count <= 0)
        {
            return false;
        }

        foreach(UnitSpawner spawner in spawners)
        {
            if(spawner.turnsUntilSpawn > 0)
            {
                return true;
            }
        }

        return false;
    }

    #region SetUps
    private void SetUpSpawners(Inputs input)
    {
        foreach (UnitSpawner spawner in GetComponentsInChildren<UnitSpawner>())
        {
            spawner.SetUp(this, input);
            spawners.Add(spawner);
        }
    }
    #endregion
}
