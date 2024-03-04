using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public class ActionRangeNew : MonoBehaviour
{
    [SerializeField]
    private GameObject rangeTemplate;
    [SerializeField]
    private Material rangeMateral;
    [SerializeField]
    private Vector3 rangeOffset = new Vector3(0, 0.05f, 0);

    private GameObject activeTemplate;
    private MeshCollider rangeCollider;
    //private GameObject range;
    
    [SerializeField] private float distanceThreshold = 0.1f;
    [SerializeField] private int iterations = 2;
    [SerializeField] private int debugStartIndex = -1;

    private float range;
    
    public GameObject Generate(float radius, Vector3 position)
    {
        if(activeTemplate != null)
        {
            DestroyImmediate(activeTemplate);
        }
        
        activeTemplate = GenerateRangeTemplate(radius, position);
        StartCoroutine(delay());

        range = radius;

        return activeTemplate;
        
        IEnumerator delay()
        {
            yield return new WaitForEndOfFrame();
            GenerateNavMesh();
        }
    }

    public void Render()
    {
        if(activeTemplate != null)
        {
            activeTemplate.transform.position = rangeOffset;
            activeTemplate.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Error: trying to Generate a range that wasn't calculated");
        }
    }

    public void Clear()
    {
        if (activeTemplate != null)
        {
            Destroy(activeTemplate.gameObject);
            activeTemplate = null;
        }
    }

    private GameObject GenerateRangeTemplate(float radius, Vector3 position)
    {
        GameObject result = Instantiate(rangeTemplate);

        result.transform.position = position;
        //result.transform.localScale = new Vector3(radius * 2, 0.01f, radius * 2);
        

        return result;
    }

    /*
    private void GenerateRange(GameObject ObjectToSpawnOn)
    {
        range = new GameObject("Navmesh Visualization");
        range.AddComponent<MeshRenderer>();
        range.AddComponent<MeshFilter>();
        rangeCollider = range.AddComponent<MeshCollider>();
        range.layer = LayerMask.NameToLayer("AbilityArea");
        
    }*/
    
    float sign (Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    bool IsPointInTriangle (Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        float d1, d2, d3;
        bool has_neg, has_pos;

        d1 = sign(pt, v1, v2);
        d2 = sign(pt, v2, v3);
        d3 = sign(pt, v3, v1);

        has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(has_neg && has_pos);
    }

    private void GenerateNavMesh()
    {
        MeshRenderer renderer = activeTemplate.GetComponent<MeshRenderer>();
        MeshFilter filter = activeTemplate.GetComponent<MeshFilter>();

        Mesh navmesh = new Mesh();
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
        rangeCollider = activeTemplate.GetComponent<MeshCollider>();
        
        
        SurfaceHelper surfaceHelper = new SurfaceHelper();
        surfaceHelper.Setup(triangulation, transform.parent.position, range);
        surfaceHelper.ProcessSurfaces();

        List<Vector3> reachableVertices = surfaceHelper.validVertices;
        List<int> reachableIndices = surfaceHelper.connectedIndices;


        List<Vector2> uvs = new List<Vector2>();
        Vector3 casterPosition = transform.parent.position;
        float minx = reachableVertices[0].x;
        float maxx = reachableVertices[0].x;
        float miny = reachableVertices[0].z;
        float maxy = reachableVertices[0].z;


        for (int i = 1; i < reachableVertices.Count; i++)
        {
            if (reachableVertices[i].x < minx)
            {
                minx = reachableVertices[i].x;
            }
            else if (reachableVertices[i].x > maxx)
            {
                maxx = reachableVertices[i].x;
            }
            if (reachableVertices[i].z < miny)
            {
                miny = reachableVertices[i].z;
            }
            else if (reachableVertices[i].z > maxy)
            {
                maxy = reachableVertices[i].z;
            }
        }
        float lengthx = maxx - minx;
        float lengthy = maxy - miny;

        for (int i = 0; i < reachableVertices.Count; i++)
        {

            //Vector3 v = reachableVertices[i] - casterPosition;
            float Vertexlengthx = reachableVertices[i].x - minx;
            float Vertexlengthy = reachableVertices[i].z - miny;

            uvs.Add(new Vector2(Vertexlengthx / lengthx, Vertexlengthy / lengthy));
        }

        rangeMateral.SetFloat("_RangeX", Mathf.Clamp(range / lengthx, 0, 1));
        rangeMateral.SetFloat("_RangeY", Mathf.Clamp(range / lengthy, 0, 1));
        rangeMateral.SetFloat("_PosX", (casterPosition.x - minx) / lengthx);
        rangeMateral.SetFloat("_PosY", (casterPosition.z - miny) / lengthy);

        Debug.Log(minx);
        Debug.Log(maxx);
        Debug.Log(miny);
        Debug.Log(maxy);
        Debug.Log(new Vector2((casterPosition.x - minx), (casterPosition.z - miny)));

        Debug.Log("amount of vertices in actionrange: " + reachableVertices.Count);

        navmesh.SetVertices(reachableVertices);
        navmesh.SetIndices(reachableIndices, MeshTopology.Triangles, 0);
        navmesh.SetUVs(0, uvs);
        navmesh.RecalculateUVDistributionMetrics();
        navmesh.RecalculateTangents();
        navmesh.RecalculateBounds();
        navmesh.RecalculateNormals();

        renderer.sharedMaterial = rangeMateral;
        filter.mesh = navmesh;
        rangeCollider.sharedMesh = navmesh;
    }
}
