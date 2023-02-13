using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathIllustrator : MonoBehaviour
{
    LineRenderer line;


    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    public void IllustratePath(Path _p)
    {
        line.positionCount = _p.Steps.Length;

        for (int i = 0; i < _p.Steps.Length; i++)
        {
           // _p.Steps[i].SetColor(HexColor.Green);
        }

       // _p.Steps[_p.Steps.Length - 1].SetColor(HexColor.Orange);

        for (int i = 0; i < _p.Steps.Length; i++)
        {
            line.SetPosition(i, _p.Steps[i].transform.position.With(y:0.33f)); //.With(y : 0.33f)); //Tiles should contain their position? Grid ID
        }

    }

    //public void IllustrateCost(Path p)
    //{
    //    foreach (Tile item in p.Steps)
    //    {
    //        item.testSetCostText(item.cost.ToString("F2"));
    //    }
    //}

    public void Clear()
    {
        line.positionCount = 0;

    }

    public void ClearLastFrontier(List <Tile> Tiles)
    {
        foreach (Tile item in Tiles)
        {
            item.ClearColor();
        }
    }

    public void ShowFrontierPath(Tile _d, Tile _s)
    {
        List<Tile> pathlist = new List<Tile>();
        Tile current = _d;

        Debug.Log("showing path");
        while (current != _s)
        {
            pathlist.Add(current);
            if (current.parent != null)
                current = current.parent;
            else
                break;
        }
        pathlist.Add(_s);
        pathlist.Reverse();

        Path newPath = new Path();
        newPath.Steps = pathlist.ToArray();

        IllustratePath(newPath);

    }

}
