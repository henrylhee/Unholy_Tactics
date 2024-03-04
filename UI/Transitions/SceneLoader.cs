using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private PlayableDirector introDirector;
    [SerializeField] private PlayableDirector outroDirector;
    
    private string  sceneToLoad = "";
    private static bool isTransitionQueued = false;
    private bool isCanvasAvailable = true;

    public static SceneLoader Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (GetComponentInParent<Canvas>() == null)
        {
            var canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                transform.SetParent(canvas.transform);
            }
            else
            {
                isCanvasAvailable = false;
            }
        }
        
        if (isCanvasAvailable && isTransitionQueued)
        {
            introDirector.Play();
        }
        
        isTransitionQueued = false;
    }
    
    public void LoadScene(string path)
    {
        transform.SetAsLastSibling();
        sceneToLoad = path;
        
        isTransitionQueued = true;
        
        if (isCanvasAvailable)
        {
            outroDirector.Play();
        }
        else // without a Canvas the Blend Animation can't be displayed so the scene is instantly loaded instead
        {
            OnSceneTransitionFinished();
        }
    }


    public static void QueueTransition()
    {
        isTransitionQueued = true;
    }
    
    public void OnSceneTransitionFinished()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    
    public static void QuitGame()
    {
      #if UNITY_EDITOR
          UnityEditor.EditorApplication.isPlaying = false;
      #else
          Application.Quit();
      #endif
    }
}
