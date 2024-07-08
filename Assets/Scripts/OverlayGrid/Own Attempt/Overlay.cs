using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//using UnityEngine.Timeline;

public class Overlay : MonoBehaviour
{
    public GameObject overlayTilePrefab;
    public GameObject overlayContainer;

    //public static Overlay instance;
    public List<Vector3Int> tiles; //contains all overlay tile positions (in tilemap coords) which only exist on very top of map
    public List<Vector2Int> tiles2D; //all overlay tile positions without z coordinate (in tilemap coords)
    public Dictionary<Vector3Int, Vector3> tilesGridandWorldPos;
    private bool preTileExists;

    //NODEGRID:
    public bool onlyDisplayPathGizmos;
    public LayerMask unwalkableMask;
    public int gridSizeX, gridSizeY; //can eventually switch to using tilemap.boundsint

    Node[,] grid;

    [HideInInspector]
    public List<Node> path;

    PathIndicator pathIndicator;

    public List<Sprite> arrowSprites;

    //What the code deletes depends on what Overlay is attached to
    //deletes Grid and Tilemap when not attached to Grid...
    //also sometimes deletes A* game object when attached to A*

    /*private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
            Debug.Log("This game object was destroyed");
        } else
        {
            instance = this;
            Debug.Log("instance was set to this");
        } //deletes overlay tiles if script is/has already been run
    }*/


    void Start()
    {
        pathIndicator = GetComponent<PathIndicator>();
        
        //why do you need to instantiate dictionaries but not lists?
        tilesGridandWorldPos = new Dictionary<Vector3Int, Vector3>();

        if (overlayTilePrefab != null)
        {
            preTileExists = true;
        }

        var tileMap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();
        Debug.Log("Tilemap: " + tileMap);
        
        tileMap.CompressBounds();

        Vector3Int origin = tileMap.origin;
        BoundsInt size = tileMap.cellBounds;

        Debug.Log("Tilemap origin: (" + origin.x + ", " + origin.y + ", " + origin.z + ")");
        Debug.Log("Boundsint size: (" + size.x + ", " + size.y + ", " + size.z + ")");

        foreach (var tileLocation in size.allPositionsWithin) //something goes wrong when z = -2. Why?
        {
            if (tileMap.HasTile(tileLocation) && !tiles2D.Contains(new Vector2Int(tileLocation.x +2, tileLocation.y+2)))
            {
                tiles.Add(tileLocation);
                tiles2D.Add(new Vector2Int(tileLocation.x, tileLocation.y));
            }
        }

        foreach (var location in tiles)
        {
            Vector3 cellPosition = tileMap.GetCellCenterWorld(location);
            TileBase tileBase = tileMap.GetTile(location);
            string tileName = tileBase.name.ToString();
            Vector3 tileOfset = tileTypeIdentifier(tileName);

            Vector3 tileWorldPos = cellPosition + tileOfset;
            tilesGridandWorldPos.Add(location - origin, tileWorldPos); //subtracting origin should make it so that the grid is onlt positive values, with bottom left being (0,0)

            if (preTileExists == true)
            {
                overlayTilePrefab.transform.position = tileWorldPos;
                preTileExists = false;
            } 
            else 
            {
                if (tileOfset != new Vector3(0,0,0))
                {
                    Instantiate(overlayTilePrefab, tileWorldPos, Quaternion.identity, overlayContainer.transform);
                }
            }
        }

        CreateGrid();
    }

    private Vector3 tileTypeIdentifier(string _tileName)
    {
        Vector3 _tileOfset = new Vector3(0,0,0);

        if (_tileName == "Dirt Path Full Height" || _tileName == "Grass and Flower Full Height" /*"isometric-sandbox-sheet_0"*/)
        {
            _tileOfset = new Vector3(0, 0.5f, -1);
        }
        if (_tileName == "Dirt Path Half Height" || _tileName == "Grass and Flower Half Height" /*"isometric-sandbox-sheet_1"*/)
        {
            _tileOfset = new Vector3(0, 0.25f, -1);
        }
        //You have to do this for every new type of tile you add to the tilemap.

        //THE BUG WHERE CERTAIN OVERLAYTILES DONT SORT PROPERLY BEHIDE TILEMAP TILES IS STILL PRESENT!
        
        return _tileOfset;
    }

    //NODEGRID:
    public int maxHeapSize
    {
        get
        {
            return gridSizeX*gridSizeY;
        }
    }
    
