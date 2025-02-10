namespace Code_Dictionary_C_
{
    #region  1. 원형 큐
    // 1. 원형 큐-----------------------------------------------------------------------------------------------------
    internal class Circle_Queue<T>
    {
        private int Size_ = 1000;
        private int front = 0;
        private int rear = 0;

        // 형식은 자유롭게 바뀜
        public T[] Queue_Data { get; set; }

        public Circle_Queue()
        {
            Queue_Data = new T[Size_];
        }

        public void Enqueue(T Value)
        {
            rear = (rear + 1) % Size_;
            Queue_Data[rear] = Value;
        }

        public T Dequeue()
        {
            front = (front + 1) % Size_;
            return Queue_Data[front];
        }

        public bool IsFull()
        {
            if ((rear + 1) % Size_ == front % Size_) { return true; }
            else { return false; }
        }
        public bool IsEmpty()
        {
            if (front == rear) { return true; }
            else { return false; }
        }

        public void IsClear()
        {
            while (true)
            {
                if (!IsEmpty())
                {
                    Dequeue();
                    return;
                }
                break;
            }
        }
    }
    //-----------------------------------------------------------------------------------------------------------------------------
    #endregion
}
