using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryMapScreen : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true);

        StartCoroutine(delay());
        IEnumerator delay()
        {
            yield return new WaitForSeconds(5);
            SceneLoader.Instance.LoadScene("Credits");
        }
    }
}
