using System;
using System.Collections;
using System.Collections.Generic;

namespace MapProject
{
    public class SortedSetNode : IComparable<SortedSetNode>, IEqualityComparer<SortedSetNode>
    {
        public int ID;
        public double weight;
        public double distance;
        public Node Parent;
        
        public SortedSetNode(int ID, double weight, double distance, Node Parent)
        {
            this.ID = ID;
            this.weight = weight;
            this.distance = distance;
            this.Parent = Parent;
        }

        public int CompareTo(SortedSetNode other)
        {
            if (this.ID == other.ID)
            {
                return 0;
            }
            else if (this.weight > other.weight)
            {
                return 1;
            }
            return -1;
        }

        public bool Equals(SortedSetNode x, SortedSetNode y)
        {
            return x.ID == y.ID;
        }

        public int GetHashCode(SortedSetNode obj)
        {
            return obj.ID;
        }
    }
}
