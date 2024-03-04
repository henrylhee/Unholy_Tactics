using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementRoom : MonoBehaviour
{
    public GameObject door;
    public TaskList list;
    public MainUnit main;
    public PopWindow startTurnWindow;

    //private bool pressured;
    private bool turnEnded;
    private bool moved;
    private bool cameraemoved;
    private bool cameraTilted;
    private bool startTurnShowed = false;

    private void Awake()
    {
        main.AnyResolved.AddListener(OnMove);
        FindObjectOfType<CombatCamera>().moved.AddListener(OnCameraMoved);
        FindObjectOfType<CombatCamera>().tilted.AddListener(OnCameraTilted);
    }

    private void Start()
    {
        StartCoroutine(delay());
        IEnumerator delay()
        {
            yield return new WaitForSeconds(0.1f);
            FindObjectOfType<MainUnit>().stats.autoManaGeneration = 0;
            while (FindObjectOfType<PlayerParty>().CanSpendMana(1))
            {
                FindObjectOfType<PlayerParty>().SpendMana(1);
            }
            FindObjectOfType<PlayerParty>().SpendMana(1);
        }
       

    }


    public void OnPressured()
    {
        //pressured = true;
        list.CrossPoint(4, 1);
        TryToOpenDoor();
    }

    public void EndTurn()
    {
        turnEnded = true;
        list.CrossPoint(3,1);
        if (!startTurnShowed)
        {
            startTurnWindow.Show();
            startTurnShowed = true;
        }
        TryToOpenDoor();
    }

    public void OnMove(Unit unit)
    {
        moved = true;
        list.CrossPoint(1,1);
        TryToOpenDoor();
    }

    public void OnCameraMoved()
    {
        cameraemoved = true;
        list.CrossPoint(2,1);
        TryToOpenDoor();
    }

    public void OnCameraTilted()
    {
        cameraTilted = true;
        list.CrossPoint(2, 2);
        TryToOpenDoor();
    }

    public void OnExit()
    {
        list.gameObject.SetActive(false);
    }

    bool opened = false;
    private void TryToOpenDoor()
    {
        if(turnEnded && moved && cameraemoved && cameraTilted && !opened)
        {
            opened = true;
            door.GetComponent<Animator>().Play("Lower");
            door.GetComponent<AudioSource>().Play();
        }
    }
}
