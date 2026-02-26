using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class Pathfinding
{
    public enum PathfindingType
    {
        Walkable = 0,
        Direct = 1
    }
    public static List<NavigationNode> FindPath(NavigationNode start, NavigationNode end, PathfindingType pathfindingType = PathfindingType.Walkable)
    {
        if (start == null || end == null)
            return new List<NavigationNode>();

        List<NavigationNode> toSearch = new();
        List<NavigationNode> processed = new();
        toSearch.Add(start);

        while (toSearch.Count > 0)
        {
            NavigationNode current = toSearch[0];
            foreach(NavigationNode node in toSearch)
            {
                if(node.Score < current.Score || (node.Score == current.Score && node.StepsToEnd < current.StepsToEnd))
                    current = node;
            }

            processed.Add(current);
            toSearch.Remove(current);

            if(current == end)
            {
                NavigationNode currentPathNode = end;
                List<NavigationNode> path = new();
                var count = 170;
                while (currentPathNode != start)
                {
                    path.Add(currentPathNode);
                    currentPathNode = currentPathNode.Connection;
                    count--;
                    if (count < 0) return null;
                }

                return path;
            }

            switch (pathfindingType)
            {
                default:
                case PathfindingType.Walkable:
                    WalkablePathinding(current, toSearch, processed, start, end);
                    break;

                case PathfindingType.Direct:
                    DirectPathinding(current, toSearch, processed, start, end);
                    break;
            }
            //foreach(NavigationNode neighbor in current.Neighbors.Where(n => n.Tile.IsWalkable && !processed.Contains(n)))
            //{
            //    bool inSearch  = toSearch.Contains(neighbor);
            //    int cost = current.StepsFromStart + NavigationNode.GetDistanceTo(current, neighbor);

            //    if (!inSearch || cost < neighbor.StepsFromStart)
            //    {
            //        neighbor.StepsFromStart = cost;
            //        neighbor.Connection = current;

            //        if (!inSearch)
            //        {
            //            neighbor.StepsToEnd = NavigationNode.GetDistanceTo(neighbor, end);
            //            toSearch.Add(neighbor);
            //        }
            //    }
            //}
        }


        return null;
    }
    private static void WalkablePathinding(NavigationNode current, List<NavigationNode> toSearch, List<NavigationNode> processed, NavigationNode start, NavigationNode end)
    {
        foreach(NavigationNode neighbor in current.Neighbors.Where(n => (n.Tile.IsWalkable && !n.Tile.IsOccupied) && !processed.Contains(n)))
        {
            bool inSearch  = toSearch.Contains(neighbor);
            int cost = current.StepsFromStart + NavigationNode.GetDistanceTo(current, neighbor);

            if (!inSearch || cost < neighbor.StepsFromStart)
            {
                neighbor.StepsFromStart = cost;
                neighbor.Connection = current;
            if (!inSearch)
            {
                    neighbor.StepsToEnd = NavigationNode.GetDistanceTo(neighbor, end);
                    toSearch.Add(neighbor);
                }
            }
        }
    }
    private static void DirectPathinding(NavigationNode current, List<NavigationNode> toSearch, List<NavigationNode> processed, NavigationNode start, NavigationNode end)
    {
        foreach(NavigationNode neighbor in current.Neighbors.Where(n => n.Tile.IsWalkable && !processed.Contains(n)))
        {
            bool inSearch  = toSearch.Contains(neighbor);
            int cost = current.StepsFromStart + NavigationNode.GetDistanceTo(current, neighbor);

            if (!inSearch || cost < neighbor.StepsFromStart)
            {
                neighbor.StepsFromStart = cost;
                neighbor.Connection = current;
            if (!inSearch)
            {
                    neighbor.StepsToEnd = NavigationNode.GetDistanceTo(neighbor, end);
                    toSearch.Add(neighbor);
                }
            }
        }
    }
}
