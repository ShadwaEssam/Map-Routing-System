using System;
using System.Collections.Generic;

namespace MapProject
{
    public class Query
    {
        public double SourceX;
        public double SourceY;
        public double DestinationX;
        public double DestinationY;
        public double Radius;
        public List<Node> StartingNodes;
        public List<Node> FinishingNodes;
        public bool[] needed;
       
        public Query(double SourceX, double SourceY, double DestinationX, double DestinationY, double Radius)
        {
            this.SourceX = SourceX;
            this.SourceY = SourceY;
            this.DestinationX = DestinationX;
            this.DestinationY = DestinationY;
            this.Radius = Radius;
            StartingNodes = new List<Node>();
            FinishingNodes = new List<Node>();

        }
    }
}
