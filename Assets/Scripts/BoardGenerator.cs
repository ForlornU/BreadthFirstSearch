//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//public class BoardGenerator : EditorWindow
//{
//    #region Variables
//    GameObject piece;
//    int baseheight = 3;
//    float raylength = 6f;
//    float colLength = 0.4f;
//    bool useray = true;
//    bool snapToRay = true;
//    int xLength = 100;
//    int yLength = 100;
//    float dotangle = 0.5f;

//    GameObject bounds;
//    GameObject prefab;

//    float minVal = -10;
//    float maxVal = 10;
//    #endregion

//    [MenuItem("Window / BoardControl")]
//    public static void ShowWindow()
//    {
//        GetWindow(typeof(BoardGenerator));
//    }

//    private void OnGUI()
//    {
//        baseheight = EditorGUILayout.IntField("Base Height", baseheight);
//        useray = EditorGUILayout.Toggle("Use Raycast", useray);
//        snapToRay = EditorGUILayout.Toggle("Snap to ray hit position", snapToRay);
//        raylength = EditorGUILayout.FloatField("Ray Length", raylength);
//        colLength = EditorGUILayout.FloatField("Col Length", colLength);
//        bounds = (GameObject)EditorGUI.ObjectField(new Rect(3, 400, position.width - 6, 20),"Bounds Mesh", bounds, typeof(GameObject), true);
//        xLength = EditorGUILayout.IntField("xLength", xLength);
//        yLength = EditorGUILayout.IntField("yLength", yLength);
//        EditorGUILayout.Space();
//        GUILayout.Label("Clamp Y position");
//        minVal = EditorGUILayout.FloatField("Minimum Y", minVal);
//        maxVal = EditorGUILayout.FloatField("Maximum Y", maxVal);
//        dotangle = EditorGUILayout.Slider("Dot Angle ",dotangle, 0f, 1f);

//        if (Selection.activeTransform != null)
//            piece = Selection.activeTransform.gameObject;

//        //piece = (GameObject)EditorGUI.ObjectField(new Rect(3, 355, position.width - 6, 20), "Prefab", piece, typeof(GameObject), true);

//        EditorGUILayout.Space();

//        if (GUILayout.Button("Generate"))
//        {
//            List <Cell> allcells = GenerateBoard();
//            ConnectBoard(allcells);
//        }

//        EditorGUILayout.Space();

//        GUILayout.Label("Select two tiles to create a ladder");
//        if (GUILayout.Button("Create Ladder"))
//        {
//            CreateLadder();
//        }

//        GUILayout.Label("Find node cell");
//        if (GUILayout.Button("Find node ground"))
//        {
//            ConnectNode();
//        }

//    }

//    void ConnectNode()
//    {
//        GameObject Node = Selection.activeTransform.gameObject;
//        RaycastHit hit;
//        if(Physics.Raycast(Node.transform.position, Vector3.down, out hit, 5f))
//        {
//            Node.GetComponent<CharacterLocomotion>().characterCell = hit.transform.GetComponent<Cell>();
//        }

//    }

//    void CreateLadder()
//    {
//        GameObject[] tiles = Selection.gameObjects;
//        if (tiles.Length != 2)
//            return;

//        if (tiles[0].GetComponent<Cell>() && tiles[1].GetComponent<Cell>())
//        {
//            tiles[0].GetComponent<Cell>().ladder = tiles[1].GetComponent<Cell>();
//            tiles[1].GetComponent<Cell>().ladder = tiles[0].GetComponent<Cell>();
//        }
//    }

//    Vector2 determineCellSize()
//    {
//        Bounds b = piece.GetComponent<MeshFilter>().sharedMesh.bounds;

//        float x = (b.extents.x * 2) * 0.75f;
//        float y = (b.extents.z * 2);

//        return new Vector2(x, y);
//    }

//    void determineBoardSize()
//    {
//        if (bounds == null)
//        {
//            Debug.Log("No bounds item set, defaulting to length values");
//            return;
//        }
//        Debug.Log("Using " + bounds.name + " mesh for size");
//        Bounds b = bounds.GetComponent<MeshFilter>().sharedMesh.bounds;

//        xLength = Mathf.RoundToInt(b.extents.x * 2);
//        yLength = Mathf.RoundToInt(b.extents.z * 2);
//    }

//    List<Cell> GenerateBoard()
//    {

//        List<Cell> CellList = new List<Cell>();

//        determineBoardSize();

//        int startZ = 0-(yLength / 2);

//        Vector2 size = determineCellSize();
//        Vector3 newPos = new Vector3(0-xLength*0.4f, 0, startZ);
//        GameObject parent = new GameObject("Board h:"+baseheight.ToString()+"ray:"+useray.ToString());

//        for (int x = 0; x < xLength; x++)
//        {
//            newPos.x += size.x;
//            newPos.z = startZ + zOffset(x % 2 == 0, size.y);

//            for (int z = 0; z < yLength; z++)
//            {
//                newPos.z += size.y;
//                newPos.y = baseheight;
//                RayData groundRay = CheckForGround(newPos);

//                if (useray == true)
//                {
//                    if(groundRay.hit == false)
//                        continue;

//                    if (snapToRay)
//                    {
//                        float h = groundRay.point.y+0.1f;
//                        h = Mathf.Round(h * 10) * 0.1f; // Round to closest 0.1
//                        newPos = groundRay.point.With(y: h);
//                    }
//                }

//                if (Blocked(newPos))
//                    continue;

//                if (newPos.y < minVal || newPos.y > maxVal)
//                    continue;

//                CellList.Add(CreateTile(newPos, new Vector2(x, z),parent.transform));
//            }
//        }

//        return CellList;
//    }

//    Cell CreateTile(Vector3 pos, Vector2 _gridID, Transform parent)
//    {
//        GameObject newPiece = Instantiate(piece, pos, Quaternion.identity, parent.transform);
//        newPiece.GetComponent<Cell>().SetID(_gridID);//(new Vector2(x, z));
//        newPiece.SetActive(true);
//        return newPiece.GetComponent<Cell>();

//    }

//    void ConnectBoard(List <Cell> list)
//    {
//        //Based on grid ID, find all neighbors and connect them
//        foreach (Cell c in list)
//        {
            
//        }
//    }

//    float zOffset(bool _even, float _y)
//    {
//        float z = 0;

//        if (!_even)
//            z = _y / 2;

//        return z;
//    }

//    RayData CheckForGround(Vector3 pos)
//    {
//        RayData returnData = new RayData();
//        RaycastHit h;

//        if (Physics.Raycast(pos, Vector3.down, out h, raylength))
//        {
//            returnData.hit = true;

//            if (h.transform.gameObject.layer == LayerMask.NameToLayer("Blocker"))
//                returnData.hit = false;

//            float dot = Vector3.Dot(h.normal, Vector3.up);

//            if (dot < dotangle)
//            {
//                returnData.hit = false;
//                Debug.Log(dot);
//            }


//            returnData.point = h.point;
//        }

//        return returnData;

//    }

//    bool Blocked(Vector3 pos)
//    {

//        Collider[] c = Physics.OverlapSphere(pos, colLength);
//        bool blocked = false;

//        if (c.Length > 0)
//        {
//            for (int i = 0; i < c.Length; i++)
//            {
//                if (c[i].gameObject.layer == LayerMask.NameToLayer("Blocker"))
//                {
//                    blocked = true;
//                    return blocked;
//                }
//            }
//        }

//        return blocked;
//    }
//}
