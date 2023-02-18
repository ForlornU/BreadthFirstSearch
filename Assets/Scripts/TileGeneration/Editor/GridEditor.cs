using UnityEditor;
using UnityEngine;

public class GridEditor : EditorWindow
{
    #region membervariables
    GameObject parent;
    GameObject tileGO;
    Vector3 gridPosition;
    Vector2Int gridSize = new Vector2Int(15, 12);
    #endregion

    [MenuItem("Window / Tools / Grid Generator")]

    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(GridEditor));
        //window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 325, 175);
    }

    void OnGUI()
    {
        if (!CanShowWindow())
            return;

        SetFields();
    }

    void SetFields()
    {
        gridPosition = EditorGUILayout.Vector3Field("Grid Start Position", gridPosition);

        EditorGUILayout.Space();

        gridSize.x = Mathf.Clamp(EditorGUILayout.IntField("Width", gridSize.x), 0, 99);
        gridSize.y = Mathf.Clamp(EditorGUILayout.IntField("Length", gridSize.y), 0, 99);

        EditorGUILayout.Space(20f);

        if (GUILayout.Button("(re)Generate"))
            CreateGrid();

        if (GUILayout.Button("Create Ladder"))
            CreateLadder();

        if (GUILayout.Button("Connect character and tile"))
            SetCharacterStartTile();

    }

    void CreateGrid()
    {
        TileGenerator tg;

        AssignGridParent();

        if (!parent.GetComponent<TileGenerator>())
            tg = parent.AddComponent<TileGenerator>();
        else
            tg = parent.GetComponent<TileGenerator>();

        tg.GenerateGrid(tileGO, gridSize);
    }

    void AssignGridParent()
    {
        if (parent == null)
            parent = new GameObject("Grid");

        parent.transform.position = gridPosition;
    }

    bool CanShowWindow()
    {
        tileGO = (GameObject)EditorGUILayout.ObjectField("Tile", tileGO, typeof(GameObject), true);

        if (tileGO == null)
        {
            GUILayout.Label("Please attach a GameObject to create a grid from");
            return false;
        }

        return true;
    }

    void SetCharacterStartTile()
    {
        GameObject character = Selection.activeTransform.gameObject;

        if (Physics.Raycast(character.transform.position, Vector3.down, out RaycastHit hit, 5f))
        {
            character.GetComponent<Character>().characterTile = hit.transform.GetComponent<Tile>();
        }

    }

    void CreateLadder()
    {
        GameObject[] tiles = Selection.gameObjects;
        if (tiles.Length != 2)
            return;

        if (tiles[0].GetComponent<Tile>() && tiles[1].GetComponent<Tile>())
        {
            tiles[0].GetComponent<Tile>().connectedTile = tiles[1].GetComponent<Tile>();
            tiles[1].GetComponent<Tile>().connectedTile = tiles[0].GetComponent<Tile>();
        }
    }
}
