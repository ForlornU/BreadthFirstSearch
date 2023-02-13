using UnityEngine;

public enum InteractState {Walking};

public class Interact : MonoBehaviour
{
    Camera mainCam;
    //public GameObject CameraTarget;
    //public float cameraMoveSens = 0.1f;

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
        RaycastHit hit;
        if (!Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit, 200f, interactMask))
            return;

        Tile c;
        DeselectHoverCharacter();

        //If we hover over a Tile
        if (c = hit.transform.GetComponent<Tile>())
        {
            if (selectedCharacter == null)
                return;

            Navigate(c);
        }

        //If we hover over a character
        else if (hit.transform.GetComponent<Character>())
        {
            hoverCharacter = hit.transform.GetComponent<Character>();
            if (hoverCharacter.moving)
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
            //SnapTarget(selectedCharacter.transform);

            if (!selectedCharacter.moving)
            {
                Frontier f = pathfinder.BreadthFirstSearch(selectedCharacter);
                selectedCharacter.charactersFrontier = f;//.NewFrontier(f);
                pathfinder.ShowFrontierArea(f);
                GetComponent<AudioSource>().PlayOneShot(pop);
            }
        }
    }

    public void SelectCharacter(Character _char)
    {
        selectedCharacter = _char;
        //SnapTarget(_char.transform);
    }

    private void Navigate(Tile c)
    {
        if (c.Reachable() && !selectedCharacter.moving)
        {
            pathfinder.IllustrateBreadthPath(c);

            if (Input.GetMouseButtonDown(0))
            {
                Path p = pathfinder.GetBreadthPath(c);
                AddToPath(p);

                selectedCharacter.Move(p);
                //Cam code
                //GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>().ChangeCameraFollowState(true);
                //SnapTarget(selectedCharacter.transform);

                pathfinder.Clear();

                selectedCharacter = null;
            }

        }
    }

    void AddToPath(Path _path)
    {
        int moves = _path.Steps.Length;
        selectedCharacter.MovesLeft -= moves;
        Debug.Log(moves.ToString());

        GetComponent<AudioSource>().PlayOneShot(click);
    }

    //private void ControlTarget(Vector3 motion)
    //{
    //    CameraTarget.transform.position += (motion.normalized * cameraMoveSens * Time.deltaTime); 
    //}

    //private void SnapTarget(Transform _parentTarget)
    //{
    //    CameraTarget.transform.position = _parentTarget.position;
    //    CameraTarget.transform.parent = _parentTarget;
    //}
    //private void SnapTarget(Vector3 target)
    //{
    //    CameraTarget.transform.position = target;
    //}
    //private void UnsnapCamera()
    //{
    //    CameraTarget.transform.parent = null;
    //}

    public void ChangeState(InteractState _state)
    {
        state = _state;
    }

    //public void ShootMode(bool _state) //Change state to enum
    //{
    //    shoot = _state;

    //    //Enum states with a switch here
    //    //Move state will snap the camera back to target etc
    //}

}
    

