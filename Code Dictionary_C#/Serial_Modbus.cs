using System.IO.Ports;

namespace Code_Dictionary_C_
{

    // Serial 통신은 RS232,485,442가 있다.
    // 흔히 사용하는 통신은 RS232,485가 있다
    // RS232는 직렬통신방식으로 1대1 통신에서 사용 , 보레이트19200bit/s 전송 속도에 약 15m 통신 거리를 보장 , 통신거리는 케이블 종류와 보레이트에 영향받는다.
    // ┖> 구조 : [START BIT , BATA(5~9)BIT , PARITY BIT , STOP BIT]
    // RS485는 직렬통신방식으로 전이중통신 , 최대 드라이버 리시버 수는 각각 32개 최대 속도 10Mbps에 최장 거리 1.2km까지 네트워크 구축 가능
    // ┖> 2선식 4선식 방식 중 선택 가능
    // ┖> 구조 : [START BIT , BATA(5~9)BIT , PARITY BIT , STOP BIT]
    internal class Serial_Modbus
    {

        SerialPort serialPort_ = new SerialPort();

        public Serial_Modbus()
        {
            serialPort_.PortName = "";
            serialPort_.BaudRate = 115200;
            serialPort_.DataBits = 0;
            serialPort_.Parity = Parity.None;
            serialPort_.StopBits = StopBits.None;

            serialPort_.Open();
            serialPort_.Close();
        }

        public void disconnection()
        {
            serialPort_.Close();
        }

        // CRC16 Check
        public void CRC_CHECK(byte[] Message_, ref byte[] CRC)
        {
            // 1단계
            ushort CRCFull = 0XFFFF;

            // 상위 필드         // 하위 필드
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            // 5단계
            for (int i = 0; i < Message_.Length - 2; i++)
            {
                // 2단계
                CRCFull = (ushort)(CRCFull ^ Message_[i]);

                for (int j = 0; j < 8; i++)
                {
                    // LSB : 가장 낮은 위치의 BIT(제일 오른쪽 비트). ex)1101010 => 0
                    // MSB : 가장 높은 위치의 BIT(제일 왼쪽 비트).   ex)1101010 => 1
                    CRCLSB = (char)(CRCFull & 0x0001); // LSB
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF); // MBS

                    // 3단계
                    if (CRCLSB == 1)
                        // 4단계
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }

            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }
    }

    //// 그래프 알고리즘
    //class Vertex
    //{
    //    public List<Vertex> edges = new List<Vertex>();
    //}

    //class programse
    //{
    //    void main()
    //    {
    //        List<Vertex> v = new List<Vertex>()
    //        {
    //            new Vertex(),
    //            new Vertex(),
    //            new Vertex(),
    //            new Vertex(),
    //            new Vertex(),
    //            new Vertex(),
    //        };

    //        v[0].edges.Add(v[1]);  // v[0] 정점은 v[1], v[3] 과 연결됨.
    //        v[0].edges.Add(v[3]);
    //        v[1].edges.Add(v[0]);  // v[1] 정점은 v[0], v[2], v[3] 과 연결됨.
    //        v[1].edges.Add(v[2]);
    //        v[1].edges.Add(v[3]);
    //        v[3].edges.Add(v[4]);  // v[3] 정점은 v[4] 과 연결됨.
    //        v[5].edges.Add(v[4]);  // v[5] 정점은 v[4] 과 연결됨.

    //    }
    //}


}
