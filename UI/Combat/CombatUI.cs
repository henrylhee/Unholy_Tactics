using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [SerializeField]
    private PlaceholderTutorial tutorial;
    [SerializeField]
    private GameoverScreen gameOver;
    [SerializeField]
    private VictoryScreen victoryScreen;
    [SerializeField]
    private VictoryVFXScreen victoryVFXScreen;
    [SerializeField]
    private PlayerParty playerParty;
    [SerializeField]
    private PartyUI partyUI;

    [SerializeField] private Button endTurnButton;
    private ColorBlock endTurnButtonColors;
    [SerializeField]
    private PauseScreen pauseScreen;

    [SerializeField] private Button pauseButton;
    
    
    public void Awake()
    {
        if (endTurnButton != null)
        {
            endTurnButtonColors = endTurnButton.colors;
        }
    }

    public void SetUp(Inputs input)
    {
        pauseScreen.SetUp(input);
    }

    public void EndTurn()
    {
        Combat.Instance.EndTurn();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OnPause()
    {
        pauseScreen.Show();
    }

    public void RestartTutorial()
    {
        tutorial.Begin();
    }

    public void ShowGameOverScreen()
    {
        pauseButton.enabled = false;
        gameOver.Show();
    }

    public void ShowVictoryVFXScreen()
    {
        endTurnButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);
        partyUI.gameObject.SetActive(false);
        foreach (PlayerUnit unit in playerParty.GetUnits())
        {
            unit.unitUI.gameObject.SetActive(false);
        }
        victoryVFXScreen.StartVFX(); //MainUnitLevel, CombatXPTogain
    }

    public void SetEndTurnHighlight(bool value)
    {
        if (endTurnButtonColors == null)
            return;

        endTurnButtonColors.normalColor = value ? Color.green : Color.white;
        endTurnButton.colors = endTurnButtonColors;
    }
}
