using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    private Camera cam;
    private Vector2 direction;

    [HideInInspector]
    public GameObject hitOverlayTile;

    [HideInInspector]
    public Vector3 hitOverlayTilePos;
    
    public GameObject player;

    Overlay overlay;
    Pathfinding pathFinding;

    private Vector3 selectedTilePos;

    void Start()
    {
        cam = Camera.main;
        Debug.Log("Camera height: " + cam.pixelHeight + ", width: " + cam.pixelWidth);

        overlay = GameObject.Find("A*").GetComponentInChildren<Overlay>();
        pathFinding = GameObject.Find("A*").GetComponentInChildren<Pathfinding>();
    }

    void Update()
    {
        Vector3 tempMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(tempMousePos, Vector2.zero);

        if (hit.collider != null)
        {
            hitOverlayTile = hit.collider.gameObject;
            hitOverlayTilePos = hit.transform.position;
        }

        if (Input.GetMouseButtonDown(0))
        {
            selectedTilePos = hitOverlayTilePos;
            foreach (Node node in overlay.grid)
            {
                node.overlayTile.GetComponentInChildren<IndividualTiles>().HideTile();
            }
            SelectTile();
            playerMovement(hitOverlayTilePos);
        }

        pathFinding.FindPath(selectedTilePos, hitOverlayTilePos);
    }

    void OnGUI()
    {
        Event currentEvent = Event.current;
        MoveMouse(currentEvent);
    }

    void MoveMouse(Event _currentEvent)
    {
        Vector2 mousePos = new Vector2();

        //Get the mouse position from Event
        //The y position from Event is inverted
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

    void SelectTile()
    {
        hitOverlayTile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

        //cam.transform.position = new Vector3(hitOverlayTilePos.x, hitOverlayTilePos.y, hitOverlayTilePos.z -10);
        //^use for beginning of player phase, moveing, selecting someone to attack
    }
    
    void playerMovement(Vector3 _posToMoveTo)
    {
        player.transform.position = _posToMoveTo + new Vector3Int(0,0,-3);
        //^this is silly because if you click on the same place twice you move out of the screen by 3 every time -_-
    }
}
