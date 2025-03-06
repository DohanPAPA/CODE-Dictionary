using System;
using System.Collections.Generic;
using System.Linq;

namespace Code_Dictionary_C_
{
    // A* 알고리즘은 최단 경로 탐색을 위한 대표적인 알고리즘이다. 
    // G(n) : 시작 노드에서 현재 노드까지의 비용
    // H(n) : 현재 노드에서 목표 노드까지의 휴리스틱(예상비용)
    // F(n) = G(n) + H(n) 최적 경로를 찾기 위한 총 비용
    // A* 는 최단 경로를 찾아가는 과정에서 가장 비용이 낮은 노드를 우선 탐색하는 방식

    // 점수 매기기
    // F = G + H
    // F = 최종 점수 (작을 수록 좋음, 경로에 따라 달라짐)
    // G = 시작점에서 해당 좌표까지 이동하는 데 드는 비용 (작을 수록 좋음, 경로에 따라 달라짐)
    // H = 목적지에서 얼마나 가까운지 (작을 수록 좋음, 고정된 값)

    class A_star_Algorithm
    {
        // SLAM에서 실시간으로 맵핑되는 데이터를 2D 배열로 변환해서 전달됨
        static int[,] gridMap =
        {
            // 0 : 이동 가능
            // 1 : 이동 불가
            { 0, 0, 0, 0, 1 },
            { 1, 0, 0, 1, 1 },
            { 0, 0, 0, 0, 0 },
            { 0, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0 }
        };

        public void Start(int[,] SLAM_Map)
        {
            //gridMap = SLAM_Map;

            Tile_node Start_Point = new Tile_node(0, 0);
            Tile_node End_Point = new Tile_node(4, 4);

            List<Tile_node> path = FindPath(Start_Point, End_Point);

            if (path != null)
            {
                Console.WriteLine("최단 경로:");
                foreach (var node in path)
                    Console.WriteLine($"({node.X}, {node.Y})");
            }
            else
            {
                Console.WriteLine("경로를 찾을 수 없습니다.");
            }
        }

        // 휴리스틱값 계산
        // 현재위치에서 목표위치까지의 예상 비용을 계산하는 함수
        // 맨하탄 거리를 계산하는 휴리스틱 함수
        private int Heuristic(Tile_node current_Tile, Tile_node end_Tile)
        {
            int dx = Math.Abs(current_Tile.X - end_Tile.X);
            int dy = Math.Abs(current_Tile.Y - end_Tile.Y);

            // 절대값으로 계산
            return dx + dy;
        }

        public List<Tile_node> FindPath(Tile_node Start_Tile, Tile_node End_Tile)
        {
            // 우선순위 큐를 사용 또는 리스트를 사용하여 오픈 리스트를 관리함
            List<Tile_node> openList = new List<Tile_node> { Start_Tile }; // 지나갈수 있는 길
            //// .NET 7 이상일때 사용할 수 있는 SortedSet 사용하면더 수월함
            //SortedSet<Tile_node> openList_ = new SortedSet<Tile_node>();
            HashSet<Tile_node> closedList = new HashSet<Tile_node>();      // 지나갈수 없는 길



            while (openList.Count > 0)
            {
                // 우선순위 큐를 사용하지 않고 리스트를 사용했을때
                // F 값이 가장 낮은 값을 선택
                Tile_node current = openList.OrderBy(tile => tile.F_cost).First();
                //openList_.First();
                //if (closedList.Contains(current))
                //    continue;

                // 현재 타일을 오픈 리스트에서 삭제
                openList.Remove(current);
                // 현재 타일을 클로우즈 리스트에 추가
                closedList.Add(current);

                // 현재 타일이 도착 타일이라면 경로 역추적 반환
                if (current.X == End_Tile.X && current.Y == End_Tile.Y)
                    return ReconstructPath(current);


                // 4방향으로 다음 부분을 총 4번 호출
                // allowDiagonal : 대각선이 있는지 없는지 설정할때 사용
                foreach (var neighbor in GetNeighbors(current, allowDiagonal: false))
                {
                    // 1. 이미 방문한 타일이 있는 경우 무시
                    if (closedList.Contains(neighbor)) continue;

                    // 2. 현재 타일을 통해 이웃 타일로 가는 비용 G 계산\
                    // 상하좌우 이동비용 1임 -> 가중치가 없는 경우
                    int tentativeG = current.G_cost + 1;
                    // 가중치가 있는 경우 상하좌우 10 / 대각선 14
                    //int moveCost = (neighbor.X != current.X && neighbor.Y != current.Y) ? 10 : 14;
                    //int tentativeG = current.G_cost + moveCost;

                    // 3. 새로운 경로가 기존 경보로다 좋은 경우 갱신
                    if (!openList.Contains(neighbor) ||
                        tentativeG < neighbor.G_cost)
                    {
                        // G_cost 업데이트 (시작점에서 해당 노드까지의 비용)
                        neighbor.G_cost = tentativeG;
                        // H_cost 업데이트 (휴리스틱: 현재 노드에서 목표까지 예상 비용)
                        neighbor.H_cost = Heuristic(neighbor, End_Tile);
                        // Parent 설정 (경로 추적을 위해 이전 노드를 저장)
                        neighbor.Parent = current;

                        // 4. 아직 openList에 없는 경우 추가하여 탐색 대상으로 만듦
                        if (!openList.Contains(neighbor))
                            openList.Add(neighbor);
                    }
                }

            }

            // 경로를 찾지 못했으므로 null 반환
            return null;
        }

