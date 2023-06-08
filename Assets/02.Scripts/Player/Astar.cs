using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AstarNode
{
    public Vector2Int Position { get; }
    public bool IsWalkable { get; }

    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost => GCost + HCost;

    public AstarNode Parent { get; set; }

    public AstarNode(Vector2Int position,bool isWalkable)
    {
        Position = position;
        IsWalkable = isWalkable;
    }
}

public class Astar : MonoBehaviour
{
    private Tilemap tilemap;
    private List<AstarNode> openList;
    private List<AstarNode> closeList;
    private Dictionary<AstarNode, AstarNode> cameFrom;
    private Dictionary<AstarNode, float> gScore;
    private Dictionary<AstarNode, float> fScore;

   
    public float playerMoveSpeed = 5f;

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }


    public List<Vector2Int> FindPath(Vector2Int start,Vector2Int end)
    {
        if (!IsVaildPosition(start) || !IsVaildPosition(end))
        {
            return null;
        }
        AstarNode startNode = new AstarNode(start, true);
        AstarNode endNode = new AstarNode(end, true);

        openList = new List<AstarNode>();
        closeList = new List<AstarNode>();
        openList.Add(startNode);

        cameFrom = new Dictionary<AstarNode, AstarNode>();
        gScore = new Dictionary<AstarNode, float>();
        fScore = new Dictionary<AstarNode, float>();

        int maxSearch = 1000;
        gScore[startNode] = 0;
        
        fScore[startNode] = HeuristicCostEstimate(startNode,endNode);



        while (openList.Count > 0&&maxSearch>0)
        {
            AstarNode currentNode = GetLowestFScoreNode();

            if (currentNode == endNode)
            {
                return ReconstructPath(cameFrom, currentNode);
            }

            closeList.Add(currentNode);
            openList.Remove(currentNode);

            AstarNode[] neighbors = GetNeighbors(currentNode).ToArray();

            foreach (AstarNode neighbor in neighbors)
            {
                if (closeList.Contains(neighbor)||!IsWalkable(neighbor.Position))
                {
                    continue;
                }

                float tentativeGScore = gScore[currentNode] + GetDistance(currentNode, neighbor);

                if (!openList.Contains(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = currentNode;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, endNode);

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }

            maxSearch--;
        }

        if(maxSearch<=0)
        {
            Debug.Log("Å½»ö È½¼ö ÃÊ°ú");
        }


        return null;
    }

    AstarNode GetLowestFScoreNode()
    {
        AstarNode lowestNode = openList[0];
        foreach(AstarNode node in openList)
        {
            if(fScore[node]<fScore[lowestNode])
            {
                lowestNode = node;
            }
        }
        return lowestNode;
    }


    float HeuristicCostEstimate(AstarNode nodeA,AstarNode nodeB)
    {
        return Vector2Int.Distance(nodeA.Position, nodeB.Position);
    }

    float GetDistance(AstarNode nodeA,AstarNode nodeB)
    {
        return Vector2Int.Distance(nodeA.Position, nodeB.Position);
    }


    List<Vector2Int> ReconstructPath(Dictionary<AstarNode,AstarNode> cameFrom,AstarNode currentNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        while(cameFrom.ContainsKey(currentNode))
        {
            path.Add(currentNode.Position);
            currentNode = cameFrom[currentNode];
        }

        path.Reverse();
        return path;
    }

    List<AstarNode> GetNeighbors(AstarNode node)
    {
        List<AstarNode> neighbors = new List<AstarNode>();
        Vector2Int[] directions={ Vector2Int.up,Vector2Int.down,Vector2Int.right,Vector2Int.left};

        foreach(Vector2Int direction in directions)
        {
            Vector2Int neighborPos = node.Position + direction;
            if (IsVaildPosition(neighborPos))
            {
                AstarNode neighborNode = new AstarNode(neighborPos, true);
                neighbors.Add(neighborNode);
            }
            

        }

        return neighbors;
    }


    bool IsVaildPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < 64 && position.y >= 0 && position.y < 64;
    }


    bool IsWalkable(Vector2Int position)
    {
        return tilemap.GetTile((Vector3Int)position) != null;
    }



}
