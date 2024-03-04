using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScrolling : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 5f;
    [SerializeField] private float sceneTransitionDelay = 1f;
    [SerializeField] private float maxScrollDistance;
    [SerializeField] private RectTransform textRect;

    private float scrollDistance;
    private bool isScrollFinished = false;

    
    // Update is called once per frame
    void Update()
    {
        if (isScrollFinished)
            return;
        
        var moveDistance = scrollSpeed * Time.deltaTime;
        scrollDistance += moveDistance;
        transform.position = transform.position += new Vector3(0f, moveDistance, 0f);

        if (!(scrollDistance >= maxScrollDistance)) return;
        isScrollFinished = true;
        Invoke(nameof(LoadMainMenu), sceneTransitionDelay);
    }

    void LoadMainMenu()
    {
        SceneLoader.Instance.LoadScene("Scenes/NewUI/MainMenu");
    }
}
