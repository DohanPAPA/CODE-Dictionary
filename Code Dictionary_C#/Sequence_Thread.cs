using System;

namespace Code_Dictionary_C_
{
    public static class Check_TimeOut
    {
        // 시퀀스 구동 체크 타임아웃 시간------------------------------------------------
        //===============================================================================
        public const int SEQ_TIMEOUT_CHECK_5_SEC = 5000;        // 5초
        public const int SEQ_TIMEOUT_CHECK_10_SEC = 10000;      // 10초
        public const int SEQ_TIMEOUT_CHECK_15_SEC = 15000;      // 15초
        public const int SEQ_TIMEOUT_CHECK_20_SEC = 20000;      // 20초
        public const int SEQ_TIMEOUT_CHECK_25_SEC = 25000;      // 25초
        public const int SEQ_TIMEOUT_CHECK_30_SEC = 30000;      // 30초
        public const int SEQ_TIMEOUT_CHECK_35_SEC = 35000;      // 35초
        public const int SEQ_TIMEOUT_CHECK_60_SEC = 60000;      // 1분
        public const int SEQ_TIMEOUT_CHECK_120_SEC = 1200000;   // 2분
        public const int SEQ_TIMEOUT_CHECK_180_SEC = 1800000;   // 3분
        public const int SEQ_TIMEOUT_CHECK_300_SEC = 3000000;   // 5분
        public const int SEQ_TIMEOUT_CHECK_600_SEC = 6000000;   // 10분
        //================================================================================
        //--------------------------------------------------------------------------------

        // 시퀀스 타임체크 및 다음 스텝 이동 관련 함수-------------------------------------
        //=================================================================================
        // 총 사용할 수 있는 시퀀스 개수 선언
        // 100개의 시퀀스를 사용할수있음
        public const int m_SEQ_MAX = 100;
        public static int[] m_SEQ_Number = new int[m_SEQ_MAX];

        // 시퀀스가 다음 스텝으로 넘어갈때의 현재시간을 저장하는 변수
        public static DateTime m_SeqStartTime = new DateTime();
        public static double[] m_SeqCheckMilliSec = new double[m_SEQ_MAX];

        // 해당되는 시퀀스 스텝으로 이동
        public static void SetNextSeq(int _seqNo, int _stepNum, double _setTimeoutSec)
        {
            m_SEQ_Number[_seqNo] = _stepNum; // 다음 단계로 이동 
            m_SeqCheckMilliSec[_seqNo] = _setTimeoutSec;
            m_SeqStartTime = DateTime.Now;
        }

        // 시퀀스 구동 타임아웃 체크
        public static bool CheckTime(int _seqNo)
        {
            //(검사 시간)이 (현재 시간 - 시작 시간) 이상이 되는 경우 타임아웃 true
            double checkTime = (DateTime.Now - m_SeqStartTime).TotalMilliseconds;
            if (checkTime >= m_SeqCheckMilliSec[_seqNo])
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //=================================================================================
        //---------------------------------------------------------------------------------
    }

    // 시퀀스 종류 : 여러개의 시퀀스를 사용할때 사용
    enum eSEQ_Type
    {
        // 예를들어 1번은 모션관련 시퀀스 2번은 보드관련 3번은 타이머관련 시퀀스로 나누어서 사용
        SEQ_Motion = 0,
        SEQ_Board = 1,
        SEQ_Timer = 2
    }

    // 시퀀스에 사용되는 스텝
    // 스텝의 이름을 넣어서 사용하면 디버깅할때 편함
    enum SEQ_Step
    {
        SEQ_Step_1 = 0,
        SEQ_Step_2 = 1,
        SEQ_Step_3 = 2,
        SEQ_Step_4 = 3,
        SEQ_Step_5 = 4,
        SEQ_END_Step = 99
    }

    internal class Sequence_Thread
    {
        // 시퀀스 타입 선택
        public const int SEQ_Type = (int)eSEQ_Type.SEQ_Motion;


        public Sequence_Thread()
        {
            // 시작할 스텝 지정
            Check_TimeOut.m_SEQ_Number[(int)eSEQ_Type.SEQ_Motion] = (int)SEQ_Step.SEQ_Step_1;
        }

