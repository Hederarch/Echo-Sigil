using System;

namespace Pathfinding
{
    public class Heap<T> where T : IHeapItem<T>
    {
        T[] items;

        public Heap(int maxHeapSize)
        {
            items = new T[maxHeapSize];
        }

        public void Add(T item)
        {
            item.HeapIndex = Count;
            items[Count] = item;
            SortUp(item);
            Count++;
        }

        public void UpdateItem(T item)
        {
            SortUp(item);
            SortDown(item);
        }

        public T RemoveFirst()
        {
            T firstItem = items[0];
            Count--;
            items[0] = items[Count];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            return firstItem;
        }

        public int Count { get; private set; } = 0;

        public bool Contains(T item)
        {
            return Equals(items[item.HeapIndex], item);
        }


        void SortUp(T item)
        {
            int parentIndex = GetParent(item.HeapIndex);

            while (true)
            {
                T parentItem = items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                {
                    Swap(item, parentItem);
                }
                else
                {
                    break;
                }

                parentIndex = GetParent(item.HeapIndex);
            }
        }

        void SortDown(T item)
        {
            while (true)
            {
                int childIndexLeft = GetChild(item.HeapIndex, false);
                int childIndexRight = GetChild(item.HeapIndex, true);
                int swapIndex = 0;

                if (childIndexLeft < Count)
                {
                    swapIndex = childIndexLeft;

                    if (childIndexRight < Count)
                    {
                        if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (item.CompareTo(items[swapIndex]) < 0)
                    {
                        Swap(item, items[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        void Swap(T itemA, T itemB)
        {
            items[itemA.HeapIndex] = itemB;
            items[itemB.HeapIndex] = itemA;
            int itemAindex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAindex;
        }

        int GetParent(int index)
        {
            return (index - 1) / 2;
        }

        int GetChild(int index, bool right)
        {
            return index * 2 + (right ? 2 : 1);
        }
    }

    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex
        {
            get;
            set;
        }
    } 
}