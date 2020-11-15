using System;
using System.Collections.Generic;

namespace Pathfinding
{
    public interface IPath<T> where T : IPathItem<T>
    {
        void FindPath(T start, T end);
        int MaxDistance { set; }
        IEnumerator<T> GetConsideredEnum();
        IEnumerator<T> GetPathEnum();
        T LastPathElement { get; }
        bool PathFound { get; }
    }

    public interface IPathItem<T> where T : IPathItem<T>
    {
        T parent { get; set; }
        T[] FindNeighbors();
        int weight { get; }
        bool walkable { get; }
        int distance { get; set; }
    } 
}
