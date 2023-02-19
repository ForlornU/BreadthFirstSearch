using UnityEngine;

public enum TileColor { Green, Highlighted };

public class Tile : MonoBehaviour
{
    #region member fields
    public Tile parent;
    public Tile connectedTile;
    public Character occupyingCharacter;
    public float cost;

    [SerializeField]
    GameObject GreenChild, WhiteChild;

    [SerializeField]
    bool debug;

    public bool Occupied { get; set; } = false;
    public bool InFrontier { get; set; } = false;
    public bool CanBeReached { get { return !Occupied && InFrontier; } }
    #endregion

    /// <summary>
    /// Changes color of the tile by activating child-objects of different colors
    /// </summary>
    /// <param name="col"></param>
    public void SetColor(TileColor col)
    {

        ClearColor();

        switch (col)
        {
            case TileColor.Green:
                GreenChild.SetActive(true);
                DebugWithArrow();
                break;
            case TileColor.Highlighted:
                WhiteChild.SetActive(true);
                break;
            default:
                break;
        }
    }

    void DebugWithArrow()
    {
        Transform childArrow = GreenChild.transform.GetChild(0);

        if (!debug) 
            childArrow.gameObject.SetActive(false);

        if(childArrow != null && parent != null)
            childArrow.rotation = Quaternion.LookRotation(parent.transform.position - transform.position, Vector3.up);
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
