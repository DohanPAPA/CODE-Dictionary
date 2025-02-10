//====================================================================================================================================================================//
//                                                                                                                                                                    //
//         1.  작성일 : 2024.04.15.월요일 (수정)                                                                                                                      //
//                                                                                                                                                                    //
//         2.  장소   : Avaco s/w개발팀                                                                                                                               //
//                                                                                                                                                                    //
//         3.  작성자 : 김근호                                                                                                                                        //
//                                                                                                                                                                    //
//====================================================================================================================================================================//
// Socket & TcpClient / TcpListener 둘 중 아무거나 사용 가능
// TcpClient가 사용하기 더 쉽고 간편함

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Code_Dictionary_C_
{
    internal class Socket_Client
    {
        //public string IP_ { get; set; }
        //public int PORT_ { get; set; }

        Socket m_Socket = null;
        IPEndPoint m_IpEndPoint = null;

        TcpClient TcpClient_ = null;

        // 비동기 소켓 작업------------------------ 
        // .NET 4.5이상에서 사용가능
        SocketAsyncEventArgs senderArgs; // 송신용
        SocketAsyncEventArgs ReceiveArgs; // 수신용
        SocketAsyncEventArgs ConnectArgs; // 연결
        //-----------------------------------------

        // 리시브 받은 바이트형식의 데이터를 담는 큐
        public Queue<byte[]> m_bRECV_MSG = new Queue<byte[]>();
        public Queue<string> m_sRECV_MSG = new Queue<string>();

        // 리시브받은 바이트 형식의 데이터를 스트링 형식으로 변환하여 담는 변수
        private string m_string_Data = string.Empty;
        private string m_string_Log_Data = string.Empty;
        private bool m_Message_Complet;

        // 프로퍼티------------------------------------------------------------------------------------------
        public bool ONLINE
        {
            get { return IsOnline(); }
        }
        public string string_Data
        {
            get { return m_string_Data; }
            set { m_string_Data = value; }
        }
        public string string_Log_Data
        {
            get { return m_string_Log_Data; }
            set { m_string_Log_Data = value; }
        }
        public bool Message_Complet
        {
            get { return m_Message_Complet; }
            set { m_Message_Complet = value; }
        }
        //----------------------------------------------------------------------------------------------------

        //public Socket_Client(string ip_address, int port_number)
        //{
        //    // 소켓 클라이언트로 실행
        //    Init_Client(ip_address, port_number);
        //}

        // 인터페이스 메서드 재정의-------------------------------------------------------------------------------------------------------------
        public bool Socket_Initialization(string ip_address, int port_number)
        {
            Stack<SocketAsyncEventArgs> m_pool;

            IPAddress IpAddress_ = IPAddress.Parse(ip_address);
            int PortNumber = port_number;

            TcpClient_ = new TcpClient();

            // 소켓 생성
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            m_IpEndPoint = new IPEndPoint(IpAddress_, PortNumber);

            try
            {
                ConnectArgs = new SocketAsyncEventArgs();
                // 소켓 연결
                ConnectArgs.RemoteEndPoint = m_IpEndPoint;

                // 이벤트 생성 (화살표 형태로도 가능)
                // 이벤트 등록 핸들러
                ConnectArgs.Completed += OnConnectCompleted; // -> ConnectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectCompleted);
                //// Socket 정보 저장역할
                //ConnectArgs.UserToken = m_Socket;

                // 소켓 연결
                return Socket_Connect();
            }
            catch (Exception ex)
            {
                //string_Log_Data = ex.ToString();
                return false;
            }
        }

        public bool Socket_Connect()
        {
            // 비동기 연결
            // .NET 4.5이상에서 사용가능
            m_Socket.ConnectAsync(ConnectArgs);

            Thread.Sleep(100);
            if (m_Socket.Connected) { return true; }
            else { return false; }
        }

        private bool IsOnline()
        {
            bool bRet = true;

            if (m_Socket == null) bRet = false;
            else if (m_Socket.Connected == false) bRet = false;

            return bRet;
        }

        public bool Socket_Disconnect()
        {
            if (ONLINE)
            {
                m_IpEndPoint = null;

                // 이벤트 리소스 제거
                ConnectArgs.Completed -= OnConnectCompleted;
                senderArgs.Completed -= OnSendCompleted;
                ReceiveArgs.Completed -= OnRecvCompleted;

                ConnectArgs.SetBuffer(null, 0, 0);
                senderArgs.SetBuffer(null, 0, 0);
                ReceiveArgs.SetBuffer(null, 0, 0);

                ConnectArgs.Dispose();
                senderArgs.Dispose();
                ReceiveArgs.Dispose();

                //// 소켓 재활용 x 소켓을 다시 생성해야함!
                //m_Socket.Shutdown(SocketShutdown.Both);
                //// 소켓 재활용 o 소켓 연결을 닫고 다시 연결할수 있게 해줌
                m_Socket.Disconnect(false);
                m_Socket.Close();

                return true;
            }

            return false;
        }

        // 메시지 보내기
        public bool SendMsg_String(string message)
        {
            object m_Lock_Object_string = new object();

            lock (m_Lock_Object_string)
            {
                if (message.Equals(string.Empty) == false)
                {
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
                    senderArgs.SetBuffer(buffer, 0, buffer.Length);
                    m_Socket.SendAsync(senderArgs);
                    Message_Complet = true;
                }
                else
                { Message_Complet = false; }

                return Message_Complet;
            }
        }

        public bool SendMsg_Byte(byte[] message)
        {
            object m_Lock_Object_Byte = new object();

            lock (m_Lock_Object_Byte)
            {
                if (message.Equals(string.Empty) == false)
                {
                    senderArgs.SetBuffer(message, 0, message.Length);
                    m_Socket.SendAsync(senderArgs);
                    //m_Socket.Send(message);
                    Message_Complet = true;
                }
                else
                { Message_Complet = false; }
            }

            return Message_Complet;
        }
        // -----------------------------------이벤트 함수--------------------------------------------------------------------------------------------
        public void OnConnectCompleted(object obj, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                // 데이터 송신용 SocketAsyncEventArgs 객체
                senderArgs = new SocketAsyncEventArgs();

                senderArgs.Completed += OnSendCompleted; // 유형.1

                // 데이터 수신용 SocketAsyncEventArgs 객체
                ReceiveArgs = new SocketAsyncEventArgs();
                //SetBuffer(비동기 소켓 메서드와 함께 사용될 데이터 버퍼 , 작업이 시작되는 위치, 버퍼에서 보내거나 받을 최대 데이터 양)
                ReceiveArgs.SetBuffer(new byte[1024], 0, 1024);
                ReceiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted); // 유형.2

                // 데이터 수신 준비를 합니다.
                bool pending = m_Socket.ReceiveAsync(ReceiveArgs);
                if (pending == false)
                    OnRecvCompleted(null, ReceiveArgs);
            }
        }

        public void OnSendCompleted(object obj, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                senderArgs.BufferList = null;
            }
        }

        public void OnRecvCompleted(object obj, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                string recvData = System.Text.Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);

                // 버퍼를 직접 큐에 넣으면 후속 작업시 버퍼가 재사용 되거나 수정될 수 있어 주소에 의한 복사가 일어나 Dequeue를 할때 모든 대기열에 같은 데이터가 들어가게 된다. 
                // 그걸 방지하기 위해서는 Enqueue를 하기전 넣을 데이터가 독립적인지 확인을 한 후에 데이터를 삽입한다. 
                // Buffer.BlockCopy(원본배열,원본배열의 복사 시작위치,복사될배열,복사될배열의 시작위치,복사개수) : byte배열에서만 동작함
                // Array.Copy(원본배열,원본배열의 복사 시작위치,복사될배열,복사될배열의 시작위치,복사개수) : 모든 형식에 사용가능
                // 둘중 속도는 비슷함으로 그때그때 상황에 맞게 사용하면 됌
                byte[] dataCopy = new byte[args.BytesTransferred];
                Buffer.BlockCopy(args.Buffer, args.Offset, dataCopy, 0, args.BytesTransferred);

                m_bRECV_MSG.Enqueue(dataCopy); // byte형으로 큐에 담기

                //m_sRECV_MSG.Enqueue(recvData); // string형으로 큐에 담기

                // .. 받은 데이터를 채팅 UI에 표현합니다 ..

                // 새로운 데이터 수신을 준비합니다.
                bool pending = m_Socket.ReceiveAsync(args);
                if (pending == false)
                    OnRecvCompleted(null, args);
            }
        }
    }
}
