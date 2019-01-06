using UnityEngine;

public class FigurePusher
{
    const float kGravityScale = 0.1f;

    private Borders borders;
    private Transform figuresParent;

    public FigurePusher(Borders borders_, Transform figuresParent_)
    {
        borders = borders_;
        figuresParent = figuresParent_;
    }
    public Transform Push(FigureData figureData)
    {
        GameObject figure = new GameObject("Figure");
        figure.transform.SetParent(figuresParent);
        figure.AddComponent<FigurePolygon>();
        figure.AddComponent<FigureDataComponent>();
        figure.AddComponent<Rigidbody2D>();
        figure.AddComponent<PolygonCollider2D>();     
        figure.AddComponent<MeshFilter>();
        figure.AddComponent<MeshRenderer>();

        FigurePolygon polygon = figure.GetComponent<FigurePolygon>();
        polygon.SetPoints(figureData.Points, borders.Left.transform.localScale.z);

        FigureDataComponent figureDataStorage = figure.GetComponent<FigureDataComponent>();
        figureDataStorage.Data = figureData;

        Rigidbody2D rigidbody2D = figure.GetComponent<Rigidbody2D>();
        rigidbody2D.gravityScale = kGravityScale;

        PolygonCollider2D collider = figure.GetComponent<PolygonCollider2D>();
        collider.points = polygon.GetPoints();

        Mesh mesh = polygon.GetMesh();
        MeshFilter meshFilter = figure.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer renderer = figure.GetComponent<MeshRenderer>();
        renderer.material.color = figureData.Color;

        SetPosition(figure.transform);

        // Check if the figure will collide with other figures at the start position. If so, we can't place it
        Collider2D[] results = new Collider2D[100];
        int numColliders = rigidbody2D.OverlapCollider(new ContactFilter2D(), results);
        if (numColliders > 0)
        {
            for (int i = 0; i < numColliders; i++)
            {
                if (results[i].transform != borders.Top)
                {
                    Object.Destroy(figure.gameObject);
                    return null;
                }
            }
        }

        return figure.transform;
    }
    private void SetPosition(Transform figure)
    {
        // Take the coordinates of the left, right and top walls and put the figure regarding them

        Debug.Assert(figure);

        FigurePolygon polygon = figure.GetComponent<FigurePolygon>();
        Debug.Assert(polygon);

        float leftWallCenterX = borders.Left.position.x;
        float rightWallCenterX = borders.Right.position.x;
        float leftWallX = leftWallCenterX - borders.Left.localScale.x / 2;
        float leftWallWidth = borders.Left.localScale.x;
        float rightWallX = rightWallCenterX - borders.Right.localScale.x / 2;

        float areaX = leftWallX + leftWallWidth;
        float areaWidth = rightWallX - areaX;
        Debug.Assert(areaWidth > 0);
        Vector2 polygonSize = polygon.CalculateSize();

        float figureX = areaX + (areaWidth - polygonSize.x) / 2;

        float topWallCenterY = borders.Top.position.y;
        float topWallY = topWallCenterY - borders.Top.localScale.y / 2;

        float figureY = topWallY - polygonSize.y;
        
        figure.position = new Vector3(figureX, figureY, 0);
    }
}
