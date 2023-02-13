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

    public void IllustratePath(Path _p)
    {
        line.positionCount = _p.tilesInPath.Length;

        for (int i = 0; i < _p.tilesInPath.Length; i++)
        {
            line.SetPosition(i, _p.tilesInPath[i].transform.position.With(y: LineHeightOffset));
        }
    }

    public void IllustrateFrontier(Frontier frontier)
    {
        foreach (Tile item in frontier.tiles)
        {
            item.SetColor(HexColor.Green);
        }
    }

    public void Clear()
    {
        line.positionCount = 0;
    }

}
