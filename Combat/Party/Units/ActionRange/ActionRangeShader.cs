using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRangeShader : MonoBehaviour
{
    private Mesh mesh;
    private float range;
    private Material material;
    public void SetShader(Vector3 casterPosition)
    {
        mesh = GetComponent<Mesh>();
        material = GetComponent<Material>();

        List<Vector3> vertices = new List<Vector3>();
        mesh.GetVertices(vertices);

        List<Vector2> uvs = new List<Vector2>();

        float maxDistance = 0;
        float distance;
        foreach(Vector3 vertex in vertices)
        {
            distance = Vector3.Distance(vertex, casterPosition);
            if (distance > maxDistance)
            {
                maxDistance = distance;
            }
        }

        for(int i = 0; i < vertices.Count; i++)
        {
            uvs.Add(new Vector2(0.5f, Vector3.Distance(vertices[i], casterPosition)/maxDistance));
        }

        material.SetFloat("_Range", range/maxDistance);
    }

    public void GetRange(float range)
    {
        this.range = range;
    }
}
