using System;
using System.Collections.Generic;

namespace Code_Dictionary_C_
{
    // 다익스트라 알고리즘은 그래프에서 최단 경로를 찾기 위해 사용되는 알고리즘
    // 특정 출발점부터 다른 정점까지의 최단 경로를 구하는 문제를 해결하는 데에 적용됨
    // 다익스트라 알고리즘은 각 정점까지의 최단 거리를 점진적으로 계산하며, 그리디 알고리즘의 한 형태이다. 

    // 활용 분야
    // 길 찾기 어플리케이션 : 지도 서비스나 네비게이션 시스템에서 출발지와 목적지 사이의 최단경로
    // 네트워크 라우팅 : 패킷 스위칭 네트워크에서 데이터 패킷을 최적 경로를 결정할 때
    // 자원 할당 : 자원을 효율적으로 할당하기 위해 다익스트라 알고리즘 사용
    // DNA 시퀀싱 : 유전자 간의 거리를 계산할 때

    enum eNode
    {
        A, B, C, D, E, F
    }

    internal class Dijkstra_Algorithm
    {
        Graph graph = new Graph(6);

        public Dijkstra_Algorithm()
        {
            graph.AddEdge((int)eNode.A, (int)eNode.B, 10);
            graph.AddEdge((int)eNode.A, (int)eNode.D, 15);
            graph.AddEdge((int)eNode.A, (int)eNode.C, 30);

            graph.AddEdge((int)eNode.B, (int)eNode.E, 20);

            graph.AddEdge((int)eNode.C, (int)eNode.F, 5);

            graph.AddEdge((int)eNode.D, (int)eNode.C, 5);
            graph.AddEdge((int)eNode.D, (int)eNode.F, 20);

            graph.AddEdge((int)eNode.E, (int)eNode.F, 20);

            graph.AddEdge((int)eNode.F, (int)eNode.D, 20);

            List<int> shortestPath = graph.Dijkstra((int)eNode.A, 5);

            foreach (int node in shortestPath)
            {
                Console.Write("{0} ", (eNode)node);
            }

        }
    }

    class Graph
    {
        private int[,] adj;
        private int size;
        List<int> path = new List<int>();

        public Graph(int size)
        {
            this.size = size;
            this.adj = new int[this.size, this.size];
        }

        // 각 노드간의 길이값 넣기
        public void AddEdge(int a, int b, int dist)
        {
            this.adj[a, b] = dist;
        }

        public List<int> Dijkstra(int start, int dest)
        {
            bool[] visited = new bool[this.size];
            int[] distance = new int[this.size];
            int[] parent = new int[this.size];

            for (int i = 0; i < distance.Length; i++)
            {
                distance[i] = Int32.MaxValue;
            }

            distance[start] = 0;
            parent[start] = start;

            while (true)
            {
                int now = -1;
                int closest = Int32.MaxValue;
                for (int i = 0; i < this.size; i++)
                {
                    if (visited[i]) continue;

                    if (distance[i] == Int32.MaxValue) continue;

                    if (distance[i] < closest)
                    {
                        closest = distance[i];
                        now = i;
                    }
                }

                if (now == -1) break;

                visited[now] = true;

                for (int next = 0; next < this.size; next++)
                {
                    if (this.adj[now, next] == 0) continue;

                    if (visited[next]) continue;

                    int nextDist = distance[now] + this.adj[now, next];

                    if (nextDist < distance[next])
                    {
                        distance[next] = nextDist;
                        parent[next] = now;
                    }
                }
            }

            return this.CalcPathFromParent(parent, dest);
        }

        private List<int> CalcPathFromParent(int[] parent, int dest)
        {
            Console.WriteLine("{0}까지 최단 경로 : ", dest);
            while (parent[dest] != dest)
            {
                this.path.Add(dest);
                dest = parent[dest];
            }

            this.path.Add(dest);
            this.path.Reverse();

            return this.path;
        }
    }


}
