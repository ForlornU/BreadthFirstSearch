using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    #region Members and Events
    public bool moving { get; private set; } = false;
    public CharacterMoveData movedata;
    public int MovesLeft;
    public Tile characterTile;
    public LayerMask GroundLayerMask;
    public Frontier charactersFrontier { get; set; }

    //Events
    public delegate void OnCompleteActions();
    public event OnCompleteActions finished;
    #endregion

    private void Awake()
    {
        MovesLeft = movedata.MaxMove;
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

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 50f, GroundLayerMask))
        {
            FinalizePosition(hit.transform.GetComponent<Tile>());
            return;
        }

        Debug.Log("Unable to find a start position");
    }

    IEnumerator MoveThroughPath(Path path, Tile startingPoint)
    {
        int step = 0;
        int last = Mathf.Clamp(path.Steps.Length, 0, movedata.MaxMove + 1);
        Tile current = startingPoint;
        float time = 0f;
        float lerptime = 0f;

        while (time <= last)
        {
            transform.position = Vector3.Lerp(current.transform.position, path.Steps[step].transform.position, lerptime / movedata.MoveSpeed);
            time += Time.deltaTime;
            lerptime += Time.deltaTime;

            if (Vector3.Distance(transform.position, path.Steps[step].transform.position) < 0.05f)
            {
                current = path.Steps[step];
                step++;
                lerptime = 0f;

                int s = step;
                if (step >= path.Steps.Length)
                {
                    step--;
                    time += 0.1f;

                    s = Mathf.Clamp(step, 0, path.Steps.Length);
                }


                Vector3 dir = path.Steps[s - 1].transform.position - path.Steps[s].transform.position;
                RotateToTile(dir);
            }

            yield return null;
        }

        FinalizePosition(path.Steps[last - 1]);
    }

    bool PathIsValid(Path p)
    {
        bool canmove = true;

        if (p == null)
            canmove = false;
        else if (p.Steps == null)
            canmove = false;
        else if (p.Steps.Length < 1)
            canmove = false;

        return canmove;
    }

    public void Move(Path _path)
    {
        if (!PathIsValid(_path))
        {
            finished();
            Debug.Log("Path invalid, stopped early");
            return;
        }

        moving = true;
        characterTile.occupied = false;
        StartCoroutine(MoveThroughPath(_path, characterTile));
    }

    void FinalizePosition(Tile tile)
    {
        transform.position = tile.transform.position;
        characterTile = tile;
        moving = false;
        tile.occupied = true;
        finished?.Invoke(); // Signal end of our event if not null (?.)
    }

    void RotateToTile(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(-direction, Vector3.up);
    }

}
