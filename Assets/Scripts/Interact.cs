using UnityEngine;

public class Interact : MonoBehaviour
{
    #region members
    [SerializeField]
    public AudioClip click;
    [SerializeField]
    public AudioClip pop;
    [SerializeField]
    public LayerMask interactMask;

    Camera mainCam;
    Character hoverCharacter;
    Character selectedCharacter;
    Pathfinder pathfinder;
    #endregion

    private void Start()
    {
        mainCam = gameObject.GetComponent<Camera>();

        if (pathfinder == null)
            pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
    }

    private void Update()
    {
        DeselectHoverCharacter();
        MouseUpdate();
    }

    private void MouseUpdate()
    {
        if (!Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, interactMask))
            return;

        if (hit.transform.GetComponent<Tile>())
            HoverOverTile(hit.transform.GetComponent<Tile>());

        else if (hit.transform.GetComponent<Character>())
            HoverOverCharacter(hit.transform.GetComponent<Character>());
    }

    private void HoverOverTile(Tile tile)
    {
        if (selectedCharacter == null || selectedCharacter.Moving)
            return;

        Navigate(tile);
    }

    private void HoverOverCharacter(Character character)
    {
        if (character.Moving)
            return;

        hoverCharacter = character;
        hoverCharacter.characterTile.SetColor(HexColor.White);

        if (Input.GetMouseButtonDown(0))
            SelectCharacter(hoverCharacter);
    }

    private void DeselectHoverCharacter()
    {
        if (hoverCharacter != null && hoverCharacter.characterTile != null)
            hoverCharacter.characterTile.ClearColor();

        hoverCharacter = null;
    }

    private void SelectCharacter(Character character)
    {
        selectedCharacter = hoverCharacter;

        Frontier newFrontier = pathfinder.BreadthFirstSearch(selectedCharacter);
        selectedCharacter.CharactersFrontier = newFrontier;

        pathfinder.illustrator.IllustrateFrontier(newFrontier);
        GetComponent<AudioSource>().PlayOneShot(pop);
    }

    private void Navigate(Tile clickedTile)
    {
        if (!clickedTile.Reachable())
            return;

        Path currentPath = pathfinder.GetPathBetween(clickedTile, selectedCharacter.characterTile);
        pathfinder.illustrator.IllustratePath(currentPath);

        if (Input.GetMouseButtonDown(0))
        {
            GetComponent<AudioSource>().PlayOneShot(click);

            selectedCharacter.Move(currentPath);

            pathfinder.ResetPathfinder();

            selectedCharacter = null;
        }
    }
}
    

