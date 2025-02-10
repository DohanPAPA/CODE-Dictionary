using System;

namespace Code_Dictionary_C_
{
    public class Forms_Delay
    {
        // Form 관련 함수
        //// UI 스레드는 정지되지않고 지연시키는 함수-------------
        ///
        //================================================================================================================================
        // ※ Application.DoEvents() 사용 시 주의사항
        // DoEvents를 남용하면 코드의 복잡성을 증가시키고, 예기치 않은 동작을 유발할 수 있다. 대신 async/await 패턴을 사용하는 것이 좋다.
        // UI가 여러 번 업데이트되는 과정에서 상태 관리가 어려울 수 있다.
        // DoEvents는 간단한 상황에서는 유용하지만, 복잡한 작업에는 비동기 프로그래밍이 더 나은 선택임
        //================================================================================================================================
        public static void Delay(int ms)
        {
            DateTime dateTimeNow = DateTime.Now; // 현재시간

            TimeSpan duration = new TimeSpan(0, 0, 0, 0, ms); // 대기시간
            DateTime dateTimeAdd = dateTimeNow.Add(duration); // 현재시간 + 대기시간

            while (dateTimeAdd >= dateTimeNow)
            {
                //System.Windows.Forms.Application.DoEvents();// UI를 정지시키지않고 작업을 처리할수 있도록 해줌
                dateTimeNow = DateTime.Now;
            }
            return;
        }
        //------------------------------------------------------
    }
}