        // 경로 역추적 함수
        private List<Tile_node> ReconstructPath(Tile_node currentTile)
        {
            List<Tile_node> path = new List<Tile_node>();

            // 1. 현재 노드(currentTile)부터 부모 노드를 따라가며 리스트에 추가
            while (currentTile != null)
            {
                path.Add(currentTile);            // 현재 노드를 리스트에 추가
                currentTile = currentTile.Parent; // 부모 노드로 이동
            }

            // 2. 리스트를 뒤집어서 올바른 순서(출발 → 도착)로 변경
            path.Reverse();

            // 3. 최종 경로 반환
            return path;
        }

        private List<Tile_node> GetNeighbors(Tile_node currentTile, bool allowDiagonal = false)
        {
            List<Tile_node> neighbors = new List<Tile_node>();
            // 상하좌우 4방향만
            int[] direction_X = { 0, 0, -1, 1 }; // X축 : 상(0) , 하(0) , 좌(-1) , 우(1)
            int[] direction_Y = { -1, 1, 0, 0 }; // Y축 : 상(-1), 하(1) , 좌(0)  , 우(0)
            //// 상하좌우 + 대각선 이동을 위한 방향 배열
            //int[] direction_X = { 0, 0, -1, 1, -1, -1, 1, 1 };
            //int[] direction_Y = { -1, 1, 0, 0, -1, 1, -1, 1 };
            //int directionCount = allowDiagonal ? 8 : 4; // 8방향(대각선 포함) 또는 4방향(기본)

            for (int i = 0; i < 4; i++)
            {
                int newX = currentTile.X + direction_X[i]; // 새로운 X 좌표
                int newY = currentTile.Y + direction_Y[i]; // 새로운 Y 좌표


                // 새로운 좌표가 유효한지 검사 (맵 범위 내 & 장애물이 없는 경우)
                if (newX >= 0 && newX < gridMap.GetLength(0) &&     // 새로운 X좌표가 0보다 크고 맵의 행 개수가 새로운 X좌표보다 큰지 확인
                    newY >= 0 && newY < gridMap.GetLength(1) &&     // 새로운 Y좌표가 0보다 크고 맵의 열 개수가 새로운 Y좌표보다 큰지 확인
                    gridMap[newX, newY] == 0)                       // map[newX,newY] == 0 : 0이면 이동 가능, 1이면 이동 불가(벽) // ture,false 형식일때는 수정 필요
                    neighbors.Add(new Tile_node(newX, newY));   // 유효한 경우 이웃 리스트에 추가
            }

            return neighbors;
        }



    }

    class Tile_node
    {
        public int X { get; }
        public int Y { get; }
        public int G_cost { get; set; }  // 시작점부터 현재 노드까지 비용
        public int H_cost { get; set; }  // 휴리스틱(목표까지 예상 비용)
        public int F_cost => G_cost + H_cost;      // 총 비용 (F = G + H)
        public Tile_node Parent { get; set; }  // 경로 추적을 위한 부모 노드

        public Tile_node(int x, int y)
        {
            X = x;
            Y = y;
        }

    }


}
