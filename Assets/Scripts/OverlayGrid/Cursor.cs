using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    private Camera cam;
    private Vector2 direction; 
    public GameObject hitOverlayTile;
    public Vector3 hitOverlayTilePos;
    public GameObject player;

    Overlay overlay;

    void Start()
    {
        cam = Camera.main;
        Debug.Log("Camera height: " + cam.pixelHeight + ", width: " + cam.pixelWidth);

        overlay = GameObject.Find("A*").GetComponentInChildren<Overlay>();
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
    }

    void OnGUI()
    {
        Event currentEvent = Event.current;
        MoveMouse(currentEvent);

        if (Input.GetMouseButtonDown(0))
        {
            foreach (Node node in overlay.grid)
            {
                node.overlayTile.GetComponentInChildren<IndividualTiles>().HideTile();
            }
            SelectTile();
        }
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
        if (hitOverlayTile.GetComponent<SpriteRenderer>().color != new Color(1, 1, 1, 1))
        {
            hitOverlayTile.GetComponent<IndividualTiles>().ShowTile();

            //cam.transform.position = new Vector3(hitOverlayTilePos.x, hitOverlayTilePos.y, hitOverlayTilePos.z -10);
            //^use for beginning of player phase, moveing, selecting someone to attack

            playerMovement(hitOverlayTilePos);
        }
    }
    
    void playerMovement(Vector3 _posToMoveTo)
    {
        player.transform.position = _posToMoveTo + new Vector3Int(0,0,-3);
        //^ -3 is an arbitrary number that only works if there aren't any tiles closer than -3.
    }
}
