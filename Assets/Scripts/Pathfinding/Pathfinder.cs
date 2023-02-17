using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathCalculator))]
[RequireComponent(typeof(PathIllustrator))]
public class Pathfinder : MonoBehaviour
{
    PathIllustrator illustrator;
    PathCalculator pathcalculator;
    [SerializeField]
    LayerMask tileMask;

    Frontier currentFrontier = new Frontier();
    Queue<Tile> openSet = new Queue<Tile>();

    private void Start()
    {
        if (pathcalculator == null)
            pathcalculator = GetComponent<PathCalculator>();
        if (illustrator == null)
            illustrator = GetComponent<PathIllustrator>();
    }

    /// <summary>
    /// Main pathfinding function, marks tiles as being in frontier, while keeping a copy of the frontier
    /// in "currentFrontier" for later clearing
    /// </summary>
    /// <param name="character"></param>
    public void FindPaths(Character character)
    {
        ResetPathfinder();

        openSet.Enqueue(character.characterTile);
        character.characterTile.cost = 0;

        while (openSet.Count > 0)
        {
            Tile currentTile = openSet.Dequeue();

            currentTile.InFrontier = true;
            currentFrontier.tiles.Add(currentTile);

            foreach (Tile adjacentTile in pathcalculator.FindAdjacentTiles(currentTile, tileMask))
            {
                if (currentFrontier.tiles.Contains(adjacentTile))
                    continue;

                adjacentTile.cost = currentTile.cost + 1;

                if (adjacentTile.cost >= character.movedata.MaxMove)
                    continue;

                adjacentTile.parent = currentTile;

                openSet.Enqueue(adjacentTile);
                currentFrontier.tiles.Add(adjacentTile);
            }
        }

        illustrator.IllustrateFrontier(currentFrontier);
    }

    public Path PathBetween(Tile dest, Tile source)
    { 
        Path path = pathcalculator.MakePath(dest, source);
        illustrator.IllustratePath(path);
        return path;
    }

    public void ResetPathfinder()
    {
        illustrator.Clear();

        foreach (Tile item in currentFrontier.tiles)
        {
            item.InFrontier = false;
            item.ClearColor();
        }

        openSet.Clear();
        currentFrontier.tiles.Clear();
    }

}
