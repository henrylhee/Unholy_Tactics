using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatRoom : MonoBehaviour
{
    public GameObject door;
    public TaskList list;
    public MainUnit main;
    public MainUnit tutorialMain;
    public AIUnit pawn;

    public List<Transform> enemySpawnPosition;
    public Transform respawnPoint;

    public GameObject TutorialEndScreen;

    private Party alliedParty;

    private int killCounter = 0;

    [SerializeField]
    private PopWindow manaWindow;
    [SerializeField]
    private PopWindow summonWindow;

    private bool showedMana = false;
    private bool showedSummon = false;

    private void Awake()
    {
        list.gameObject.SetActive(false);
        main.health.Die.AddListener(OnMainDeath);
    }

    public void OnEnter()
    {
        killCounter = 0;
        list.gameObject.SetActive(true);

        alliedParty = Combat.GetActiveParty();
        alliedParty.disableLoosing = true;
        alliedParty.ManaUse.AddListener(OnManaUse);
        alliedParty.SoulUse.AddListener(OnSoulUse);

        FindObjectOfType<MainUnit>().stats.autoManaGeneration = 1;
        FindObjectOfType<PlayerParty>().GainMana(10);

        foreach (Transform spawnpoint in enemySpawnPosition)
        {
            AIParty enemy = (AIParty)Combat.GetInActiveParties()[0];
            enemy.disableLoosing = true;

            AIUnit target = Instantiate(pawn);
            //target.Setup(Combat.Instance.input);
            target.transform.position = spawnpoint.position;
            enemy.SetUpUnit(target);

            FindObjectOfType<PlayerParty>().OnUnitRegistered.AddListener(OnUnitRegistered);
            FindObjectOfType<MainUnit>().abilitySelected.AddListener(OnAbilitySelected);

            target.health.Die.AddListener(OnKill);
        }

        if (!showedMana)
        {
            showedMana = true;
            manaWindow.Show();
        }
    }

    public void OnExit()
    {
        list.gameObject.SetActive(false);
    }

    public void OnMainDeath()
    {
        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(0.1f);
            Destroy(main.gameObject);

            main = Instantiate(tutorialMain);
            main.transform.position = respawnPoint.position;

           // main.Setup(Combat.Instance.input);
            main.health.Die.AddListener(OnMainDeath);
            alliedParty.SetUpUnit(main);
        }
    }

    public void OnAbilitySelected(Action action, Unit unit)
    {
        if(unit is MainUnit && action is Summon)
        {
            list.CrossPoint(1, 1);
        }
        
    }

    public void OnSummonAttack(Action action)
    {
        if(action is Range)
        {
            list.CrossPoint(2, 1);
        }
    }

    public void OnUnitRegistered(Unit unit)
    {
        list.CrossPoint(1, 2);
        ((PlayerUnit)unit).performed.AddListener(OnSummonAttack);
        if (!showedSummon)
        {
            showedMana = true;
            summonWindow.Show();
        }
    }

    public void OnKill()
    {
        killCounter++;
        if(killCounter >= enemySpawnPosition.Count)
        {
            list.CrossPoint(3,1);
            door.transform.position += new Vector3(0, 1000);
            TutorialEndScreen.SetActive(true);
            SaveState.Instance.data.tutorialFinished = true;
            SaveState.Instance.Save();
            Time.timeScale = 0;
        }
    }

    public void OnManaUse()
    {
        //list.CrossPoint(0);
    }

    public void OnSoulUse()
    {
       // list.CrossPoint(1);
    }

    public void LoadLevelSelect()
    {
        Time.timeScale = 1;
        SceneLoader.Instance.LoadScene("LevelSelection");
    }
}
