using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathCalculator))]
[RequireComponent(typeof(PathIllustrator))]
public class Pathfinder : MonoBehaviour
{
    PathIllustrator illustrator;
    PathCalculator pathcalculator;
    public LayerMask tileMask;
    public Character currentCharacter;
    Tile lastDestination;
    Path lastPath;

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

    public void ShowFrontierArea(Frontier frontier)
    {
        foreach (Tile item in frontier.tiles)
        {
            item.SetColor(HexColor.Green);
        }
    }

    public void IllustrateBreadthPath(Tile _dest)
    {
        if (_dest == lastDestination)
            return;

        lastDestination = _dest;
        illustrator.IllustratePath(GetBreadthPath(_dest));
    }

    public void Clear()
    {
        ResetPathfinder();
    }

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

        currentCharacter = character;
        openSet.Enqueue(character.characterTile);
        currentFrontier.tiles.Add(character.characterTile);
        character.characterTile.cost = 0;

        while (openSet.Count > 0)
        {
            Tile currentTile = openSet.Dequeue();

            currentTile.inFrontier = true;
            currentFrontier.tiles.Add(currentTile);

            foreach (Tile nextTile in pathcalculator.FindAdjacentTiles(currentTile))
            {

                if (!currentFrontier.tiles.Contains(nextTile))
                {

                    nextTile.parent = currentTile;
                    nextTile.cost = currentTile.cost + 1;

                    if (nextTile.cost >= currentCharacter.MovesLeft)
                        continue;

                    openSet.Enqueue(nextTile);
                    currentFrontier.tiles.Add(nextTile);
                }
            }
        }

        return currentFrontier;
    }

    public Path GetBreadthPath(Tile dest)
    {
        return pathcalculator.MakePath(dest, currentCharacter.characterTile);
    }

    private void ResetPathfinder()
    {
        illustrator.Clear();
        illustrator.ClearLastFrontier(currentFrontier.tiles);

        foreach (Tile item in openSet)
        {
            item.inFrontier = false;
        }
        foreach (Tile item in currentFrontier.tiles)
        {
            item.inFrontier = false;
        }

        openSet.Clear();
        currentFrontier.tiles.Clear();
    }

}
