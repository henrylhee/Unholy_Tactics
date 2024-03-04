using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Combat : MonoBehaviour
{
    public static Combat Instance;
     
    private List<Party> parties;
    private Party activeParty;

    private Inputs input; 

    [SerializeField] private CombatUI ui;
    [SerializeField] private CombatCamera combatCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Time.timeScale = 1;
    }

    private void Start()
    {
        SetUpInputs();
        SetUpParties();
        SetUpCamera();
        SetUpUI();

        StartTurn();
    }

    private void Update()
    {
        activeParty?.ActiveUpdate();
    }

    public void EndTurn()
    {
        if (!input.Turn.End.enabled)
        {
            return;
        }

        activeParty.SetInactive();
        combatCamera.DeActivate(new InputAction.CallbackContext());
        StartTurn();
    }

    private void StartTurn()
    {
        StartCoroutine(delay());
        IEnumerator delay()
        {
            yield return new WaitForSeconds(0.25f);
            int nextIndex = parties.IndexOf(activeParty) + 1;
            if (nextIndex > parties.Count - 1)
            {
                nextIndex = 0;
            }

            activeParty = parties[nextIndex];
            activeParty.SetActive();
        }
    }


    private void OnPartyDeath(Party party)
    {
        if (party == parties[0])
        {
            Lose();
        }
        else
        {
            Win();
        }
    }

    public static void Lose()
    {
        Combat.Instance.ui.ShowGameOverScreen();
    }

    public static void Win()
    {
        Combat.Instance.ui.ShowVictoryVFXScreen();
    }

    #region Static Functions
    public static void ResetFreeCam()
    {
        Combat.Instance.combatCamera.DeActivate(new InputAction.CallbackContext());
    }

    public static void DisableCombatCamera()
    {
        ResetFreeCam();
        Combat.Instance.input.CameraMovement.Disable();
    }

    public static void EnableCombatCamera()
    {
        Combat.Instance.input.CameraMovement.Enable();
    }

    public static Party GetActiveParty()
    {
        return Combat.Instance.activeParty;
    }

    public static List<Party> GetInActiveParties()
    {
        List<Party> opposingParties = new List<Party>();
        foreach(Party p in Combat.Instance.parties)
        {
            if(p != Combat.GetActiveParty())
            {
                opposingParties.Add(p);
            }
        }

        return opposingParties; 
    } 

    public static List<Party> GetOtherParties(Party exception)
    {
        List<Party> others = new List<Party>();
        foreach(Party p in Combat.Instance.parties)
        {
            if(p != exception)
            {
                others.Add(p);
            }
        }

        return others;
    }

    #endregion

    #region Setup Functions
    private void SetUpInputs()
    {
        input = new Inputs();

        input.Unit.Enable();
        input.CameraMovement.Enable();
        input.Turn.End.Enable();
    }

    private void SetUpParties()
    {
        parties = new List<Party>();
        foreach (var party in GetComponentsInChildren<Party>())
        {
            parties.Add(party);
            party.Die.AddListener(OnPartyDeath);
            party.SetUp(input);
        }

        activeParty = parties[parties.Count - 1];
    }

    private void SetUpCamera()
    {
        combatCamera.SetUp(input);
    }

    private void SetUpUI()
    {
        ui.SetUp(input);
    }
    #endregion
}
