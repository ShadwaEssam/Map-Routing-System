using System;
namespace MapProject
{
    public class Edge
    {
        public double velocity;
        public double distance;
        public double time;
        public int NeighbourID;


        public Edge (double velocity, double distance, int NeighbourID)
        {
            this.velocity = velocity;
            this.distance = distance;
            this.time = distance / velocity;
            this.NeighbourID = NeighbourID;
        }
    }


}
