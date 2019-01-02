using System.Collections.Generic;
using UnityEngine;
using System;

public interface FigurePolygonListener
{
    void OnCollision(Transform object1, Transform object2);
}

public class FigurePolygon : MonoBehaviour {

    private Vector2[] points;
    private Mesh mesh;
    private HashSet<FigurePolygonListener> listeners = new HashSet<FigurePolygonListener>();

    public void AddListener(FigurePolygonListener listener)
    {
        if (listeners.Contains(listener))
        {
            Debug.Assert(listeners.Contains(listener));
            return;
        }
        listeners.Add(listener);
    }

    public void RemoveListener(FigurePolygonListener listener)
    {
        listeners.Remove(listener);
    }

    public void SetPoints(Vector2[] points_ )
    {
        // Note: See method TestCreateCube in commentaries at the end of this file for simple example of setting vertices and triangles to mesh

        points = points_;

        int vecticesCount = points.Length; // First take number of points for the front face of the figure
        vecticesCount += points.Length; // add the same number of points for the back face
        int numOfSideFaces = points.Length; // points.Length is just the number of side faces
        vecticesCount += numOfSideFaces * 4; // add number of points for the side faces (each face is rectangle with 4 points)

        Vector3[] vertices = new Vector3[vecticesCount]; // verticles for assign to Mesh.vertices

        int filledVerts = 0; // how many points already put to the vertices array

        // points for the front face
        for (int iVert = 0; iVert < points.Length; iVert++)
        {
            vertices[iVert].x = points[iVert].x;
            vertices[iVert].y = points[iVert].y;
            vertices[iVert].z = -0.5f;
        }

        filledVerts += points.Length;

        // points for the back face
        for (int iVert = 0; iVert < points.Length; iVert++)
        {
            vertices[filledVerts + iVert].x = points[iVert].x;
            vertices[filledVerts + iVert].y = points[iVert].y;
            vertices[filledVerts + iVert].z = 0.5f;
        }

        filledVerts += points.Length;

        // points for the side faces
        for (int iPoint = 0; iPoint < numOfSideFaces; iPoint++)
        {
            vertices[filledVerts].x = points[iPoint].x;
            vertices[filledVerts].y = points[iPoint].y;
            vertices[filledVerts].z = -0.5f;

            filledVerts++;

            vertices[filledVerts].x = points[iPoint].x;
            vertices[filledVerts].y = points[iPoint].y;
            vertices[filledVerts].z = 0.5f;

            filledVerts++;

            if (iPoint < points.Length - 1)
            {
                vertices[filledVerts].x = points[iPoint + 1].x;
                vertices[filledVerts].y = points[iPoint + 1].y;
                vertices[filledVerts].z = 0.5f;

                filledVerts++;

                vertices[filledVerts].x = points[iPoint + 1].x;
                vertices[filledVerts].y = points[iPoint + 1].y;
                vertices[filledVerts].z = -0.5f;

                filledVerts++;
            }
            else
            {
                vertices[filledVerts].x = points[0].x;
                vertices[filledVerts].y = points[0].y;
                vertices[filledVerts].z = 0.5f;

                filledVerts++;

                vertices[filledVerts].x = points[0].x;
                vertices[filledVerts].y = points[0].y;
                vertices[filledVerts].z = -0.5f;

                filledVerts++;
            }
        }

        Triangulator triangulator = new Triangulator(points);
        int[] triangulated = triangulator.Triangulate(); // array of points indices for triangles
        Mesh m = new Mesh();

        int frontTrianglesSize = triangulated.Length;
        int backTrianglesSize = triangulated.Length;
        int sideTrianglesSize = numOfSideFaces * 6; // we will generate two triangles for each side face, so there is six points needed (3 for each triangle)
        int trianglesSize = frontTrianglesSize + backTrianglesSize + sideTrianglesSize;
        int[] triangles = new int[trianglesSize]; // triangles for assign to Mesh.triangles

        int filledTris = 0; // how many indices already put to the triangles array

        // triangles for the front face
        for (int i = 0; i < triangulated.Length; i += 3)
        {
            triangles[i] = triangulated[i];
            triangles[i + 1] = triangulated[i + 1];
            triangles[i + 2] = triangulated[i + 2];
        }

        filledTris += triangulated.Length;
       
        int backPointsOffset = points.Length;

        // triangles for the back face
        for (int i = 0; i < triangulated.Length; i += 3)
        {
            triangles[filledTris + i] = triangulated[i + 2] + backPointsOffset;
            triangles[filledTris + i + 1] = triangulated[i + 1] + backPointsOffset;
            triangles[filledTris + i + 2] = triangulated[i] + backPointsOffset;
        }

        filledTris += triangulated.Length;
        int sidesPointsOffset = backPointsOffset + points.Length;

        // triangles for the side faces
        for (int i = 0; i < points.Length; i++)
        {
            // first triangle
            triangles[filledTris] = sidesPointsOffset;
            triangles[filledTris + 1] = sidesPointsOffset + 1;
            triangles[filledTris + 2] = sidesPointsOffset + 2;

            // second triangle
            triangles[filledTris + 3] = sidesPointsOffset;
            triangles[filledTris + 4] = sidesPointsOffset + 2;
            triangles[filledTris + 5] = sidesPointsOffset + 3;

            filledTris += 6;
            sidesPointsOffset += 4;
        }

        m.vertices = vertices;
        m.triangles = triangles;
        m.RecalculateNormals();
        m.RecalculateBounds();

        mesh = m;
    }

    public Mesh GetMesh()
    {
        return mesh;
    }

    public Vector2[] GetPoints()
    {
        return points;
    }

    public Vector2 CalculateSize()
    {
        if (points.Length == 0)
            return new Vector2(0, 0);

        float minX = Single.NaN,
            maxX = Single.NaN,
            minY = Single.NaN,
            maxY = Single.NaN;

        foreach (Vector2 point in points)
        {
            if (Single.IsNaN(minX) || point.x < minX)
                minX = point.x;
            else if (Single.IsNaN(maxX) || point.x > maxX)
                maxX = point.x;

            if (Single.IsNaN(minY) || point.y < minY)
                minY = point.y;
            else if (Single.IsNaN(maxY) || point.y > maxY)
                maxY = point.y;
        }

        return new Vector2(maxX - minX, maxY - minY);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (FigurePolygonListener listener in listeners)
            listener.OnCollision(transform, collision.transform);
    }

    /*private Mesh TestCreateCube()
    {
        Vector3[] vertices = {
            //front
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 0, 0),

            //back
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 0, 1),

            //left
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),
            new Vector3(0, 0, 1),

            //right
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 1, 1),
            new Vector3(1, 0, 1),

            //bottom
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(1, 0, 0),

            //top
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 1, 0)
        };

        int[] triangles = {
            //front
            0,1,2,
            0,2,3,
            //back
            4,6,5,
            4,7,6,
            //left
            11,10,9,
            11,9,8,
            //right
            12,13,14,
            12,14,15,
            //bottom
            19,18,17,
            19,17,16,
            //top
            20,21,22,
            20,22,23
        };

        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }*/
}
