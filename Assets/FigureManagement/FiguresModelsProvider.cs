using System.Collections.Generic;
using UnityEngine;

public class FiguresModelsProvider
{
    private List<Vector2[]> models = new List<Vector2[]>();
    public FiguresModelsProvider()
    {
        {
            Vector2[] points = new Vector2[8];
            points[0] = new Vector2(0, 0);
            points[1] = new Vector2(0, 1);
            points[2] = new Vector2(1, 1);
            points[3] = new Vector2(1, 2);
            points[4] = new Vector2(2, 2);
            points[5] = new Vector2(2, 1);
            points[6] = new Vector2(3, 1);
            points[7] = new Vector2(3, 0);
            models.Add(points);
        }

        {
            Vector2[] points = new Vector2[4];
            points[0] = new Vector2(0, 0);
            points[1] = new Vector2(0, 2);
            points[2] = new Vector2(2, 2);
            points[3] = new Vector2(2, 0);
            models.Add(points);
        }

        {
            Vector2[] points = new Vector2[8];
            points[0] = new Vector2(0, 0);
            points[1] = new Vector2(0, 1);
            points[2] = new Vector2(1, 1);
            points[3] = new Vector2(1, 2);
            points[4] = new Vector2(3, 2);
            points[5] = new Vector2(3, 1);
            points[6] = new Vector2(2, 1);
            points[7] = new Vector2(2, 0);
            models.Add(points);
        }

        {
            Vector2[] points = new Vector2[8];
            points[0] = new Vector2(0, 1);
            points[1] = new Vector2(0, 2);
            points[2] = new Vector2(2, 2);
            points[3] = new Vector2(2, 1);
            points[4] = new Vector2(3, 1);
            points[5] = new Vector2(3, 0);
            points[6] = new Vector2(1, 0);
            points[7] = new Vector2(1, 1);
            models.Add(points);
        }

        {
            Vector2[] points = new Vector2[4];
            points[0] = new Vector2(0, 0);
            points[1] = new Vector2(0, 1);
            points[2] = new Vector2(4, 1);
            points[3] = new Vector2(4, 0);
            models.Add(points);
        }

        {
            Vector2[] points = new Vector2[6];
            points[0] = new Vector2(0, 0);
            points[1] = new Vector2(0, 1);
            points[2] = new Vector2(2, 1);
            points[3] = new Vector2(2, 2);
            points[4] = new Vector2(3, 2);
            points[5] = new Vector2(3, 0);
            models.Add(points);
        }

        {
            Vector2[] points = new Vector2[6];
            points[0] = new Vector2(0, 0);
            points[1] = new Vector2(0, 2);
            points[2] = new Vector2(1, 2);
            points[3] = new Vector2(1, 1);
            points[4] = new Vector2(3, 1);
            points[5] = new Vector2(3, 0);
            models.Add(points);
        }
    }

    public Vector2[] GetRandomModel()
    {
        if (models.Count == 0)
            return null;

        int rnd = Random.Range(0, models.Count);
        return models[rnd];
    }
}
