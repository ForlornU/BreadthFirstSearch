using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathIllustrator : MonoBehaviour
{
    private const float LineHeightOffset = 0.33f;
    LineRenderer line;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    public void IllustratePath(Path path)
    {
        line.positionCount = path.tilesInPath.Length;

        for (int i = 0; i < path.tilesInPath.Length; i++)
        {
            Transform tileTransform = path.tilesInPath[i].transform;
            line.SetPosition(i, tileTransform.position.With(y: tileTransform.position.y + LineHeightOffset));
        }
    }

    public void IllustrateFrontier(Frontier frontier)
    {
        foreach (Tile item in frontier.tiles)
        {
            item.SetColor(TileColor.Green);
        }
    }

    public void Clear()
    {
        line.positionCount = 0;
    }

}
