using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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

    public void FindPath(Vector3 startPos, Vector3 targetPos, bool drawPath, float range)
    {
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
                        RetraceAndSetPath(startNode, targetNode, drawPath);
                        return;
                    }

                    SearchThroughNeighbours(currentNode, targetNode, openSet, closedSet);
                }
            }
        }
    }

    public Node FindNodeWithinRange(Vector3 startPos, Vector3 targetPos, bool drawPath, float range)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        Heap<Node> openSet = new Heap<Node>(grid.maxHeapSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        if (startNode != null && targetNode != null)
        {
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePathToNode(startNode, targetNode, range);
                }

                SearchThroughNeighbours(currentNode, targetNode, openSet, closedSet);
            }
        }
        return targetNode;
    }

    private void SearchThroughNeighbours(Node _currentNode, Node _targetNode, Heap<Node> _openSet, HashSet<Node> _closedSet)
    {
        foreach (Node neighbour in grid.GetNeighbours(_currentNode))
        {
            if (!neighbour.walkable || _closedSet.Contains(neighbour))
            {
                continue;
            }

            int newMovementCostToNeighbour = _currentNode.gCost + GetDistance(_currentNode, neighbour);

            if (newMovementCostToNeighbour < neighbour.gCost || !_openSet.Contains(neighbour))
            {
                neighbour.gCost = newMovementCostToNeighbour;
                neighbour.hCost = GetDistance(neighbour, _targetNode);
                neighbour.parent = _currentNode;

                if (!_openSet.Contains(neighbour))
                {
                    _openSet.Add(neighbour);
                }
            }
        }
    }

    private void RetraceAndSetPath(Node startNode, Node endNode, bool _drawPath)
    {
        if (_drawPath) pathIndicator.ErasePath();

        List<Node> path = RetracePath(startNode, endNode);
        grid.path = path;

        if (_drawPath) pathIndicator.DrawPath();
    }

    private Node RetracePathToNode(Node startNode, Node endNode, float range)
    {
        List<Node> path = RetracePath(startNode, endNode);

        int rangeIndex = Mathf.RoundToInt(range) - 1 < 0? 0: Mathf.RoundToInt(range) - 1; //makes sure doesn't go below 0
        if (rangeIndex > path.Count-1) rangeIndex = path.Count-1;
        
        Node node = path[rangeIndex];

        return node;
    }

    private List<Node> RetracePath(Node _startNode, Node _endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = _endNode;

        while (currentNode != _startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        return path;
    }

    public int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY); //works for neighbouring nodes and when considering diagonal movement (diagonals aren't being considered...)
        }

        return 14 * dstX + 10 * (dstY - dstX);
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

}

