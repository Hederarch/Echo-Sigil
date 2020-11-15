using System.Collections.Generic;

namespace Pathfinding
{
    public class AStar<T> : IPath<T> where T : IAStarItem<T>
    {
        List<T> visitedList = new List<T>();
        public bool PathFound { get; private set; } = false;

        public int MaxDistance { private get; set; }

        public T LastPathElement => throw new System.NotImplementedException();

        Stack<T> path = new Stack<T>();

        public AStar(T start, T end, int maxDistance)
        {
            MaxDistance = maxDistance;
            FindPath(start, end);
        }

        public void FindPath(T start, T end)
        {
            Heap<T> openSet = new Heap<T>(start.GetMaxSize());
            Heap<T> closedSet = new Heap<T>(start.GetMaxSize());

            start.distance = 0;
            openSet.Add(start);

            while (openSet.Count > 0)
            {
                T current = openSet.RemoveFirst();

                closedSet.Add(current);

                if (current.Equals(end))
                {
                    PathFound = true;
                    path.Push(current);
                    while (!current.Equals(start))
                    {
                        current = current.parent;
                        path.Push(current);
                    }
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
                        neighbor.distance = current.distance + neighbor.weight;

                        if (neighbor.distance <= MaxDistance)
                        {
                            if (!openSet.Contains(neighbor))
                            {
                                openSet.Add(neighbor);
                                visitedList.Add(neighbor);
                            }
                            else
                            {
                                openSet.UpdateItem(neighbor);

                            }
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

    public interface IAStarItem<T> : IHeapItem<T>, IPathItem<T> where T : IAStarItem<T>
    {
        int GetDistance(T other);
        int F { get; }
        int G { get; set; }
        int H { get; set; }

        int GetMaxSize();
    }
}

