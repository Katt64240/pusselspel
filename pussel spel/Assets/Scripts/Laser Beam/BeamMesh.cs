﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BeamMesh
{
    private static readonly int lod = 8; 
    private static readonly float beamRadious = 0.1f;

    
    public static Mesh GenerateMesh(BeamSegment[] segments, Transform origo)
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();


        for(int i = 0; i < segments.Length; i++)
        {

            Vector3[] cylindePoints = GenerateCylinderPoints(segments[i], origo);

            int l = vertices.Count;

            for (int p = 0; p < lod; p++)
            {

                triangles.Add(l + p);
                triangles.Add(l + p + lod);
                triangles.Add(l + (p+1)%lod + lod);

                triangles.Add(l + p);
                triangles.Add(l + (p + 1) % lod + lod);
                triangles.Add(l + (p + 1) % lod);
                // add endpoints
            }

            vertices.AddRange(cylindePoints);
            
        }


        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        return mesh;
    }

    static Vector3[] GenerateCylinderPoints(BeamSegment segment, Transform origo)
    {
        Vector3[] cylindePoints = new Vector3[lod * 2];

        Vector3 p1 = segment.p1;
        Vector3 p2 = segment.p2;
        Vector3 n1 = segment.n1;
        Vector3 n2 = segment.n2;
        Vector3 dir = (p2 - p1).normalized;


        float angleSegment = (2 * Mathf.PI) / lod;

        Vector3 perpendicularAxis = FindPerpendicularVector(n1, n2);

        Vector3 flatDir1 = Vector3.Cross(perpendicularAxis, n1).normalized;
        Vector3 flatDir2 = Vector3.Cross(perpendicularAxis, n2).normalized;

        float scaleFactor1 = -beamRadious / Vector3.Dot(n1, dir);
        float scaleFactor2 = -beamRadious / Vector3.Dot(n2, dir);


        for (int p = 0; p < lod; p++)//*(0.5f/ Mathf.Cos(angle1))
        {
            //create elongated rings
            cylindePoints[p] = flatDir1 * Mathf.Cos(angleSegment * p) * scaleFactor1
                + perpendicularAxis * Mathf.Sin(angleSegment * p) * beamRadious;

            cylindePoints[p + lod] = flatDir2 * Mathf.Cos(angleSegment * p) * scaleFactor2
                + perpendicularAxis * Mathf.Sin(angleSegment * p) * beamRadious;

            //place points
            cylindePoints[p] += p1;
            cylindePoints[p + lod] += p2;

            //transform points to local space
            cylindePoints[p] = origo.InverseTransformPoint(cylindePoints[p]);
            cylindePoints[p + lod] = origo.InverseTransformPoint(cylindePoints[p + lod]);
        }

        return cylindePoints;
    }

    static Vector3 FindPerpendicularVector(Vector3 v1, Vector3 v2)
    {
        if (Vector3.Cross(v1, v2) != Vector3.zero)
        {
            return Vector3.Cross(v1, v2).normalized;
        }
        else
        {
            if (Vector3.Cross(v1, Vector3.up) != Vector3.zero)
            {
                return Vector3.Cross(v1, Vector3.up).normalized;
            }
            else
            {
                return Vector3.Cross(v1, Vector3.forward).normalized;
            }
        }
    }

}
