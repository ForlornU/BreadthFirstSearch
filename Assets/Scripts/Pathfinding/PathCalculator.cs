using System;
using System.Collections.Generic;
using UnityEngine;

public class PathCalculator : MonoBehaviour
{
    public LayerMask hexonlymask;

    public List<Tile> FindAdjacentTiles(Tile origin)
    {
        List<Tile> list = new List<Tile>();
        RaycastHit hit;
        Vector3 dir = Vector3.forward;

        //Rotate a raycast 60 degree steps and find all adjacent tiles
        for (int i = 0; i < 6; i++)
        {
            dir = Quaternion.Euler(0f, 60f, 0f) * dir;

            //IF this failes to find adjacent tiles, 1m distance to next is probably wrong!
            Vector3 aboveTilePos = (origin.transform.position + dir).With(y: origin.transform.position.y+5);

            //Old raycast to sides
            //if (Physics.Raycast(origin.transform.position, dir, out hit, 1f))
            if(Physics.Raycast(aboveTilePos, Vector3.down, out hit, 50f, hexonlymask))
            {
                Tile h;
                if (h = hit.transform.GetComponent<Tile>())
                {
                    if(h.occupied == false)
                    {
                        list.Add(h);
                    }
                }
            }
        }
        // Add laterally connected movement
        if (origin.ladder != null)
            list.Add(origin.ladder);

        return list;
    }

    public float CalculateTileCost(Tile start ,Tile end, Tile Tile)
    {
        float startDist = Vector3.Distance(Tile.transform.position, start.transform.position);
        float endDist = Vector3.Distance(Tile.transform.position, end.transform.position) * 2;
        float cost = startDist + endDist;
        return cost;
    }

    public Tile FindCheapestTile(List<Tile> Tiles, Tile destination, Tile start)
    {
        if (Tiles.Count == 0)
            Debug.Log("Holy shit, all tiles searched");

        Tile bestTile = destination;
        float compareCost = 100f;
        float cost;
        foreach (Tile c in Tiles)
        {
            cost = CalculateTileCost(start, destination, c);
            c.cost = cost;
            if (cost < compareCost)
            {
                bestTile = c;
                compareCost = cost;
            }
        }
        return bestTile;
    }

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

        Path path = new Path();
        path.Steps = tiles.ToArray();
        return path;
    }

}
