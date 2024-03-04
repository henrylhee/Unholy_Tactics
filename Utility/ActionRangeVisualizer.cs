using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActionRangeVisualizer : MonoBehaviour
{
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material moveMaterial;
    [SerializeField] private MeshFilter maskMesh;
    [SerializeField] private Vector3 rangeOffset = new Vector3(0, 0.1f, 0);

    private MeshCollider rangeCollider;
    private Mesh defaultMesh;
    private MeshRenderer meshRender;
    private MeshFilter filter;
    private NavMeshTriangulation triangulation;
    public NavMeshTriangulation Triangulation => triangulation;
    
    private static ActionRangeVisualizer instance;
    public static ActionRangeVisualizer Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            instance.Setup();
            Hide();
        }
        else
        {
            Debug.LogWarning("There can only be one ActionRangeVisualizer! Duplicate was deleted.");
            Destroy(gameObject);
        }
    }

    private void Setup()
    {
        triangulation = NavMesh.CalculateTriangulation();
        
        meshRender = GetComponent<MeshRenderer>();
        filter = GetComponent<MeshFilter>();
        defaultMesh = new Mesh();
        rangeCollider = GetComponent<MeshCollider>();

        defaultMesh.SetVertices(triangulation.vertices);
        defaultMesh.SetIndices(triangulation.indices, MeshTopology.Triangles, 0);
        
        if (defaultMaterial != null)
            meshRender.sharedMaterial = defaultMaterial;
        filter.mesh = defaultMesh;
        rangeCollider.sharedMesh = defaultMesh;
    }
    
    public void RenderDefaultRange(Vector3 position,  float radius)
    {
        if (defaultMaterial != null)
            meshRender.sharedMaterial = defaultMaterial;
        filter.mesh = defaultMesh;
        rangeCollider.sharedMesh = defaultMesh;
        transform.position = position;
        
        UpdateMask(position, radius);
        gameObject.SetActive(true);
    }

    public void RenderMoveRange(Mesh moveMesh, Vector3 position,  float radius)
    {
        if (moveMaterial != null)
            meshRender.sharedMaterial = moveMaterial;
        filter.mesh = moveMesh;
        rangeCollider.sharedMesh = moveMesh;
        transform.position = position;
        UpdateMask(position, radius);
        gameObject.SetActive(true);
    }

    private void UpdateMask(Vector3 position, float radius)
    {
        maskMesh.mesh = SurfaceHelper.Get2DCircle(radius, position + rangeOffset * 2, 60);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