        // 시퀀스 동작 함수
        public void SEQ_Run_Step_Control()
        {
            switch (Check_TimeOut.m_SEQ_Number[(int)eSEQ_Type.SEQ_Motion])
            {
                case 0:
                    { Method_1(); }
                    break;
                case 1:
                    { Method_2(); }
                    break;
                case 2:
                    { Method_3(); }
                    break;
                case 3:
                    { Method_4(); }
                    break;
                case 99:
                    {
                        // 시퀀스 종료
                    }
                    break;
            }
        }

        public void Method_1()
        {
            // 실행할 조건에 맞는지 확인 후 실행
            if (true)
            {
                // 다음 시퀀스 구동(시퀀스 번호,시퀀스 스텝의 번호,타임아웃 시간)
                Check_TimeOut.SetNextSeq(SEQ_Type, (int)SEQ_Step.SEQ_Step_2, Check_TimeOut.SEQ_TIMEOUT_CHECK_20_SEC);
            }
            else
            {
                // 조건에 부합하지 않으면 지정된 시간만큼 체크했다가 타임아웃으로 빠짐
                if (Check_TimeOut.CheckTime(SEQ_Type))
                {
                    // 시퀀스 종료
                    Check_TimeOut.SetNextSeq(SEQ_Type, (int)SEQ_Step.SEQ_END_Step, Check_TimeOut.SEQ_TIMEOUT_CHECK_20_SEC);
                }
            }
        }

        public void Method_2()
        {
            // 실행할 조건에 맞는지 확인 후 실행
            if (true)
            {
                // 다음 시퀀스 구동(시퀀스 번호,시퀀스 스텝의 번호,타임아웃 시간)
                Check_TimeOut.SetNextSeq(SEQ_Type, (int)SEQ_Step.SEQ_Step_3, Check_TimeOut.SEQ_TIMEOUT_CHECK_20_SEC);
            }
            else
            {
                // 조건에 부합하지 않으면 지정된 시간만큼 체크했다가 타임아웃으로 빠짐
                if (Check_TimeOut.CheckTime(SEQ_Type))
                {
                    // 시퀀스 종료
                    Check_TimeOut.SetNextSeq(SEQ_Type, (int)SEQ_Step.SEQ_END_Step, Check_TimeOut.SEQ_TIMEOUT_CHECK_20_SEC);
                }
            }
        }

        public void Method_3()
        {
            // 실행할 조건에 맞는지 확인 후 실행
            if (true)
            {
                // 다음 시퀀스 구동(시퀀스 번호,시퀀스 스텝의 번호,타임아웃 시간)
                Check_TimeOut.SetNextSeq(SEQ_Type, (int)SEQ_Step.SEQ_Step_4, Check_TimeOut.SEQ_TIMEOUT_CHECK_20_SEC);
            }
            else
            {
                // 조건에 부합하지 않으면 지정된 시간만큼 체크했다가 타임아웃으로 빠짐
                if (Check_TimeOut.CheckTime(SEQ_Type))
                {
                    // 시퀀스 종료
                    Check_TimeOut.SetNextSeq(SEQ_Type, (int)SEQ_Step.SEQ_END_Step, Check_TimeOut.SEQ_TIMEOUT_CHECK_20_SEC);
                }
            }
        }

        public void Method_4()
        {
            // 실행할 조건에 맞는지 확인 후 실행
            if (true)
            {
                // 다음 시퀀스 구동(시퀀스 번호,시퀀스 스텝의 번호,타임아웃 시간)
                Check_TimeOut.SetNextSeq(SEQ_Type, (int)SEQ_Step.SEQ_Step_5, Check_TimeOut.SEQ_TIMEOUT_CHECK_20_SEC);
            }
            else
            {
                // 조건에 부합하지 않으면 지정된 시간만큼 체크했다가 타임아웃으로 빠짐
                if (Check_TimeOut.CheckTime(SEQ_Type))
                {
                    // 시퀀스 종료
                    Check_TimeOut.SetNextSeq(SEQ_Type, (int)SEQ_Step.SEQ_END_Step, Check_TimeOut.SEQ_TIMEOUT_CHECK_20_SEC);
                }
            }
        }
    }
}
