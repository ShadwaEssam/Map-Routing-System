using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace MapProject
{
    public class Operations
    {
        public static Dictionary<int, List<Edge>> Edges = new Dictionary<int, List<Edge>>();
        public static Dictionary<int, Node> Nodes = new Dictionary<int, Node>();
        public static Query CurrentQuery;
        public const double WalkingSpeed = 5.0;
        public static Stopwatch TotalExecution = new Stopwatch();
        public static Stopwatch Execution = new Stopwatch();
        public static FileStream fileStream;
        public static StreamWriter fileWriter;
        public static int SourceID;
        public static int DestinationID;


        public static double Euclidean(double x1, double y1, double x2, double y2)
        {
            double result = Math.Sqrt((((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2))));
            return result;
        }

        public static void ReadInput()
        {
            FileStream file = File.Open("SFMap.txt", FileMode.Open);

            StreamReader stream = new StreamReader(file);

            string line = stream.ReadLine(); // awl w7d hyt2ry bara (no.of data)
            int NumOfVertices = Int32.Parse(line);

            for (int i = 0; i < NumOfVertices; i++)
            {
                line = stream.ReadLine();
                string[] tokens = line.Split(' ');

                Edges.Add(Int32.Parse(tokens[0]), new List<Edge>()); // id , new dic da l neighbors l lsa m2rtosh
                Nodes.Add(Int32.Parse(tokens[0]), new Node(Int32.Parse(tokens[0]), Double.Parse(tokens[1]), Double.Parse(tokens[2]))); // 0 id, 1 x, 2 y .... s
            }

            line = stream.ReadLine();
            int NumOfConnections = Int32.Parse(line);

            for (int i = 0; i < NumOfConnections; i++)
            {
                line = stream.ReadLine();
                string[] tokens = line.Split(' ');

                Edges[Int32.Parse(tokens[0])].Add(new Edge(Double.Parse(tokens[3]), Double.Parse(tokens[2]), Int32.Parse(tokens[1]))); // id1,id2, dis, speed
                Edges[Int32.Parse(tokens[1])].Add(new Edge(Double.Parse(tokens[3]), Double.Parse(tokens[2]), Int32.Parse(tokens[0])));
            }

            stream.Close();

            SourceID = Nodes.Count;   // index l id mn 0, f l b3d l mwgod count , l2n akher l ids count-1
            DestinationID = Nodes.Count + 1;
        }

        public static void ExecuteQuery(string queryName)
        {
            string OutputFileName = queryName.Substring(0, queryName.Length - 4) + "Output.txt";

            if (File.Exists(OutputFileName))
            {
                File.Delete(OutputFileName);
            }

            fileStream = File.Open(OutputFileName, FileMode.Append);
            fileWriter = new StreamWriter(fileStream);



            FileStream QueryFile = File.Open(queryName, FileMode.Open);
            StreamReader stream = new StreamReader(QueryFile);

            string line = stream.ReadLine();
            int NumOfTestCases = Int32.Parse(line);

            Execution.Reset(); // start sw for all queries
            for (int t = 0; t < NumOfTestCases; t++)
            {

                line = stream.ReadLine();
                string[] tokens = line.Split(' '); 

                Nodes.Add(SourceID, new Node(Nodes.Count, Double.Parse(tokens[0]), Double.Parse(tokens[1])));  // puting virtual nodes

                Nodes.Add(DestinationID, new Node(Nodes.Count, Double.Parse(tokens[2]), Double.Parse(tokens[3])));  //puting vn
                Edges.Add(SourceID, new List<Edge>());
                Edges.Add(DestinationID, new List<Edge>());

                CurrentQuery = new Query(Double.Parse(tokens[0]), Double.Parse(tokens[1]), Double.Parse(tokens[2]), Double.Parse(tokens[3]), Double.Parse(tokens[4]) / 1000); // sx, sy, dx, dy, r

                for (int i = 0; i < Nodes.Count; i++)
                {
                    double SourceDistance = Euclidean(CurrentQuery.SourceX, CurrentQuery.SourceY, Nodes[i].x, Nodes[i].y);
                    double DestinationDistance = Euclidean(CurrentQuery.DestinationX, CurrentQuery.DestinationY, Nodes[i].x, Nodes[i].y);

                    if (SourceDistance <= CurrentQuery.Radius) //da l sa7 Nodes.ElementAt(i) .. l 7ta l abl dijksrta
                    {
                        //CurrentQuery.StartingNodes.Add(Nodes[i]);
                        Edges[SourceID].Add(new Edge(WalkingSpeed, SourceDistance, Nodes[i].ID)); //puting neighbors to virtual starting node
                    }
                    ///////////////////////
                    if (DestinationDistance <= CurrentQuery.Radius) //da l sa7 Nodes.ElementAt(i)
                    {
                        CurrentQuery.FinishingNodes.Add(Nodes[i]);
                        Edges[Nodes[i].ID].Add(new Edge(WalkingSpeed, DestinationDistance, DestinationID));   //endnode is neighbour to finishingnodes

                    }
                }
            

                    FindShortestTime();

                    Nodes.Remove(SourceID);
                    Edges.Remove(SourceID);

                    for (int i = 0; i < CurrentQuery.FinishingNodes.Count; i++)
                    {
                        Edges[CurrentQuery.FinishingNodes[i].ID].RemoveAt(Edges[CurrentQuery.FinishingNodes[i].ID].Count - 1); // korasa exmple
                    }

                    Nodes.Remove(DestinationID); // safe to remove
                    Edges.Remove(DestinationID);
                }

                stream.Close();

                fileWriter.WriteLine(Execution.ElapsedMilliseconds.ToString("0.00") + " ms");

            }

            public static List<SortedSetNode> ExtractSolution(SortedSetNode[] results)
            {
                List<SortedSetNode> Route = new List<SortedSetNode>();


                SortedSetNode CurrentNode = results[DestinationID]; // the virtual end h7otha fl currentt to dsply 
                Stack<SortedSetNode> ReversedRoute = new Stack<SortedSetNode>();
                ReversedRoute.Push(CurrentNode);

                while (CurrentNode.Parent != null)
                {
                    CurrentNode = results[CurrentNode.Parent.ID]; // basawy nfsy bl ably 
                    ReversedRoute.Push(CurrentNode);
                }

                while (ReversedRoute.Count > 0) // bamshy 3l stack 
                {
                    Route.Add(ReversedRoute.Pop());
                }

                return Route;
            }

            public static void FindShortestTime()
            {
                Execution.Start(); // start sw for all queries
                double bestTotalTime = Int32.MaxValue;
                double bestTotalDistace = Int32.MaxValue;
                double VehicleDistance = -1;
                double walkingDistance = -1;

                SortedSetNode[] results = Dijkstra(Nodes[SourceID]); // sorted set node array da l byrg3 mn dijkstra

                List<SortedSetNode> Route = ExtractSolution(results);

                double sourceToStartingNodeDistance = Route[1].distance;
                double sourceToStartingNodeTime = sourceToStartingNodeDistance / WalkingSpeed;

                double startingNodeToFinishingNodeTime = Route[Route.Count - 2].weight - Route[1].weight;
                double startingNodeToFinishingNodeDistance = Route[Route.Count - 2].distance - Route[1].distance;

                double finishingNodeToDestinationDistance = Route[Route.Count - 1].distance - Route[Route.Count - 2].distance;
                double finishingNodeToDestinationTime = Route[Route.Count - 1].weight - Route[Route.Count - 2].weight;

                bestTotalTime = sourceToStartingNodeTime + startingNodeToFinishingNodeTime + finishingNodeToDestinationTime;

                bestTotalDistace = sourceToStartingNodeDistance + startingNodeToFinishingNodeDistance + finishingNodeToDestinationDistance;
                walkingDistance = sourceToStartingNodeDistance + finishingNodeToDestinationDistance;
                VehicleDistance = startingNodeToFinishingNodeDistance;

                Execution.Stop();
                for (int i = 1; i < Route.Count - 2; i++) //msh hy3dy 3la akher w7da l end
                {
                    fileWriter.Write(Route[i].ID + " ");
                }


                fileWriter.WriteLine(Route[Route.Count - 2].ID);
                fileWriter.WriteLine((bestTotalTime * 60).ToString("0.00") + " mins"); // rounding display
                fileWriter.WriteLine(bestTotalDistace.ToString("0.00") + " km");
                fileWriter.WriteLine(walkingDistance.ToString("0.00") + " km");
                fileWriter.WriteLine(VehicleDistance.ToString("0.00") + " km");

                fileWriter.WriteLine();
            }

            public static SortedSetNode[] Dijkstra(Node CurrentNode) // 3 thngs nedded
            {
                SortedSetNode[] results = new SortedSetNode[Nodes.Count]; //1.array of obj from class ssn
                SortedSet<SortedSetNode> NextLowest = new SortedSet<SortedSetNode>(); // forimg ssn frm class
                bool[] visited = new bool[Nodes.Count];
                

                NextLowest.Add(new SortedSetNode(CurrentNode.ID, 0, 0, null));

                results[CurrentNode.ID] = new SortedSetNode(CurrentNode.ID, 0, 0, null); //id,c,d,p

                int NumberOfNodes = Nodes.Count;

                while (NumberOfNodes != 0)   /////////////////////////////////////////////
                {
                    CurrentNode = Nodes[NextLowest.First().ID]; // hshof el 3ndo  asghr w aro7 l node-o
                    NumberOfNodes--;

                    if (CurrentNode.ID == DestinationID)
                    {
                        return results;
                    }


                    NextLowest.Remove(NextLowest.First());
                    visited[CurrentNode.ID] = true; // hashel l node mn l visited dic 

                    int EdgeCount = Edges[CurrentNode.ID].Count;

                    for (int i = 0; i < EdgeCount; i++)       // looping on all neighbours
                    {
                        if (!visited[Edges[CurrentNode.ID][i].NeighbourID])
                        {
                            double CurrentNodeTime = results[CurrentNode.ID].weight;                                 // node 2 time
                            double CurrentNodeDistance = results[CurrentNode.ID].distance;

                            int NeighbourID = Edges[CurrentNode.ID][i].NeighbourID;

                            double NeighbourEdgeTime = Edges[CurrentNode.ID][i].time;                // edge between
                            double NeighbourEdgeDistance = Edges[CurrentNode.ID][i].distance;

                            double NeighbourNodeTime;

                            if (results[NeighbourID] == null)
                            {
                                NeighbourNodeTime = Int32.MaxValue;
                            }
                            else
                            {
                                NeighbourNodeTime = results[NeighbourID].weight;
                            }

                            if (NeighbourEdgeTime + CurrentNodeTime < NeighbourNodeTime)
                            {
                                SortedSetNode NewNode = new SortedSetNode(NeighbourID, NeighbourEdgeTime + CurrentNodeTime, NeighbourEdgeDistance + CurrentNodeDistance, Nodes[CurrentNode.ID]);  // updating the shortest path results for node 3
                                SortedSetNode OldNode = results[NeighbourID];
                                results[NeighbourID] = NewNode;

                                NextLowest.Remove(OldNode);
                                NextLowest.Add(NewNode);
                            }
                        }
                    }
                }

                return results;
            }





            public static void Run()
            {
                TotalExecution.Restart();

                ReadInput();
                ExecuteQuery("SFQueries.txt");

                TotalExecution.Stop();
                fileWriter.WriteLine();
                fileWriter.WriteLine(TotalExecution.ElapsedMilliseconds.ToString("0.00") + " ms ");
                fileWriter.Close();
            }
    

        }
    }



    
    
