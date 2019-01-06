using UnityEngine;

public class NextFigurePusher
{
    private NextFigureArea nextFigureArea;

    public NextFigurePusher( NextFigureArea nextFigureArea_ )
    {
        nextFigureArea = nextFigureArea_;
    }

    public Transform Push(FigureData figureData)
    {
        GameObject figure = new GameObject("Next figure");
        figure.AddComponent<FigurePolygon>();
        figure.AddComponent<FigureDataComponent>();
        figure.AddComponent<MeshFilter>();
        figure.AddComponent<MeshRenderer>();
        figure.AddComponent<FigureRotatorY>();

        FigurePolygon polygon = figure.GetComponent<FigurePolygon>();
        polygon.SetPoints(figureData.Points, nextFigureArea.Left.transform.localScale.z);

        FigureDataComponent figureDataStorage = figure.GetComponent<FigureDataComponent>();
        figureDataStorage.Data = figureData;

        Mesh mesh = polygon.GetMesh();
        MeshFilter meshFilter = figure.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer renderer = figure.GetComponent<MeshRenderer>();
        renderer.material.color = figureData.Color;

        SetPosition(figure);

        FigureRotatorY rotator = figure.GetComponent<FigureRotatorY>();
        Vector2 size = polygon.CalculateSize();
        Vector3 center = new Vector3(size.x / 2, size.y / 2, 0);
        rotator.SetCenter(figure.transform.position + center);

        return figure.transform;
    }

    private void SetPosition(GameObject figure)
    {
        // Take the coordinates of the next figure's area and put the figure to the center

        Debug.Assert(figure);

        FigurePolygon polygon = figure.GetComponent<FigurePolygon>();
        Debug.Assert(polygon);
        Vector2 polygonSize = polygon.CalculateSize();

        float polygonCenterX = nextFigureArea.Top.position.x;
        float polygonCenterY = nextFigureArea.Left.position.y;

        float figureX = polygonCenterX - polygonSize.x / 2;
        float figureY = polygonCenterY - polygonSize.y / 2;

        Transform figureT = figure.GetComponent<Transform>();
        figureT.position = new Vector3(figureX, figureY, 0);
    }
}
