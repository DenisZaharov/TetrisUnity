using System.Collections.Generic;
using UnityEngine;

public class FigureCreator
{
    private FiguresModelsProvider figuresProvider = new FiguresModelsProvider();

    public FigureData GenerateFigure()
    {
        FigureData figureData;
        figureData.Color = GenerateColor();
        figureData.Points = figuresProvider.GetRandomModel();
        return figureData;
    }

    private static Color GenerateColor()
    {
        Color[] colors = { Color.blue, Color.cyan, Color.green, Color.red, Color.yellow, Color.magenta, Color.gray, Color.grey };
        int rndIdx = Random.Range(0, colors.Length);
        return colors[rndIdx];
    }
}
