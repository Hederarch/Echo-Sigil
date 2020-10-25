using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pathfinding
{
    public class AStar<T> : IPathFinder<T> where T : IAStarItem<T>
    {
        public Path<T> FindPath(T start, T end)
        {
            Heap<T> openSet = new Heap<T>(start.GetMaxSize());
            Heap<T> closedSet = new Heap<T>(start.GetMaxSize());

            openSet.Add(start);

            while (openSet.Count > 0)
            {
                T current = openSet.RemoveFirst();

                closedSet.Add(current);

                if (current.Equals(end))
                {
                    return new Path<T>(end, start);
                }

                foreach (T neighbor in current.FindNeighbors())
                {
                    if (closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbor = current.G + current.GetDistance(neighbor) + neighbor.weight;
                    if (newMovementCostToNeighbor < neighbor.G || !openSet.Contains(neighbor))
                    {
                        neighbor.G = newMovementCostToNeighbor;
                        neighbor.H = neighbor.GetDistance(end);
                        neighbor.parent = current;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbor);
                        }
                    }
                }
            }
            return new Path<T>(false);
        }
    }

    public interface IAStarItem<T> : IHeapItem<T>, IPathItem<T> where T : IAStarItem<T>
    {
        int GetDistance(T other);
        int F { get; }
        int G { get; set; }
        int H { get; set; }

        int GetMaxSize();
    } 
}

