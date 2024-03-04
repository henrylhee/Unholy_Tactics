using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CombatCamera : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent moved;
    [HideInInspector]
    public UnityEvent tilted;

    [SerializeField]
    private int speed;
    public float rotateSpeed;
    [SerializeField]
    private float zoomSpeed;
    [SerializeField]
    private CinemachineVirtualCamera cam;

    [SerializeField]
    private Inputs inputs;
    private CinemachineBrain mainBrain;
    private CinemachineConfiner cinemachineConfiner;
    
    private float defaultBlendTime = 0;

    private float zoomDistance;

    public static CombatCamera Instance;
    
    private Transform parentTransform;

    private bool isActive;
    
    public void Awake()
    {
        parentTransform = transform.parent.transform;
        if (Instance == null)
            Instance = this;
    }

    public void SetUp(Inputs inputs)
    {
        mainBrain = Camera.main.GetComponent<CinemachineBrain>();
        cinemachineConfiner = GetComponent<CinemachineConfiner>();
        
        this.inputs = inputs;
        defaultBlendTime = mainBrain.m_DefaultBlend.m_Time;

        inputs.Party.Select_Next.performed += DeActivate;
        inputs.Party.Select_Previous.performed += DeActivate;
        inputs.CameraMovement.activation.performed += Activate;
    }

    private void Activate(InputAction.CallbackContext context)
    {
        if (isActive || parentTransform == null)
            return;

        isActive = true;
        mainBrain.m_DefaultBlend.m_Time = 0;

        parentTransform.position = Combat.GetActiveParty() is PlayerParty playerParty
            ? playerParty.GetSelectedUnit().model.transform.position
            : cam.transform.position;
        parentTransform.rotation = Quaternion.identity;
        transform.position = Camera.main.transform.position;
        transform.rotation = Camera.main.transform.rotation;
        cam.Priority = int.MaxValue;
    }

    public void DeActivate(InputAction.CallbackContext context)
    {
        isActive = false;
        
        cam.Priority = int.MinValue;
        mainBrain.m_DefaultBlend.m_Time = defaultBlendTime;
    }

    private void Update()
    {
        if (!isActive || cam.Priority != int.MaxValue)
        {
            return;
        }

        Vector2 inputDirection = inputs.CameraMovement.movements.ReadValue<Vector2>();
        Vector3 sideDirection = new Vector3(inputDirection.x * transform.right.x,0, inputDirection.x * transform.right.z).normalized;
        Vector3 forwardDirection = new Vector3(inputDirection.y * transform.forward.x,0, inputDirection.y * transform.forward.z).normalized;
        Vector2 rotateDirection = inputs.CameraMovement.Rotation.ReadValue<Vector2>().normalized;

        
        if(inputDirection != new Vector2())
        {
            moved?.Invoke();
        }
        if(rotateDirection != new Vector2())
        {
            tilted?.Invoke();
        }

        Transform testTransform;
        
        
        parentTransform.position += sideDirection * speed * Time.deltaTime;

        if (!cinemachineConfiner.m_BoundingVolume.bounds.Contains(new Vector3(transform.position.x, transform.position.y, transform.position.z)))
            parentTransform.position -= sideDirection * speed * Time.deltaTime;

        parentTransform.position += forwardDirection * speed * Time.deltaTime;
        
        if (!cinemachineConfiner.m_BoundingVolume.bounds.Contains(new Vector3(transform.position.x, transform.position.y, transform.position.z)))
            parentTransform.position -= forwardDirection * speed * Time.deltaTime;
        
        parentTransform.Rotate(Vector3.up, rotateDirection.x * rotateSpeed * Time.deltaTime);
        if (!cinemachineConfiner.m_BoundingVolume.bounds.Contains(new Vector3(transform.position.x, transform.position.y, transform.position.z)))
            parentTransform.Rotate(Vector3.up, -rotateDirection.x * rotateSpeed * Time.deltaTime);
        
        HandleZoom();
    }

    private void HandleZoom()
    {
        Vector3 zoomDirection =  transform.forward.normalized;
        
        var testPosition = transform.position + zoomDirection * inputs.CameraMovement.Zoom.ReadValue<Vector2>().y * zoomSpeed * Time.deltaTime;
        if (cinemachineConfiner.m_BoundingVolume.bounds.Contains(testPosition))
        {
            transform.position = testPosition;
        }
    }
}
