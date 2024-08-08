using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [HideInInspector]
    public Transform seeker, target;

    Overlay grid;
    PathIndicator pathIndicator;

    void Awake()
    {
	    grid = GetComponent<Overlay>();
        pathIndicator = GetComponent<PathIndicator>();
    }

    /*void Update()
    {
	    Debug.Log("seeker: " + seeker.position + ", target: " + target.position);
		FindPath(seeker.position, target.position);
    }*/

    public void FindPath(Vector3 startPos, Vector3 targetPos, bool drawPath, float range)
    {
        //Debug.Log("startPos = " + startPos + ", targetPos = " + targetPos);
        
        Node startNode = grid.NodeFromWorldPoint(startPos);
	    Node targetNode = grid.NodeFromWorldPoint(targetPos);
	
        Heap<Node> openSet = new Heap<Node>(grid.maxHeapSize);
	    HashSet<Node> closedSet = new HashSet<Node>();
	
        if (startNode != null && targetNode != null)
		{
            if (isInRange(startNode, targetNode, range))
            {
                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        RetracePath(startNode, targetNode, drawPath);
                        return;
                    }

                    foreach (Node neighbour in grid.GetNeighbours(currentNode))
                    {
                        if (!neighbour.walkable || closedSet.Contains(neighbour))
                        {
                            continue;
                        }

                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;

                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }
                        }
                    }
                }
            }
        }
    }   

    void RetracePath(Node startNode, Node endNode, bool _drawPath)
    {
        if (_drawPath) pathIndicator.ErasePath();
        
        List<Node> path = new List<Node>();
	    Node currentNode = endNode;
	
        while (currentNode != startNode)
	    {
		    path.Add(currentNode);
		    currentNode = currentNode.parent;
	    }
	
        path.Reverse();
	    grid.path = path;

        if (_drawPath) pathIndicator.DrawPath();
    }

    public bool isInRange(Node startNode, Node endNode, float _range)
    {
        if (grid.grid != null)
        {
            int originGridX = startNode.gridX;
            int originGridY = startNode.gridY;

            int nodeGridX = endNode.gridX;
            int nodeGridY = endNode.gridY;

            int distance = (Mathf.Abs(nodeGridX - originGridX) + Mathf.Abs(nodeGridY - originGridY)) * 10;

            if (distance <= _range) return true;
        }
        return false;
    }

    public int GetDistance(Node nodeA, Node nodeB)
    {
	    int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
	    int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
	
        if (dstX>dstY)
	    {
            return 14*dstY + 10*(dstX-dstY); //works for neighbouring nodes and when considering diagonal movement (diagonals aren't being considered...)
        }

        return 14*dstX + 10*(dstY-dstX);
    }
}
