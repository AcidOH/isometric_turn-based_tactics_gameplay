using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    private Camera cam;
    private Vector2 direction;

    private Overlay overlay;
    private Pathfinding pathFinding;
    private GameController gameController;

    private GameObject hitObject;
    private GameObject hitOverlayTile;
    private Vector3 hitOverlayTilePos;

    private GameObject selectedTile;
    private Vector3 selectedTilePos;

    private FighterStats hitFighterStats;

    public float unitRangeOfMovement
    {
        get { return gameController.currentAttacker.GetComponent<HumanEntity>().HumanData.movement.Value * 10; }
    }

    private bool searchForPath
    {
        get
        {
            if (hitFighterStats != null && hitFighterStats.lookForPath == true)
            {
                return true;
            }
            return false;
        }
    }


    void Start()
    {
        cam = Camera.main;
        Debug.Log("Camera height: " + cam.pixelHeight + ", width: " + cam.pixelWidth);

        overlay = GameObject.Find("A*").GetComponentInChildren<Overlay>();
        pathFinding = GameObject.Find("A*").GetComponentInChildren<Pathfinding>();
        
        gameController = GameObject.Find("GameControllerObject").GetComponentInChildren<GameController>();
    }

    void Update()
    {
        Vector3 tempMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(tempMousePos, Vector2.zero);

        bool thereIsAHeroOrEnemy = false;

        foreach (RaycastHit2D hitted in hits)
        {
            if (hitted.collider != null)
            {
                GameObject hittedObject = hitted.collider.gameObject;
                if (hittedObject.tag == "OverlayTileTag")
                {
                    hitOverlayTile = hittedObject; //the hit object isn't necessarily an overlay tile, but shares the position of the overlay tile beneath it (needs to be able to access that space in order for cursor to go over that area)
                    hitOverlayTilePos = hitted.transform.position;
                }

                if (hittedObject.tag == "Hero" || hittedObject.tag == "Enemy")
                {
                    thereIsAHeroOrEnemy = true;
                    hitObject = hittedObject;
                }
            }
        }
        if (!thereIsAHeroOrEnemy)
        {
            hitObject = hitOverlayTile;
        }


        if (gameController.currentAttacker == null)
        {
            hitFighterStats = null;
        }

        if (gameController.currentPhase == "Player Phase")
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("HitObject = " + hitObject);
                Debug.Log("CurrentAttacker = " + gameController.currentAttacker);
                
                if (hitOverlayTile != null)
                {
                    Debug.Log("A new tile was selected.");
                    
                    selectedTile = hitOverlayTile;
                    selectedTilePos = hitOverlayTilePos;
                }

                if (hitObject.tag == "Hero")
                {
                    Debug.Log("A Hero was clicked.");
                    
                    hitFighterStats = hitObject.GetComponent<FighterStats>();
                    hitFighterStats.SelectUnitAsAttacker();
                }
                else if (hitObject.tag == "Enemy")
                {
                    Debug.Log("An Enemy was clicked.");
                    
                    hitFighterStats = hitObject.GetComponent<FighterStats>();
                    hitFighterStats.SelectUnitAsVictim();
                }
                else
                {
                    Debug.Log("Neither a Hero nor Enemy was clicked.");
                    
                    foreach (Node node in overlay.grid)
                    {
                        node.overlayTile.GetComponentInChildren<IndividualTiles>().HideTile();
                    }

                    if (searchForPath == true)
                    {
                        Debug.Log("searchForPath == true");
                        hitFighterStats.Move(hitOverlayTilePos);
                    }
                }
            }

            if (searchForPath == true)
            {
                pathFinding.FindPath(selectedTilePos, hitOverlayTilePos, true, unitRangeOfMovement);
                selectedTile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }

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

}
