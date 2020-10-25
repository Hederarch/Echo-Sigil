using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class BFS<T> : IPathFinder<T> where T : IBFSItem<T>
    {
        public List<T> visitedList = new List<T>();
        public int maxDistance = 2;
        public Path<T> FindPath(T start, T end)
        {
            visitedList.Clear();
            Queue<T> itemQueue = new Queue<T>();
            itemQueue.Enqueue(start);
            start.visited = true;

            while (itemQueue.Count > 0)
            {
                T item = itemQueue.Dequeue();
                
                if(item.Equals(end))
                {
                    return new Path<T>(end, start);
                }

                foreach(T neighbor in item.FindNeighbors())
                {
                    if (!neighbor.visited)
                    {
                        neighbor.distance = item.distance + neighbor.weight;
                        if(neighbor.distance < maxDistance)
                        {
                            visitedList.Add(neighbor);
                            itemQueue.Enqueue(neighbor);
                            neighbor.visited = true;
                            neighbor.parent = item;
                        }
                    }
                }
            }
            return new Path<T>(false);
        }
    }

    public interface IBFSItem<T> : IPathItem<T> where T : IBFSItem<T>
    {
        bool visited { get; set; }
        int distance { get; set; }

    } 
}