    void CreateGrid()
    {	
        grid = new Node[gridSizeX, gridSizeY];
        
        foreach (var overlayTilePos in tilesGridandWorldPos)
        {
            bool walkable = true;
			//bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadiusX, unwalkableMask));
			//^THIS MAY NOT BE THE WAY YOU WANT TO CHECK FOR COLLISIONS!
			
            grid[overlayTilePos.Key.x, overlayTilePos.Key.y] = new Node(walkable, overlayTilePos.Value, overlayTilePos.Key.x, overlayTilePos.Key.y);
		}
    }

    public List<Node> GetNeighbours(Node node)
    {
	    List<Node> neighbours = new List<Node>();
	    for (int x = -1; x <= 1; x++)
	    {
		    for (int y = -1; y <= 1; y++)
		    {
			    //if ((x == y) || (x == -1 && y == 1) || (x == 1 && y == -1)) //ignores of corners and center
			    if (x == 0 && y == 0)
                {
				    continue;
			    }
			    
                int checkX = node.gridX + x; //gridX and gridY are properties of Node class
			    int checkY = node.gridY + y;
			
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX,checkY]);
                }
		    }
	    }
	    return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 targetWorldPosition) //can get world point from raycast in from Cursor script
    {
		Debug.Log("NodeFromWorldPoint was called");
        Debug.Log("targetWorldPosition: " + targetWorldPosition);
        
        //THIS IS BORROWED FROM CURSOR SCRIPT! (You need to turn it into a function that you can reuse)
        RaycastHit2D hit = Physics2D.Raycast(targetWorldPosition, Vector2.zero);

        Debug.Log("Raycast hit: " + hit);

        if (hit.collider != null)
        {
            Debug.Log("hit.collider != null");
            GameObject hitOverlayTile = hit.collider.gameObject;
            Vector3 hitOverlayTilePos = hit.transform.position;

            if (tilesGridandWorldPos != null)
            {
                Debug.Log("tilesGridandWorldPos != null");
                foreach (var overlayTilePos in tilesGridandWorldPos)
                {
                    Debug.Log("In foreach loop");
                    if (hitOverlayTilePos == overlayTilePos.Value)
                    {
                        Debug.Log("hitOverlayTilePos is in overlayWorldGrid");
                        int x = overlayTilePos.Key.x;
                        int y = overlayTilePos.Key.y;

                        Debug.Log("int x and int y were set. " + x + ", " + y);

                        return grid[x, y];
                    }
                }
            } else
            {
                Debug.Log("tilesGridandWorldPos = null");
            }
			
			//int index = overlayWorldGrid.IndexOf(hitOverlayTilePos); //returns -1
			//Debug.Log("Index of hitOverlayTilePos in overlayWorldGrid: " + index);
		
			/*int x = overlayGrid[index].x;
			int y = overlayGrid[index].y;
	
        	return grid[x,y];*/
        }

        Debug.Log("hit.collider = null");
		return null;
    }

    /*void OnDrawGizmos()
    {
        if (grid != null)
        {
	        foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable)?Color.white:Color.red;
			
                if (path != null)
			    {  
	                if (path.Contains(n))
			        {
			            Gizmos.color = Color.black;
		            }
	            }
                
                Gizmos.DrawCube(n.worldPosition, new Vector3(0.25f, 0.25f, 0.25f));
		    }
        }
    }*/

    public void DrawPath()
    {
        if (path != null)
        {
            foreach (Node node in path)
            {
                int currentIndex = path.IndexOf(node);

                (Vector2Int, Vector2Int) prevAndnext = GetPrevAndNextNode(currentIndex);

                PathIndicator.ArrowDirection direction = pathIndicator.TranslateDirection(prevAndnext.Item1, prevAndnext.Item2);
                SetArrowSprite(direction);
            }
        }
    }

    private (Vector2Int,Vector2Int) GetPrevAndNextNode(int i)
    {
        Vector2Int curr = new Vector2Int(path[i].gridX, path[i].gridY);
        int h = i - 1;
        int j = i + 1;

        if (i == 0)
        {
            h = 0;
        }
        else if (i == path.Count - 1)
        {
            j = path.Count - 1;
        }

        Vector2Int prev = curr - new Vector2Int(path[h].gridX, path[h].gridY);
        Vector2Int next = new Vector2Int(path[j].gridX, path[j].gridY) - curr;

        return (prev, next);
    }

    private void SetArrowSprite(PathIndicator.ArrowDirection direction)
    {
        var arrow = GetComponentInChildren<SpriteRenderer>();

        if (direction == PathIndicator.ArrowDirection.None)
        {
            arrow.color = new Color(1, 1, 1, 0);
        }
        else
        {
            arrow.color = new Color(1, 1, 1, 1);
            arrow.sprite = arrowSprites[(int)direction];
            //arrow.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;  
        }
    }
}
