using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCamera : MonoBehaviour
{
    public Quaternion SavedRotation;
    [SerializeField] private Transform pivotTransform;
    private Inputs inputs;

    private Quaternion baseRotation;

    private void Awake()
    {
        if (pivotTransform == null)
            pivotTransform = transform.parent.GetComponentInChildren<UnitModel>().transform;
        transform.rotation = new Quaternion(0.4226183f, 0f, 0f, 0.9063079f);
        transform.position = pivotTransform.position + new Vector3(0f, 6f, -6f);
        
        baseRotation = transform.rotation;
        SavedRotation = transform.rotation;
      
        inputs = new Inputs();
        inputs.CameraMovement.Enable();
    }

    public void Set(UnitCamera toCopy)
    {
        transform.rotation = toCopy.transform.rotation;
        transform.position = pivotTransform.position + toCopy.transform.localPosition;
        
        baseRotation = transform.rotation;
        SavedRotation = transform.rotation;
    }
    
    private void Update()
    {
        Vector2 rotateDirection = inputs.CameraMovement.Rotation.ReadValue<Vector2>().normalized;
        
        transform.RotateAround(pivotTransform.position, Vector3.up, rotateDirection.x * CombatCamera.Instance.rotateSpeed * Time.deltaTime);
        SavedRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        transform.rotation = SavedRotation;
    }
}
