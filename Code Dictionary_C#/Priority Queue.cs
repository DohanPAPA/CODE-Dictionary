using System;
using System.Collections.Generic;
using System.Linq;

namespace Code_Dictionary_C_
{
    #region 2. 우선순위 큐
    // 2. 우선순위 큐--------------------------------------------------------------------------------------------------------------
    public class Priority_Queue
    {

        // 트리 구조에서의 부모인덱스와 자식인덱스 찾기
        // 인덱스는 0번부터 시작함!
        // O(1)
        int leftChild(int index)
        {
            return (index * 2) + 1;
        }
        // O(1)
        int rightChild(int index)
        {
            return (index * 2) + 2;
        }
        // O(1)
        int parent(int index)
        {
            return (index - 1) / 2;
        }

        List<int> Heap = new List<int>();

        public void Enqueue(int data)
        {
            // list에 데이터 추가
            Heap.Add(data);

            // 현재 리스트 크기 확인 탐색
            int now_index = Heap.Count() - 1;

            // Min값으로 정렬
            while (now_index > 0)
            {
                //현재 넣은 데이터 차일드 검색
                int child = now_index;

                //부모 인덱스 저장
                int parant = (child - 1) / 2;

                //값 비교후 더 낮으면 스왑
                if (Heap[child] < Heap[parant])
                {
                    Swap(parant, child);
                    now_index = parant;
                }
                else
                {
                    break;
                }
            }
        }

        public int Dequeue()
        {
            if (Heap.Count == 0)
            {
                throw new ApplicationException("자료가 없습니다");
            }

            //리턴할 데이터 저장
            int data = Heap[0];

            //마지막 데이터를 0에다 저장
            Heap[0] = Heap[Heap.Count() - 1];
            //마지막 인덱스 삭제
            Heap.RemoveAt(Heap.Count() - 1);

            //마지막 데이터를 넣은 root인덱스 저장
            int parant = 0;
            //현재 채워져있는 Count 저장
            int index = Heap.Count() - 1;

            while (parant <= index)
            {
                // 일단 왼쪽 child 인데스 저장
                int child = (parant * 2) + 1;

                // child인덱스가 원래 크기보다 크면 브레이크
                if (child > index)
                {
                    break;
                }

                // 오른쪽 child가 index값이랑 같거나 작고 왼쪽 child보다 작으면 오른쪽 child로 경로 변경
                if (child + 1 <= index && Heap[child] > Heap[child + 1])
                {
                    child++;
                }

                // 현재 child에 저장된 값과 parant값을 비교후 더 작으면 스왑 후 변경된 값과 그 child들과 다시 비교하기 위해
                // parant를 child 값으로 변경
                if (Heap[parant] > Heap[child])
                {
                    Swap(parant, child);
                    parant = child;
                }
                // 더 작으면 종료
                else
                {
                    break;
                }


            }

            // 우선순위 값 반환
            return data;
        }

        public int Peek()
        {
            if (Heap.Count == 0)
            {
                throw new ApplicationException("자료가 없습니다");
            }


            int peekData = Heap[0];

            return peekData;
        }

        public void DebugHeap()
        {
            foreach (int i in Heap)
            {
                Console.Write($"{i} ");
            }
        }


        public void Swap(int parant, int child)
        {
            int temp = Heap[parant];
            Heap[parant] = Heap[child];
            Heap[child] = temp;
        }
    }
    //-----------------------------------------------------------------------------------------------------------------------------
    #endregion
}
