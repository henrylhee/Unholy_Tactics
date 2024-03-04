using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRoom : MonoBehaviour
{
    public GameObject door;
    public TaskList list;
    public MainUnit main;
    public AIUnit pawn;

    public Transform enemySpawnPosition;

    private void Awake()
    {
        list.gameObject.SetActive(false);
    }

    public void OnEnter()
    {
        list.gameObject.SetActive(true);

        AIParty enemy = (AIParty)Combat.GetInActiveParties()[0];
        enemy.disableLoosing = true;

        AIUnit target = Instantiate(pawn);
        //target.Setup(Combat.Instance.input);
        target.transform.position = enemySpawnPosition.position;
        enemy.SetUpUnit(target);

        FindObjectOfType<MainUnit>().abilitySelected.AddListener(OnAbilitySelected);
        target.health.DamageTaken.AddListener(OnAttack);
        target.health.Die.AddListener(OnKill);
    }

    public void OnExit()
    {
        list.gameObject.SetActive(false);
    }

    public void OnAttack(int damage)
    {
        list.CrossPoint(1,2);
    }

    public void OnAbilitySelected(Action action, Unit unit)
    {
        if(action is not Melee)
        {
            return;
        }

        list.CrossPoint(1, 1);
    }

    public void EndTurn()
    {
        //list.CrossPoint(1, 3);
    }

    bool open = false;
    public void OnKill()
    {
        if (open)
        {
            return;
        }

        open = true;
        //list.CrossPoint(2, 1);
        door.GetComponentInChildren<Animator>().Play("Lower");
        door.GetComponent<AudioSource>().Play();
        Destroy(this);
    }
}
