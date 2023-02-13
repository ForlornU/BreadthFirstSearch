using UnityEngine;

public enum InteractState {Walking};

public class Interact : MonoBehaviour
{
    Camera mainCam;

    public AudioClip click;
    public AudioClip pop;

    private Character hoverCharacter;
    public Character selectedCharacter;
    public LayerMask interactMask;
    Pathfinder pathfinder;
    Ray inputRay;
    InteractState state;

    private void Start()
    {
        mainCam = gameObject.GetComponent<Camera>();

        if (pathfinder == null)
            pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
    }

    private void Update()
    {
        MouseUpdate();
    }

    void MouseUpdate()
    {
        if (!Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, interactMask))
            return;

        Tile hoveredTile;
        DeselectHoverCharacter();

        //If we hover over a Tile
        if (hoveredTile = hit.transform.GetComponent<Tile>())
        {
            if (selectedCharacter == null || selectedCharacter.Moving)
                return;

            Navigate(hoveredTile);
        }

        //If we hover over a character
        else if (hit.transform.GetComponent<Character>())
        {
            hoverCharacter = hit.transform.GetComponent<Character>();
            if (hoverCharacter.Moving)
                return;
            hoverCharacter.characterTile.SetColor(HexColor.White);

            SelectCharacter();
        }

    }

    private void DeselectHoverCharacter()
    {
        if (hoverCharacter != null && hoverCharacter.characterTile != null)
            hoverCharacter.characterTile.ClearColor();

        hoverCharacter = null;
    }

    private void SelectCharacter()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectedCharacter = hoverCharacter;

            if (selectedCharacter.Moving)
                return;

            Frontier newFrontier = pathfinder.BreadthFirstSearch(selectedCharacter);
            selectedCharacter.CharactersFrontier = newFrontier;

            pathfinder.illustrator.IllustrateFrontier(newFrontier);
            GetComponent<AudioSource>().PlayOneShot(pop);

        }
    }

    private void Navigate(Tile clickedTile)
    {
        if (!clickedTile.Reachable())
            return;

        Path pathToIllustrate = pathfinder.GetPathBetween(clickedTile, selectedCharacter.characterTile);
        pathfinder.illustrator.IllustratePath(pathToIllustrate);

        if (Input.GetMouseButtonDown(0))
        {
            GetComponent<AudioSource>().PlayOneShot(click);

            Path path = pathfinder.GetPathBetween(clickedTile, selectedCharacter.characterTile);

            selectedCharacter.Move(path);

            pathfinder.ResetPathfinder();

            selectedCharacter = null;
        }
    }



    public void ChangeState(InteractState _state)
    {
        state = _state;
    }
}
    

