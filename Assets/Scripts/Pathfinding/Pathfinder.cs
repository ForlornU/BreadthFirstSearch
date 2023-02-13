using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathCalculator))]
[RequireComponent(typeof(PathIllustrator))]
public class Pathfinder : MonoBehaviour
{
    public PathIllustrator illustrator { get; private set; }
    PathCalculator pathcalculator;
    public LayerMask tileMask;

    Frontier currentFrontier = new Frontier();
    Queue<Tile> openSet = new Queue<Tile>();

    bool CanFindPath()
    {
        if (pathcalculator == null)
            pathcalculator = GetComponent<PathCalculator>();
        if (illustrator == null)
            illustrator = GetComponent<PathIllustrator>();

        if(pathcalculator == null || illustrator == null)
            return false;
        else
            return true;
    }

    //public void ShowFrontierArea(Frontier frontier)
    //{
    //    foreach (Tile item in frontier.tiles)
    //    {
    //        item.SetColor(HexColor.Green);
    //    }
    //}

    //public void IllustratePath(Tile destination, Tile source)
    //{
    //    illustrator.IllustratePath(GetPathBetween(destination, source));
    //}

    bool CanInitializeSearch()
    {
        if (!CanFindPath())
            return false;

        ResetPathfinder();
        return true;
    }

    public Frontier BreadthFirstSearch(Character character)
    {
        if (!CanInitializeSearch())
            return null;

        openSet.Enqueue(character.characterTile);
        character.characterTile.cost = 0;

        while (openSet.Count > 0)
        {
            Tile currentTile = openSet.Dequeue();

            currentTile.InFrontier = true;
            currentFrontier.tiles.Add(currentTile);

            foreach (Tile nextTile in pathcalculator.FindAdjacentTiles(currentTile, tileMask))
            {
                if (!currentFrontier.tiles.Contains(nextTile))
                {

                    nextTile.parent = currentTile;
                    nextTile.cost = currentTile.cost + 1;

                    if (nextTile.cost >= character.MovesLeft)
                        continue;

                    openSet.Enqueue(nextTile);
                    currentFrontier.tiles.Add(nextTile);
                }
            }
        }

        return currentFrontier;
    }

    public Path GetPathBetween(Tile dest, Tile source)
    {
        return pathcalculator.MakePath(dest, source);
    }

    public void ResetPathfinder()
    {
        illustrator.Clear();

        foreach (Tile item in openSet)
        {
            item.InFrontier = false;
        }
        foreach (Tile item in currentFrontier.tiles)
        {
            item.InFrontier = false;
            item.ClearColor();
        }

        openSet.Clear();
        currentFrontier.tiles.Clear();
    }

}
