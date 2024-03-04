
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerParty : Party
{
    public float unitSwitchDelay = 0.5f;
    
    private PlayerUnit selectedUnit;
    private Unit hoveredUnit;

    private PartyUI ui;

    public override void SetUp(Inputs input)
    {
        base.SetUp(input);

        SetUpUI();
        SetUpInputs(input);
    }


    private void SelectNextUnit(InputAction.CallbackContext context)
    {
        if (!active || IsResolving())
        {
            return;
        }

        int index = units.IndexOf(selectedUnit) + 1;

        if (index > units.Count - 1)
        {
            index = 0;
        }

        units[index].Select();
        inputs.Party.Disable();
        Invoke(nameof(EnableInputs), unitSwitchDelay);
    }

    private void SelectPreviousUnit(InputAction.CallbackContext context)
    {
        if (!active || IsResolving())
        {
            return;
        }

        int index = units.IndexOf(selectedUnit) - 1;

        if (index < 0)
        {
            index = units.Count - 1;
        }

        units[index].Select();
        inputs.Party.Disable();
        Invoke(nameof(EnableInputs), unitSwitchDelay);
    }

    private void MoveHover(InputAction.CallbackContext context)
    {
        if (!active)
        {
            return;
        }

        hoveredUnit?.HoverEnd();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            hoveredUnit = hit.transform.GetComponent<Unit>();
        }

        hoveredUnit?.HoverStart(units.Contains(hoveredUnit));
    }

    private void OnSelectUnit(PlayerUnit toSelect)
    {
        selectedUnit = toSelect;

        foreach (PlayerUnit unit in units)
        {
            if(unit != toSelect)
            {
                unit.Deselect();
            }
        }
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    private bool IsResolving()
    {
        foreach (PlayerUnit u in units)
        {
            if (u.IsResolving())
            {
                return true;
            }
        }

        return false;
    }

    #region Overrides

    public override void SetActive()
    {
        base.SetActive();
        Combat.EnableCombatCamera();

        ui.StartTurn();
        units[0].Select();
    }

    public override void ActiveUpdate()
    {
        if (!active)
        {
            return;
        }

        selectedUnit?.SelectedUpdate();
    }

    public override void SetInactive()
    {
        base.SetInactive();

        StartCoroutine(delay());
        IEnumerator delay()
        {
            yield return new WaitForFixedUpdate();
            selectedUnit?.Deselect();
            selectedUnit = null;
        }
    }

    public override void GainMana(int toGain)
    {
        base.GainMana(toGain);
        ui.UpdateManaCount(manaCount, maxMana);

        foreach (PlayerUnit unit in units)
        {
            unit.OnRessourceUpdated(this);
        }
    }

    public override bool SpendMana(int toSpend)
    {
        if (base.SpendMana(toSpend))
        {
            ui.UpdateManaCount(manaCount, maxMana);

            foreach (PlayerUnit unit in units)
            {
                unit.OnRessourceUpdated(this);
            }

            return true;
        }
        return false;
    }

    protected override void OnUnitDie(Unit Unit, int souls)
    {
        ui.RemovePartyUnit((PlayerUnit)Unit);
        base.OnUnitDie(Unit, souls);
    }

    #endregion

    public void SelectUnit1(InputAction.CallbackContext context)
    {
        if (units.Count < 1)
            return;
        units[0].Select();
        inputs.Party.Disable();
        Invoke(nameof(EnableInputs), unitSwitchDelay);
    }
    
    public void SelectUnit2(InputAction.CallbackContext context)
    {
        if (units.Count < 2)
            return;
        units[1].Select();
        inputs.Party.Disable();
        Invoke(nameof(EnableInputs), unitSwitchDelay);
    }
    
    public void SelectUnit3(InputAction.CallbackContext context)
    {
        if (units.Count < 3)
            return;
        units[2].Select();
        inputs.Party.Disable();
        Invoke(nameof(EnableInputs), unitSwitchDelay);
    }
    
    public void SelectUnit4(InputAction.CallbackContext context)
    {
        if (units.Count < 4)
            return;
        units[3].Select();
        inputs.Party.Disable();
        Invoke(nameof(EnableInputs), unitSwitchDelay);
    }
    
    #region SetUpFunctions
    private void SetUpUI()
    {
        ui = GetComponentInChildren<PartyUI>();
        ui.UpdateManaCount(manaCount, maxMana);

        foreach (Unit u in units)
        {
            ui.AddPartyUnit((PlayerUnit)u);
        }
    }

    private void SetUpInputs(Inputs input)
    {
        input.Party.Enable();
        input.Party.Select_1.performed += SelectUnit1;
        input.Party.Select_2.performed += SelectUnit2;
        input.Party.Select_3.performed += SelectUnit3;
        input.Party.Select_4.performed += SelectUnit4;
        
        input.Party.Select_Next.performed += SelectNextUnit;
        input.Party.Select_Previous.performed += SelectPreviousUnit;
        input.Unit.HoverMovement.performed += MoveHover;
        inputs = input;
    }

    void EnableInputs()
    {
        inputs.Party.Enable();
    }
    
    public override void SetUpUnit(Unit toRegister)
    {
        base.SetUpUnit(toRegister);

        PlayerUnit unit = (PlayerUnit)toRegister;

        unit.Selected.AddListener(OnSelectUnit);
        unit.OnRessourceUpdated(this);

        ui?.AddPartyUnit((PlayerUnit)toRegister);
    }
    #endregion
}
