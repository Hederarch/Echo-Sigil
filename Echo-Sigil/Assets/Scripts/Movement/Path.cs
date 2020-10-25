using System;
using System.Collections.Generic;

namespace Pathfinding
{
    public static class PathRequestManager<T> where T : IPathItem<T>
    {
        static Queue<PathRequest<T>> pathRequestQueue = new Queue<PathRequest<T>>();
        static PathRequest<T> currentPathRequest;
        private static bool isProcessingPath;

        public static void RequestPath(IPathFinder<T> pathFinder, T start, T end, Action<Path<T>> callback)
        {
            PathRequest<T> newRequest = new PathRequest<T>(pathFinder, start, end, callback);
            pathRequestQueue.Enqueue(newRequest);
            TryProcessNext();
        }

        struct PathRequest<intT> where intT : IPathItem<intT>
        {
            public IPathFinder<intT> pathFinder;
            public intT start;
            public intT end;
            public Action<Path<intT>> callback;

            public PathRequest(IPathFinder<intT> _pathFinder, intT _start, intT _end, Action<Path<intT>> _callback)
            {
                pathFinder = _pathFinder;
                start = _start;
                end = _end;
                callback = _callback;
            }

        }

        static void TryProcessNext()
        {
            if (!isProcessingPath && pathRequestQueue.Count > 0)
            {
                currentPathRequest = pathRequestQueue.Dequeue();
                isProcessingPath = true;
                FinishedProcessingPath(currentPathRequest.pathFinder.FindPath(currentPathRequest.start, currentPathRequest.end));
            }
        }

        public static void FinishedProcessingPath(Path<T> path)
        {
            isProcessingPath = false;
            currentPathRequest.callback(path);
            TryProcessNext();
        }
    }

    public struct Path<T> where T : IPathItem<T>
    {
        public bool sucsess;
        public T[] itemsContained;
        public Path(T end, T start)
        {
            List<T> itemList = new List<T>();
            T current = end;

            while (!current.Equals(start))
            {
                itemList.Add(current);
                current = current.parent;
            }
            itemList.Reverse();
            itemsContained = itemList.ToArray();
            sucsess = true;
        }

        public Path(bool _sucsess = false)
        {
            sucsess = _sucsess;
            itemsContained = new T[0];
        }
    }

    public interface IPathFinder<T> where T : IPathItem<T>
    {
        Path<T> FindPath(T start, T end);
    }

    public interface IPathFollower<T> where T : IPathItem<T>
    {
        void OnPathFound(Path<T> newPath);
    }

    public interface IPathItem<T> where T : IPathItem<T>
    {
        T parent { get; set; }
        T[] FindNeighbors();
        int weight { get; }
        bool walkable { get; }
    } 
}
