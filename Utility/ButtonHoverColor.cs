using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class ButtonHoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField]
    private Color hoverColor;
    [SerializeField]
    private ActionSound hoverSound;
    [SerializeField]
    private ActionSound clickSound;

    private Color defaultColor;
    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        if(GetComponentInChildren<TextMeshProUGUI>() != null && hoverColor.a > 0)
        {
            defaultColor = GetComponentInChildren<TextMeshProUGUI>().color;
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        source.outputAudioMixerGroup = hoverSound.group;
        source.clip = hoverSound.audioClip;
        source.volume = hoverSound.volume;

        source.Play();

        if (GetComponentInChildren<TextMeshProUGUI>() != null && hoverColor.a > 0)
        {
            GetComponentInChildren<TextMeshProUGUI>().color = hoverColor;

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GetComponentInChildren<TextMeshProUGUI>() != null && hoverColor.a > 0)
        {
            GetComponentInChildren<TextMeshProUGUI>().color = defaultColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        source.outputAudioMixerGroup = clickSound.group;
        source.clip = clickSound.audioClip;
        source.volume = clickSound.volume;

        source.Play();
    }
}
