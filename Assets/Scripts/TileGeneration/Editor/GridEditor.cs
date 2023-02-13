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

    private void OnGUI()
    {
        if (!CanShowWindow())
            return;

        SetFields();
    }

    private void SetFields()
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

    private void AssignGridParent()
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
        RaycastHit hit;

        if (Physics.Raycast(character.transform.position, Vector3.down, out hit, 5f))
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
            tiles[0].GetComponent<Tile>().ladder = tiles[1].GetComponent<Tile>();
            tiles[1].GetComponent<Tile>().ladder = tiles[0].GetComponent<Tile>();
        }
    }
}
