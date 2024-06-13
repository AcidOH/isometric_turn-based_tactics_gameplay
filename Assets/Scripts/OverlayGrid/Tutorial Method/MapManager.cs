using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance { get { return _instance; } }
    
    public OverlayTile overlayTilePrefab;
    public GameObject overlayContainer;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            Debug.Log("This game object was destroyed");
        } else
        {
            _instance = this;
            Debug.Log("instance was set to this");
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();
        
        tileMap.CompressBounds(); //Even if tiles on tile map are erased, 
        //they are still included in the map. CompressBounds just changes it back to actual size
        BoundsInt bounds = tileMap.cellBounds; 

        Debug.Log("Bounds are " + bounds);
        Debug.Log("Bound max = " + bounds.max + ", bounds min = " + bounds.min);

        // Looping through all tiles
        for(int z = bounds.max.z; z > bounds.min.z; z--) //start for a z size of 0, and increase to bound size in z
        {
            for(int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for(int x = bounds.min.z; x < bounds.max.x; x++)
                {
                    Debug.Log("x,y,z: " + x + "," + y + "," + z);
                    var tileLocation = new Vector3Int(x,y,z);
                    
                    if(tileMap.HasTile(tileLocation))
                    {
                        Debug.Log("tile location:" + x + "," + y + "," + z);

                        //var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                        var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);
                        Instantiate(overlayTilePrefab, cellWorldPosition, Quaternion.identity, overlayContainer.transform);
                        
                        //overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z+1);
                        //overlayTile.transform.position = new Vector3(x,y,z+1);
                        //overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;
                    }
                }
            }
        }
    }

}
