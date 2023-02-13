using System.Collections.Generic;
using UnityEngine;

public class PathCalculator : MonoBehaviour
{

    /// <summary>
    /// Returns a list of all neighboring hexagonal tiles and ladders
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    public List<Tile> FindAdjacentTiles(Tile origin, LayerMask tileOnlyMask)
    {
        List<Tile> tiles = new List<Tile>();

        Vector3 dir = Vector3.forward;
        float rayLength = 50f;

        //Rotate a raycast in 60 degree steps and find all adjacent tiles
        for (int i = 0; i < 6; i++)
        {
            dir = Quaternion.Euler(0f, 60f, 0f) * dir;

            Vector3 aboveTilePos = (origin.transform.position + dir).With(y: origin.transform.position.y+5);

            if(Physics.Raycast(aboveTilePos, Vector3.down, out RaycastHit hit, rayLength, tileOnlyMask))
            {
                Tile hitTile;
                if (hitTile = hit.transform.GetComponent<Tile>())
                {
                    if(hitTile.Occupied == false)
                        tiles.Add(hitTile);
                }
            }
        }

        //Also return ladder tiles
        if (origin.ladder != null)
            tiles.Add(origin.ladder);

        return tiles;
    }

    /// <summary>
    /// Creates a path between two tiles
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public Path MakePath(Tile destination, Tile origin)
    {
        List<Tile> tiles = new List<Tile>();
        Tile current = destination;
        int safetytick = 100;

        while (current != origin)
        {
            safetytick--;
            if (safetytick < 1)
            {
                Debug.Log("Broke out of MakePath to avoid infinite loop");
                break;
            }

            tiles.Add(current);
            if (current.parent != null)
                current = current.parent;
            else
                break;
        }
        tiles.Add(origin);
        tiles.Reverse();

        Path path = new Path
        {
            tilesInPath = tiles.ToArray()
        };
        return path;
    }

}
