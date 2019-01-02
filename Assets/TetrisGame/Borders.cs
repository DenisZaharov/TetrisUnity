using UnityEngine;

public class Borders : MonoBehaviour {

    public Transform Left;
    public Transform Right;
    public Transform Top;
    public Transform Bottom;

    public bool IsBorder( Transform border )
    {
        return border == Left || border == Right || border == Top || border == Bottom;
    }
}
