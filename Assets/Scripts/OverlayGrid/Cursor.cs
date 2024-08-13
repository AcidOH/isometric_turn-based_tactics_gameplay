using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    private Camera cam;
    private Vector2 direction;

    private Overlay overlay;
    private Pathfinding pathFinding;
    private GameObject actionMenu;
    private GameController gameController;

    private GameObject hitObject;
    private GameObject hitOverlayTile;
    private Vector3 hitOverlayTilePos;

    private GameObject selectedTile;
    private Vector3 selectedTilePos;

    [HideInInspector]
    public GameObject selectedUnit;

    [HideInInspector]
    public FighterStats selectedUnitsFighterStats;

    [HideInInspector]
    public GameObject selectedEnemy;

    private float unitRangeOfMovement;


    void Start()
    {
        cam = Camera.main;
        Debug.Log("Camera height: " + cam.pixelHeight + ", width: " + cam.pixelWidth);

        overlay = GameObject.Find("A*").GetComponentInChildren<Overlay>();
        pathFinding = GameObject.Find("A*").GetComponentInChildren<Pathfinding>();
        
        actionMenu = GameObject.Find("ActionMenu");
        gameController = GameObject.Find("GameControllerObject").GetComponentInChildren<GameController>();
    }

    void Update()
    {
        Vector3 tempMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        //RaycastHit2D hit = Physics2D.Raycast(tempMousePos, Vector2.zero);

        RaycastHit2D[] hits = Physics2D.RaycastAll(tempMousePos, Vector2.zero);

        foreach (RaycastHit2D hitted in hits)
        {
            if (hitted.collider != null)
            {
                hitObject = hitted.collider.gameObject;

                Debug.Log("Hit object tag: " + hitObject.tag);

                if (hitObject.tag == "OverlayTileTag")
                {
                    hitOverlayTile = hitObject; //the hit object isn't necessarily an overlay tile, but shares the position of the overlay tile beneath it (needs to be able to access that space in order for cursor to go over that area)
                    hitOverlayTilePos = hitted.transform.position;
                }
                if (hitObject.tag == "Hero" || hitObject.tag == "Enemy")
                {
                    break;
                }
            }
        }

        /*if (hit.collider != null)
        {
            hitObject = hit.collider.gameObject;
            
            hitOverlayTile = hitObject; //the hit object isn't necessarily an overlay tile, but shares the position of the overlay tile beneath it (needs to be able to access that space in order for cursor to go over that area)
            hitOverlayTilePos = hit.transform.position;
        }*/

        if (Input.GetMouseButtonDown(0))
        {          
            if (hitObject.tag == "Hero") //hero unit is selected or unselected (no other functionality).
            {
                selectedUnitsFighterStats = hitObject.GetComponent<FighterStats>();
                Debug.Log("A Hero Object was selected");

                if (selectedUnitsFighterStats.isSelectable)
                {
                    if (!selectedUnitsFighterStats.isSelected)
                    {
                        selectedUnit = hitObject;
                        unitRangeOfMovement = selectedUnit.GetComponent<HumanEntity>().HumanData.movement.Value * 10;

                        if (hitOverlayTile != null)
                        {
                            selectedTile = hitOverlayTile;
                            selectedTilePos = hitOverlayTilePos;
                        }

                        selectedUnitsFighterStats.isSelected = true;

                        selectedUnitsFighterStats.ShowTilesInRange();

                        Debug.Log("A unit is selected.");
                    }
                    else //unit is selected and is clicked on again
                    {
                        selectedUnitsFighterStats.hasMoved = true;
                        //actionMenu.SetActive(true);
                        gameController.FirstTurn();
                    } 
                }
            }
            else if (hitObject.tag == "Enemy") //an enemy unit is selected (no other functionality).
            {
                Debug.Log("An enemy was clicked");
                if (selectedUnitsFighterStats.isSelected)
                {
                    selectedEnemy = hitObject;
                    Debug.Log("An enemy was selected");
                }
            }
            else //an overlay tile is selected. If the player hasn't moved yet and the selected tile is in range, then the player can move.
            {// if player hasn't moved and the tile is out of range, the unit is unselected.

                if (hitOverlayTile != null)
                {
                    selectedTile = hitOverlayTile;
                    selectedTilePos = hitOverlayTilePos;
                }

                Debug.Log("Something that wasn't a hero or enemy was clicked");

                foreach (Node node in overlay.grid)
                {
                    node.overlayTile.GetComponentInChildren<IndividualTiles>().HideTile();
                }

                if (selectedUnitsFighterStats != null && !selectedUnitsFighterStats.hasMoved && pathFinding.isInRange(overlay.NodeFromWorldPoint(selectedUnit.transform.position), overlay.NodeFromWorldPoint(hitOverlayTilePos), unitRangeOfMovement))
                {
                    playerMovement(hitOverlayTilePos);
                }
                else if (selectedUnitsFighterStats != null)
                {
                    selectedUnitsFighterStats.isSelected = false;
                    Debug.Log("The selected hero was unselected");
                }
            }
        }
        
        if (selectedTile != null && selectedUnitsFighterStats != null && selectedUnitsFighterStats.isSelected && !selectedUnitsFighterStats.hasMoved)
        {
            pathFinding.FindPath(selectedTilePos, hitOverlayTilePos, true, unitRangeOfMovement);
            selectedTile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    }

    void OnGUI()
    {
        Event currentEvent = Event.current;
        MoveMouse(currentEvent);
    }

    void MoveMouse(Event _currentEvent)
    {
        Vector2 mousePos = new Vector2();

        mousePos.x = _currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - _currentEvent.mousePosition.y;

        Vector3 worldMousePos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
        //transform.position = worldMousePos; //just follows mouse exactly
        transform.position = new Vector3(hitOverlayTilePos.x, hitOverlayTilePos.y, hitOverlayTilePos.z - 0.5f);

        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + cam.pixelWidth + ":" + cam.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + worldMousePos.ToString("F3"));
        GUILayout.EndArea();
    }

    void playerMovement(Vector3 _posToMoveTo)
    {
        selectedUnit.transform.position = _posToMoveTo;
     
        selectedUnitsFighterStats.hasMoved = true;
        Debug.Log("The hero has moved.");

        //actionMenu.SetActive(true);
        gameController.FirstTurn();
    }

}
