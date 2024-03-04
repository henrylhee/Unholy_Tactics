using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnPreview : MonoBehaviour
{
    [SerializeField] private Color validColor;
    [SerializeField] private Color invalidColor;
    [SerializeField] private Material spawnPreviewMaterial;

    private bool isValid = false;
    private MeshRenderer meshRenderer;
    private Color defaultColor;
    
    public void Setup()
    {
        defaultColor = spawnPreviewMaterial.color;
        spawnPreviewMaterial.color = isValid ? validColor : invalidColor;
    }

    public void SetValid(bool value)
    {
        if (value == isValid)
            return;
        isValid = value;
        spawnPreviewMaterial.color = isValid ? validColor : invalidColor;
    }

    private void OnDestroy()
    {
        spawnPreviewMaterial.color = defaultColor;
    }
}
