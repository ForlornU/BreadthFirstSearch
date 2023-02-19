using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region member fields
    public bool Moving { get; private set; } = false;

    public CharacterMoveData movedata;
    public Tile characterTile;
    [SerializeField]
    LayerMask GroundLayerMask;
    #endregion

    private void Awake()
    {
        FindTileAtStart();
    }

    /// <summary>
    /// If no starting tile has been manually assigned, we find one beneath us
    /// </summary>
    void FindTileAtStart()
    {
        if (characterTile != null)
        {
            FinalizePosition(characterTile);
            return;
        }

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 50f, GroundLayerMask))
        {
            FinalizePosition(hit.transform.GetComponent<Tile>());
            return;
        }

        Debug.Log("Unable to find a start position");
    }

    IEnumerator MoveThroughPath(Path path)
    {
        int step = 0;
        int pathlength = Mathf.Clamp(path.tilesInPath.Length, 0, movedata.MaxMove + 1);
        Tile currentTile = path.tilesInPath[0];
        float animationtime = 0f;
        const float minimumistanceFromNextTile = 0.05f;

        while (step < pathlength)
        {
            yield return null;
            Vector3 nextTilePosition = path.tilesInPath[step].transform.position;

            MoveAndRotate(currentTile.transform.position, nextTilePosition, animationtime / movedata.MoveSpeed);
            animationtime += Time.deltaTime;

            if (Vector3.Distance(transform.position, nextTilePosition) > minimumistanceFromNextTile)
                continue;

            currentTile = path.tilesInPath[step];
            step++;
            animationtime = 0f;
        }

        FinalizePosition(path.tilesInPath[pathlength - 1]);
    }

    public void Move(Path _path)
    {
        Moving = true;
        characterTile.Occupied = false;
        StartCoroutine(MoveThroughPath(_path));
    }

    void FinalizePosition(Tile tile)
    {
        transform.position = tile.transform.position;
        characterTile = tile;
        Moving = false;
        tile.Occupied = true;
        tile.occupyingCharacter = this;
    }

    void MoveAndRotate(Vector3 origin, Vector3 destination, float duration)
    {
        transform.position = Vector3.Lerp(origin, destination, duration);
        transform.rotation = Quaternion.LookRotation(origin.DirectionTo(destination).Flat(), Vector3.up);
    }

}
