namespace Code_Dictionary_C_
{
    public enum Step
    {
        step1 = 0,
        step2 = 1,
        step3 = 2,
        step4 = 3,
        step5
    }
    // 장비 or 설비 제어 시 필요한 시퀀스 구현 방법
    internal class Sequence_Timer
    {
        // 콜백용 타이머 생성
        System.Threading.Timer m_Timer;
        int mn_eventID;

        public Sequence_Timer()
        {
            // 타이머 콜백 메서드 등록!!
            m_Timer = new System.Threading.Timer(Recipe_Run_Test_Timer);

            // 시퀀스 타이머 시작
            mn_eventID = (int)Step.step1; // 시작하고자 하는 스텝 선택
            // Change(스탭번호, 시간간격)
            m_Timer.Change(mn_eventID, 5000);
        }
        private void Recipe_Run_Test_Timer(object state)
        {
            switch (mn_eventID)
            {
                case (int)Step.step1:
                    {
                        mn_eventID = (int)Step.step2;
                    }
                    break;
                case (int)Step.step2:
                    {
                        mn_eventID = (int)Step.step3;
                    }
                    break;
                case (int)Step.step3:
                    {
                        mn_eventID = (int)Step.step4;
                    }
                    break;
                case (int)Step.step4:
                    {
                        mn_eventID = (int)Step.step5;
                    }
                    break;
                case (int)Step.step5:
                    {
                        // 타이머 중지
                        m_Timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    }
                    break;
            }
        }

        //public bool TimeCheck()
        //{
        //    //(검사 시간)이 (현재 시간 - 시작 시간) 이상이 되는 경우 타임아웃 true
        //    double checkTime = (DateTime.Now - m_SeqStartTime).TotalMilliseconds;
        //    if (checkTime >= m_SeqCheckMilliSec[_seqNo])
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
    }
}
