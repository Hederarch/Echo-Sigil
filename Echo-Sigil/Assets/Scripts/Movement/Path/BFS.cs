using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class BFS<T> : IPath<T> where T : IBFSItem<T>
    {
        public List<T> visitedList = new List<T>();
        public int maxDistance = 2;

        public bool PathFound { get; private set; } = false;

        private Stack<T> path = new Stack<T>();

        public BFS(T start, T end, int maxDistance = 2)
        {
            this.maxDistance = maxDistance;
            FindPath(start, end);
        }

        public void FindPath(T start, T end)
        {
            visitedList.Clear();
            Queue<T> itemQueue = new Queue<T>();
            itemQueue.Enqueue(start);
            visitedList.Add(start);
            start.distance = 0;

            while (itemQueue.Count > 0)
            {
                T item = itemQueue.Dequeue();
                
                if(item.Equals(end))
                {
                    PathFound = true;
                    T current = end;
                    path.Push(current);
                    while (!current.Equals(start))
                    {
                        current = current.parent;
                        path.Push(current);
                    }
                }

                foreach(T neighbor in item.FindNeighbors())
                {
                    if (!visitedList.Contains(neighbor))
                    {
                        neighbor.distance = item.distance + neighbor.weight;
                        if(neighbor.distance < maxDistance)
                        {
                            visitedList.Add(neighbor);
                            itemQueue.Enqueue(neighbor);
                            neighbor.parent = item;
                        }
                    }
                }
            }
        }

        public IEnumerator<T> GetConsideredEnum()
        {
            return visitedList.GetEnumerator();
        }

        public IEnumerator<T> GetPathEnum()
        {
            return path.GetEnumerator();
        }
    }

    public interface IBFSItem<T> : IPathItem<T> where T : IBFSItem<T>
    {
        int distance { get; set; }
    } 
}