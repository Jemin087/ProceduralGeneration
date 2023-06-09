using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;
public class AstarNode
{
    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost => GCost + HCost;

    public AstarNode Parent { get; set; }
    public Vector3Int Position { get; set; }
}

public class Astar : MonoBehaviour
{
    [SerializeField]
    Tilemap tilemap;

    [SerializeField]
    Tile[] groundTile;

    Tile targetTile;

    [SerializeField]
    Transform playerTr;

    private int currentPathIndex = 0;
    private List<Vector3Int> path;

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void FindPath(Vector2Int startPos, Vector2Int targetPos)
    {
        Vector3Int startTile = new Vector3Int(startPos.x, startPos.y, 0);
        Vector3Int targetTile = new Vector3Int(targetPos.x, targetPos.y, 0);

        List<AstarNode> openList = new List<AstarNode>();
        List<AstarNode> closedList = new List<AstarNode>();

        AstarNode startNode = new AstarNode { Position = startTile };
        AstarNode targetNode = new AstarNode { Position = targetTile };

        openList.Add(startNode);
        while(openList.Count>0)
        {
            AstarNode currentNode = openList[0];

            for(int i=1; i<openList.Count; i++)
            {
                if(openList[i].FCost<currentNode.FCost||(openList[i].FCost==currentNode.FCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if(currentNode.Position==targetNode.Position)
            {
                GeneratePath(startNode, currentNode);
                return;
            }


            Vector3Int[] neighborTiles = GetNeighborTiles(currentNode.Position);

            foreach(Vector3Int neighborTile in neighborTiles)
            {
                if(!IsTileWalkable(neighborTile))
                {
                    continue;
                }


                AstarNode neighborNode = new AstarNode
                {
                    Position = neighborTile,
                    Parent = currentNode,
                    GCost = currentNode.GCost + CalculateDistance(currentNode.Position, neighborTile),
                    HCost = CalculateDistance(neighborTile, targetNode.Position)
                };

                if(IsNodeInList(neighborNode,closedList))
                {
                    continue;
                }

                if(IsNodeInList(neighborNode,openList))
                {
                    AstarNode existingNode = openList.Find(node => node.Position == neighborNode.Position);
                    if(neighborNode.GCost<existingNode.GCost)
                    {
                        existingNode.GCost = neighborNode.GCost;
                        existingNode.Parent = currentNode;
                    }
                }
                else
                {
                    openList.Add(neighborNode);
                }

            }

        }


    }


    private void GeneratePath(AstarNode startNode, AstarNode endNode)
    {
        path = new List<Vector3Int>();
        AstarNode currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
    }


    private Vector3Int[] GetNeighborTiles(Vector3Int position)
    {
        Vector3Int[] neighborTiles = new Vector3Int[4];

        neighborTiles[0] = new Vector3Int(position.x - 1, position.y, position.z);
        neighborTiles[1] = new Vector3Int(position.x + 1, position.y, position.z);
        neighborTiles[2] = new Vector3Int(position.x, position.y-1, position.z);
        neighborTiles[3] = new Vector3Int(position.x, position.y+1, position.z);

        return neighborTiles;
    }

    bool IsTileWalkable(Vector3Int position)
    {
        TileBase tile = tilemap.GetTile(position);
        return tile != null && Array.Exists(groundTile, t => t == tile);
    }

    bool IsNodeInList(AstarNode node,List<AstarNode> list)
    {
        return list.Exists(n => n.Position == node.Position);
    }

    int CalculateDistance(Vector3Int a,Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int targetPos = new Vector2Int(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));

            
            Vector3Int clickedTile = tilemap.WorldToCell(mousePos);
            targetTile = tilemap.GetTile(clickedTile) as Tile;
      
            FindPath(new Vector2Int((int)playerTr.position.x, (int)playerTr.position.y), targetPos);
            currentPathIndex = 0;
        }

        if(path!=null&&path.Count>0&&currentPathIndex<path.Count)
        {
            float distance = Vector3.Distance(playerTr.position, path[currentPathIndex]);

            if(distance<0.1f)
            {
                currentPathIndex++;
            }

            if(currentPathIndex<path.Count)
            {
                Vector3 direction = path[currentPathIndex] - playerTr.position;

                float speed = 5f;

                playerTr.position += direction * speed * Time.deltaTime;
            }
        }
    }

}
