using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    private Camera cam;
    private Vector2 direction;

    private GameObject hitObject;

    [HideInInspector]
    public GameObject hitOverlayTile;

    [HideInInspector]
    public Vector3 hitOverlayTilePos;

    public GameObject player;

    private Overlay overlay;
    private Pathfinding pathFinding;
    private FighterStats fighterStats;

    [HideInInspector]
    public GameObject selectedUnit;

    [HideInInspector]
    public bool unitIsSelected = false;
    
    private float unitRangeOfMovement;

    private GameObject selectedTile;
    private Vector3 selectedTilePos;

    void Start()
    {
        cam = Camera.main;
        Debug.Log("Camera height: " + cam.pixelHeight + ", width: " + cam.pixelWidth);

        overlay = GameObject.Find("A*").GetComponentInChildren<Overlay>();
        pathFinding = GameObject.Find("A*").GetComponentInChildren<Pathfinding>();
        fighterStats = player.GetComponent<FighterStats>();
        unitRangeOfMovement = player.GetComponent<HumanEntity>().HumanData.movement.Value *10;

    }

    void Update()
    {
        Vector3 tempMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(tempMousePos, Vector2.zero);

        if (hit.collider != null)
        {
            hitObject = hit.collider.gameObject;
            
            hitOverlayTile = hitObject; //the hit object isn't necessarily an overlay tile, but shares the position of the overlay tile beneath it (needs to be able to access that space in order for cursor to go over that area)
            hitOverlayTilePos = hit.transform.position;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (hitObject.tag == "Hero")
            {
                if (!unitIsSelected)
                {
                    selectedUnit = hitObject;
                    unitIsSelected = true;

                    Debug.Log("A unit is selected.");
                } 
                else unitIsSelected = false;
            }
            else
            {
                selectedTile = hitOverlayTile;
                selectedTilePos = hitOverlayTilePos;
                foreach (Node node in overlay.grid)
                {
                    node.overlayTile.GetComponentInChildren<IndividualTiles>().HideTile();
                }
                //SelectTile();
                playerMovement(hitOverlayTilePos);
            }
        }
        
        if (selectedTile != null)
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

    //void SelectTile()
    //{
        //hitOverlayTile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    //}
    
    void playerMovement(Vector3 _posToMoveTo)
    {
        player.transform.position = _posToMoveTo;
        fighterStats.ShowTilesInRange();
    }
}
