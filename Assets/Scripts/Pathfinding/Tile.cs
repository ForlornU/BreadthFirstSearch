using UnityEngine;

public enum HexColor { Green, White };

public class Tile : MonoBehaviour
{
    public Tile parent;
    public Tile ladder;
    public float cost;

    [SerializeField]
    GameObject GreenChild, WhiteChild;

    public bool Occupied { get; set; } = false;
    public bool InFrontier { get; set; } = false;

    /// <summary>
    /// Returns true if tile is not occupied and within reach
    /// </summary>
    /// <returns></returns>
    public bool Reachable()
    {
        if (!Occupied && InFrontier)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Changes color of the tile by activating child-objects of different colors
    /// </summary>
    /// <param name="col"></param>
    public void SetColor(HexColor col)
    {

        ClearColor();

        switch (col)
        {
            case HexColor.Green:
                GreenChild.SetActive(true);
                break;
            case HexColor.White:
                WhiteChild.SetActive(true);
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// Deactivates all children, removing all color
    /// </summary>
    public void ClearColor()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
