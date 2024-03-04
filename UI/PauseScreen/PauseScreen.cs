using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    [SerializeField]
    private ConcedeConfirmation concede;
    [SerializeField]
    private QuitConfirmation quit;
    [SerializeField]
    private OptionsMenu options;

    private Inputs input;
    [SerializeField]
    private Animator anim;

    public void SetUp(Inputs input)
    {
        this.input = input;

        input.General.Pause.performed += OnPausePressed;
        input.General.Pause.Enable();
    }

    public void OnConcede()
    {
        concede.Show();
    }

    public void OnRestart()
    {
        Time.timeScale = 1;
        SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().path);
    }

    public void OnMainMenu()
    {
        Time.timeScale = 1;
        SceneLoader.Instance.LoadScene("Scenes/NewUI/MainMenu");
    }

    public void OnOptions()
    {
        options.Show();
    }

    public void Quit()
    {
        quit.Show();
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (gameObject.activeInHierarchy)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Show()
    {
        input.Disable();
        input.General.Pause.Enable();

        Time.timeScale = 0;

        gameObject.SetActive(true);
        anim.Play("PauseIntroAnimation");
    }

    public void Hide()
    {
        input.Enable();
        input.General.Pause.Enable();
        Time.timeScale = 1;
        anim.Play("PauseOutroAnimation");
    }
}
