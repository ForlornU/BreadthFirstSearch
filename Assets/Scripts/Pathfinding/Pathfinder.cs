using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathIllustrator))]
public class Pathfinder : MonoBehaviour
{
    #region member fields
    PathIllustrator illustrator;
    [SerializeField]
    LayerMask tileMask;

    Frontier currentFrontier = new Frontier();
    #endregion

    private void Start()
    {
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

        Queue<Tile> openSet = new Queue<Tile>();
        openSet.Enqueue(character.characterTile);
        character.characterTile.cost = 0;
    
        while (openSet.Count > 0)
        {
            Tile currentTile = openSet.Dequeue();

            foreach (Tile adjacentTile in FindAdjacentTiles(currentTile))
            {
                if (openSet.Contains(adjacentTile))
                    continue;

                adjacentTile.cost = currentTile.cost + 1;

                if (!IsValidTile(adjacentTile, character.movedata.MaxMove))
                    continue;

                adjacentTile.parent = currentTile;

                openSet.Enqueue(adjacentTile);
                AddTileToFrontier(adjacentTile);
            }
        }

        illustrator.IllustrateFrontier(currentFrontier);
    }

    bool IsValidTile(Tile tile, int maxcost)
    {
        bool valid = false;

        if (!currentFrontier.tiles.Contains(tile) && tile.cost <= maxcost)
            valid = true;

        return valid;
    }

    void AddTileToFrontier(Tile tile)
    {
        tile.InFrontier = true;
        currentFrontier.tiles.Add(tile);
    }

    /// <summary>
    /// Returns a list of all neighboring hexagonal tiles and ladders
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    private List<Tile> FindAdjacentTiles(Tile origin)
    {
        List<Tile> tiles = new List<Tile>();
        Vector3 direction = Vector3.forward;
        float rayLength = 50f;
        float rayHeightOffset = 1f;

        //Rotate a raycast in 60 degree steps and find all adjacent tiles
        for (int i = 0; i < 6; i++)
        {
            direction = Quaternion.Euler(0f, 60f, 0f) * direction;

            Vector3 aboveTilePos = (origin.transform.position + direction).With(y: origin.transform.position.y + rayHeightOffset);

            if (Physics.Raycast(aboveTilePos, Vector3.down, out RaycastHit hit, rayLength, tileMask))
            {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                if (hitTile.Occupied == false)
                    tiles.Add(hitTile);
            }
        }

        if (origin.connectedTile != null)
            tiles.Add(origin.connectedTile);

        return tiles;
    }

    /// <summary>
    /// Called by Interact.cs to create a path between two tiles on the grid 
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public Path PathBetween(Tile dest, Tile source)
    {
        Path path = MakePath(dest, source);
        illustrator.IllustratePath(path);
        return path;
    }

    /// <summary>
    /// Creates a path between two tiles
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    private Path MakePath(Tile destination, Tile origin)
    {
        List<Tile> tiles = new List<Tile>();
        Tile current = destination;

        while (current != origin)
        {
            tiles.Add(current);
            if (current.parent != null)
                current = current.parent;
            else
                break;
        }

        tiles.Add(origin);
        tiles.Reverse();

        Path path = new Path();
        path.tilesInPath = tiles.ToArray();

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

        currentFrontier.tiles.Clear();
    }
}
