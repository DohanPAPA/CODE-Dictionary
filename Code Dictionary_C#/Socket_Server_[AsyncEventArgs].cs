//====================================================================================================================================================================//
//                                                                                                                                                                    //
//         1.  작성일 : 2024.04.15.월요일 (수정)                                                                                                                      //
//                                                                                                                                                                    //
//         2.  장소   : Avaco s/w개발팀                                                                                                                                         //
//                                                                                                                                                                    //
//         3.  작성자 : 김근호                                                                                                                                        //
//                                                                                                                                                                    //
//====================================================================================================================================================================//
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Code_Dictionary_C_
{
    public class Socket_Server
    {
        //public string IP_ { get; set; }
        //public int PORT_ { get; set; }

        Socket m_Socket = null;
        IPEndPoint m_IpEndPoint = null;

        SocketAsyncEventArgs acceptArgs;
        SocketAsyncEventArgs recvArgs;

        // 리시브 받은 바이트형식의 데이터를 담는 큐
        public Queue<byte[]> m_bRECV_MSG = new Queue<byte[]>();
        public Queue<string> m_sRECV_MSG = new Queue<string>();

        // 리시브받은 바이트 형식의 데이터를 스트링 형식으로 변환하여 담는 변수
        private string m_string_Data = string.Empty;
        private string m_string_Log_Data = string.Empty;
        private bool m_Message_Complet;

        // 클라이언트 관리
        private static ConcurrentDictionary<int, Socket> _clients = new ConcurrentDictionary<int, Socket>();
        // 고유 클라이언트 ID 카운터
        private static int _clientIdCounter = 0;

        // 프로퍼티---------------------------------------------------------------------
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
        //------------------------------------------------------------------------------
        //public Socket_Server(string ip_address, int port_number)
        //{
        //    Socket_Initialization(ip_address, port_number);
        //}

        // 재정의------------------------------------------------------------------------------------------------------------------------
        private bool IsOnline()
        {
            bool bRet = true;

            if (m_Socket == null) bRet = false;
            else if (m_Socket.Connected == false) bRet = false;
            //else if (m_status_Connected == false) bRet = false;

            return bRet;
        }

        public bool Socket_Initialization(string ip_address, int port_number)
        {
            IPAddress IpAddress_ = IPAddress.Parse(ip_address);
            int PortNumber = port_number;

            // 소켓 생성
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // IPEndPoint 생성
            m_IpEndPoint = new IPEndPoint(IpAddress_, port_number);

            //// Callback 생성
            //m_AcceptCallback = new AsyncCallback(accept_Callback);        // 클라이언트 접속 대기 콜백 
            //m_Server_ReceiveCallback = new AsyncCallback(Server_DataReceive_Callback);   // 데이터 리시브 대기 콜백

            //Task fafa = Task.Run(async () =>
            //{
            //    while (true) // 클라이언트 접속 대기
            //    {
            //        m_Socket.BeginAccept(m_AcceptCallback, m_Socket); // 3
            //        await Task.Delay(10);
            //    }
            //});

            try
            {
                m_Socket.Bind(m_IpEndPoint); // 1
                m_Socket.Listen(10);         // 2 동시접속 10명까지 가능


                //while (true)
                //{
                //    var clientSocket = await Task.Factory.FromAsync(m_Socket.BeginAccept, m_Socket.EndAccept, null);
                //    _clients.Add(clientSocket);
                //    Console.WriteLine("Client connected.");

                //    _ = Task.Run(() => HandleClient(clientSocket));

                //    return true;
                //}


                acceptArgs = new SocketAsyncEventArgs();
                acceptArgs.Completed += OnAcceptCompleted;

                RegisterAccept(acceptArgs);

                return true;


            }
            catch (Exception ex)
            {
                string_Log_Data = ex.ToString();
                return false;
            }
        }

        //private async Task HandleClient(Socket clientSocket)
        //{
        //    var buffer = new byte[1024];
        //    while (true)
        //    {
        //        try
        //        {
        //            int bytesRead = await Task.Factory.FromAsync(
        //                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, null, null),
        //                clientSocket.EndReceive);

        //            if (bytesRead <= 0)
        //            {
        //                // Client disconnected
        //                _clients.TryTake(out _);
        //                clientSocket.Close();
        //                Console.WriteLine("Client disconnected.");
        //                break;
        //            }

        //            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        //            Console.WriteLine($"Received: {receivedMessage}");

        //            // Process the received message (if needed)
        //        }
        //        catch (SocketException)
        //        {
        //            _clients.TryTake(out _);
        //            clientSocket.Close();
        //            Console.WriteLine("Client disconnected due to an error.");
        //            break;
        //        }
        //    }
        //}

        // 사용 안함
        public bool Socket_Connect()
        {
            return true;
        }

        public void RegisterSend()
        {
            //byte[] buff = sendQueue.Dequeue();
            //
            //// 연결된 모든 클라이언트들에게 데이터를 보냅니다.
            //// 역시 데이터 송신용 SocketAsyncEventArgs 객체를 만들어 담은 후
            //// 소켓 클래스의 SendAsync() 함수에 인수로 담아 전달합니다.
            //for (int i = 0; i < clients.Count; ++i)
            //{
            //    SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
            //    sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            //    sendArgs.SetBuffer(buff, 0, buff.Length);
            //
            //
            //    bool pending = clients[i].SendAsync(sendArgs);
            //
            //    if (pending == false)
            //    {
            //        OnSendCompleted(null, sendArgs);
            //    }
            //}
        }

        public bool Socket_Disconnect()
        {
            //for (int i = 0; i < clients.Count; ++i)
            //{
            //    clients[i].Shutdown(SocketShutdown.Both);
            //    clients[i].Close();
            //}
            if (ONLINE)
            {
                if (m_Socket != null)
                {
                    m_Socket = null;
                    m_Socket.Close();

                    // 이벤트 해제
                    acceptArgs.Completed -= OnAcceptCompleted;
                    recvArgs.Completed -= OnRecvCompleted;

                    acceptArgs.SetBuffer(null, 0, 0);
                    recvArgs.SetBuffer(null, 0, 0);

                    acceptArgs.Dispose();
                    recvArgs.Dispose();
                }
                return true;
            }

            return false;

        }

        // CallBack Method-----------------------------------------------------------------------------------------------------
        void RegisterRecv(SocketAsyncEventArgs args)
        {
            Socket client = args.UserToken as Socket;
            bool pending = client.ReceiveAsync(args);
            if (pending == false)
            {
                OnRecvCompleted(null, args);
            }
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                //if (sendQueue.Count > 0)
                //{
                //}
            }
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                // 데이터 수신용 SocketAsyncEventArgs 객체
                recvArgs = new SocketAsyncEventArgs();
                recvArgs.SetBuffer(new byte[1024], 0, 1024);
                recvArgs.Completed += OnRecvCompleted;
                recvArgs.UserToken = args.AcceptSocket;
                recvArgs.AcceptSocket = args.AcceptSocket;

                // 고유한 클라이언트 ID 생성 및 추가
                // 여러 스레드가 동시에 접근하여 값을 증가시킬때 데이터의무결성을 보장하기 위해 사용함
                int clientId = System.Threading.Interlocked.Increment(ref _clientIdCounter);
                _clients.TryAdd(clientId, args.AcceptSocket);

                //_ = Task.Run(() => HandleClient(args.AcceptSocket));
                int a = _clients.Count;

                // 데이터 수신을 기다립니다.
                // 데이터 수신용 SocketAsyncEventArgs 객체가 인수로 넘어갑니다.
                RegisterRecv(recvArgs);
            }
            else
            {
                //print(args.SocketError.ToString());
            }

            // 새로운 승인을 기다립니다.
            // 승인작업을 가지고 온 SocketAsyncEventArgs 객체가 작업을 마치고
            // 다시 승인작업을 하러 넘어갑니다.
            RegisterAccept(args);
        }

        // 클라이언트 연결 승인 요청
        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            // 서버는 승인요청을 받는 작업을 할 SocketAsyncEventArgs 객체와 함께
            // 클라이언트의 요청이 들어오면 승인 작업을 하도록 처리합니다.
            bool pending = m_Socket.AcceptAsync(args);


            if (pending == false)
            {
                OnAcceptCompleted(null, args);
            }


        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            // 종료되는 클라이언트의 key를 찾는 코드
            int key = _clients.FirstOrDefault(x => x.Value == args.AcceptSocket).Key;

            // 클라이언트가 연결을 끊었거나 오류가 발생한 경우
            if (args.SocketError != SocketError.Success || args.BytesTransferred == 0)
            {
                HandleDisconnectedClient(key);

                return;
            }

            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                string recvData = System.Text.Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                byte[] sendArray = System.Text.Encoding.UTF8.GetBytes(recvData);

                //sendQueue.Enqueue(sendArray);

                //// 받은 데이터를 연결된 모든 클라이언트에게 전송하는 함수입니다.
                //RegisterSend();

                // 새로운 데이터 수신을 위해 준비합니다.
                RegisterRecv(args);
            }
        }

        // 클라이언트가 연결을 끊었을 때 처리하는 함수
        void HandleDisconnectedClient(int clientId)
        {
            if (_clients.TryRemove(clientId, out Socket clientSocket))
            {
                // 소켓 종료 및 자원 해제
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                clientSocket.Dispose();
                Console.WriteLine($"클라이언트 {clientId}가 제거되었습니다.");
            }
        }

    }
}
