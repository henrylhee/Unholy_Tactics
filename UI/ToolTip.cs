using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private bool followMouse = true;
    [SerializeField] private bool fixPosition = true;
    private bool isActive = false;


    public bool IsActive
    {
        set
        {
            gameObject.SetActive(value);
            isActive = value;
        }
        get => isActive;
    }

    public string Description
    {
        set
        {
            if (baseDescription == "")
                baseDescription = value;
            text.text = value;
        }
        get => text.text;
    }

    private string baseDescription = "";
    public string BaseDescription => baseDescription;

    private Camera cam;
    private Vector3 min, max;
    private RectTransform rect;
    [SerializeField] private Vector2 offset = Vector2.zero;

    [HideInInspector] public TextMeshProUGUI text = null;

    // Start is called before the first frame update
    void Start()
    {
        if (text == null)
            Setup();
    }

    public void Setup()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        cam = Camera.main;
        rect = GetComponent<RectTransform>();
        min = new Vector3(0, 0, 0);
        max = new Vector3(cam.pixelWidth, cam.pixelHeight, 0);
        IsActive = isActive;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive && fixPosition)
        {
            //get the tooltip position with offset
            Vector3 position = followMouse ?
                new Vector3(Input.mousePosition.x + rect.rect.width + offset.x, Input.mousePosition.y - (rect.rect.height / 2) + offset.y, 0f)
                : transform.position;
            //clamp it to the screen size so it doesn't go outside
            transform.position = new Vector3(Mathf.Clamp(position.x, min.x + rect.rect.width/2, max.x - rect.rect.width/2), Mathf.Clamp(position.y, min.y + rect.rect.height / 2, max.y - rect.rect.height / 2), transform.position.z);
        }
    }
}