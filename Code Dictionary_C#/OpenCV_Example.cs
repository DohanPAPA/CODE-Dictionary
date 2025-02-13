using System;
using System.Collections.Generic;
using System.Threading.Tasks;



using OpenCvSharp;
using OpenCvSharp.Blob;
using OpenCvSharp.Extensions;

namespace Code_Dictionary_C_
{
    public class OpenCV_Example
    {

        bool bRet = false;

        // 이진화 처리
        public Mat Binary_Filter(Mat OriginImage)
        {
            Mat m_Clone_OriginImage = new Mat();

            // 새로운 이미지를 담을 변수
            Mat New_Image = new Mat();
            // 원본 이미지 복사본
            m_Clone_OriginImage = OriginImage.Clone();

            // 이진화 처리된 이미지 사용
            Cv2.InRange(m_Clone_OriginImage, new Scalar(1, 0, 0), new Scalar(255, 255, 255), New_Image);

            return New_Image;
        }

        void Event(MouseEventTypes @event, int x, int y, MouseEventFlags flags, IntPtr userdata)
        {
            Mat data = new Mat(userdata);

            if (flags == MouseEventFlags.LButton)
            {
                Cv2.Circle(data, new OpenCvSharp.Point(x, y), 10, new Scalar(0, 0, 255), -1);
                Cv2.ImShow("draw", data);
            }
        }

        public async void VideoCature_Task_Run_()
        {

            // 카메라 출력의 인덱스는 카메라의 장치 번호(ID)를 의미함
            // 웹캠이 내장된 노트북이나 카메라가 내장돼 있지 않은 컴퓨터에 카메라를 연결할 경우 장치번호 0을 사용함
            VideoCapture capture = new VideoCapture(0);

            Mat frame = new Mat();

            capture.Set(VideoCaptureProperties.FrameWidth, 640);
            capture.Set(VideoCaptureProperties.FrameHeight, 480);

            await Task.Delay(8);
            bRet = true;
            while (bRet)
            {
                if (capture.IsOpened() == true)
                {
                    capture.Read(frame);
                    Cv2.ImShow("VideoFrame", frame);
                    if (Cv2.WaitKey(10) == 'q')
                    {
                        bRet = false;
                        break;
                    }
                    //if (Cv2.WaitKey(99) == 'c')
                    //{
                    //    bool Success = capture.Grab();

                    //    if (Success)
                    //    {
                    //    }
                    //}

                }
            }
            capture.Release();
            Cv2.DestroyAllWindows();
        }
        public void TrackBar_Task_Run()
        {
            int value_R = 0;
            int value_G = 0;
            int value_B = 0;
            Cv2.NamedWindow("Palette");
            Cv2.CreateTrackbar("R", "Palette", ref value_R, 255);
            Cv2.CreateTrackbar("G", "Palette", ref value_G, 255);
            Cv2.CreateTrackbar("B", "Palette", ref value_B, 255);

            //await Task.Delay(8);
            while (true)
            {
                int pixel_R = Cv2.GetTrackbarPos("R", "Palette");
                int pixel_G = Cv2.GetTrackbarPos("G", "Palette");
                int pixel_B = Cv2.GetTrackbarPos("B", "Palette");
                Mat src = new Mat(new OpenCvSharp.Size(500, 500), MatType.CV_8UC3, new Scalar(pixel_R, pixel_G, pixel_B)); ;
                Cv2.ImShow("Palette", src);
                if (Cv2.WaitKey(10) == 'q')
                {
                    break;
                }
            }
            Cv2.DestroyAllWindows();
        }

        // Hue 공간 색상 검출
        public void Spatial_Color_Detection()
        {
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg", ImreadModes.Color); // 입력이미지
            Mat hsv = new Mat(src.Size(), MatType.CV_8UC1); // 3채널 이미지에서 1채널 이미지로 변환(단일채널)
            Mat dst = new Mat(src.Size(), MatType.CV_8UC1); // 출력이미지

            Cv2.CvtColor(src, hsv, ColorConversionCodes.BGR2HSV); // 색상공간 변환 함수 => (원본 이미지, 결과 이미지, 색상 변환 코드)
            Mat[] HSV = Cv2.Split(hsv);
            Mat H_orange = new Mat(src.Size(), MatType.CV_8UC1);
            Cv2.InRange(HSV[0], new Scalar(8), new Scalar(20), H_orange);

            Cv2.BitwiseAnd(hsv, hsv, dst, H_orange);
            Cv2.CvtColor(dst, dst, ColorConversionCodes.HSV2BGR);

            Cv2.ImShow("Orange", dst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        // 색상 검출
        public void Color_Inspec()
        {
            // HSV의 값을 설정할 때 각 속성마다 최솟값과 최댓값이 있음
            // 색상 = 0 ~ 179
            // 채도 = 0 ~ 255
            // 명도 = 0 ~ 255
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg", ImreadModes.Color);
            Mat hsv = new Mat(src.Size(), MatType.CV_8UC1);
            Mat lower_red = new Mat(src.Size(), MatType.CV_8UC1);
            Mat upper_red = new Mat(src.Size(), MatType.CV_8UC1);
            Mat added_red = new Mat(src.Size(), MatType.CV_8UC1);
            Mat dst = new Mat(src.Size(), MatType.CV_8UC1);

            Cv2.CvtColor(src, hsv, ColorConversionCodes.BGR2HSV);

            Cv2.InRange(hsv, new Scalar(0, 100, 100), new Scalar(5, 255, 255), lower_red);
            Cv2.InRange(hsv, new Scalar(170, 100, 100), new Scalar(179, 255, 255), upper_red);
            Cv2.AddWeighted(lower_red, 1.0, upper_red, 1.0, 0.0, added_red);

            Cv2.BitwiseAnd(hsv, hsv, dst, added_red);
            Cv2.CvtColor(dst, dst, ColorConversionCodes.HSV2BGR);

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        // 이진화
        public void Threshold()
        {
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg", ImreadModes.Color);
            Mat gray = new Mat(src.Size(), MatType.CV_8UC1);
            Mat binary = new Mat(src.Size(), MatType.CV_8UC1);

            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            // (원본이미지, 결과 이미지, 임계값, 최대값, 임계값 형식) 으로 이진화를 적용!!
            Cv2.Threshold(gray, binary, 127, 255, ThresholdTypes.Otsu);

            Cv2.ImShow("binary", binary);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        // 적응형 이진화
        public void AdaptiveThreshold()
        {
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg", ImreadModes.Color);
            Mat gray = new Mat(src.Size(), MatType.CV_8UC1);
            Mat binary = new Mat(src.Size(), MatType.CV_8UC1);

            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            // (원본이미지, 결과 이미지, 임계값, 최대값, 임계값 형식) 으로 이진화를 적용!!
            Cv2.AdaptiveThreshold(gray, binary, 225, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 25, 5);

            Cv2.ImShow("binary", binary);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        // 흐림효과
        public void Blurring()
        {
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg", ImreadModes.Color);
            Mat dst = new Mat(src.Size(), MatType.CV_8UC3);

            Cv2.GaussianBlur(src, dst, new OpenCvSharp.Size(9, 9), 3, 3, BorderTypes.Isolated);

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        // 이미지 확대
        public void Image_Up()
        {
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg", ImreadModes.Color);
            Mat dst = new Mat(src.Size(), MatType.CV_8UC3);

            Cv2.PyrUp(src, dst, new OpenCvSharp.Size((src.Width * 2) + 1, (src.Height * 2) - 1));

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        // 이미지 크기 조절
        public void Image_Size()
        {
            // 이미지 불러오기
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg", ImreadModes.Color);
            Mat dst = new Mat(new OpenCvSharp.Size(), MatType.CV_8UC3);

            // Cv2.Resize*(원본 이미지, 결과 이미지, 절대 크기, 상대 크기(X), 상대 크기(Y), 보간법)
            Cv2.Resize(src, dst, new OpenCvSharp.Size(0, 0), 0.5, 0.5, InterpolationFlags.Cubic);


            // Rect(int x, int y, int width, int height)
            // 아래처럼 사용 가는ㅇ
            // Rect rect = new Rect(70, 30, 120, 120);
            // dst = src.SubMat(rect); 
            // Cv2.SubMat*(시작 X좌표, 시작 Y좌표, 너비, 높이)
            //dst = src.SubMat(280, 310, 50, 405); // 관심영역 추출
            //Cv2.Resize(dst, dst, new OpenCvSharp.Size(9999, 0), 0.5, 0.5, InterpolationFlags.Cubic); // 이미지 리사이즈(원하는 이미지 사이즈로 변경)

            pictureBox1.Image = BitmapConverter.ToBitmap(dst);

            Cv2.ImShow("dst", dst);

            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        // 대칭,회전
        public void Flip_or_Rotation()
        {
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg", ImreadModes.Color);
            Mat dst = new Mat();

            Cv2.Flip(src, dst, FlipMode.Y);
            // GetRotationMatrix2D = (Center : 회전의 기준이 될 중심 , angle : 각도 , Scale : 회전 후의 이미지의 확대 또는 축소 비율)
            Mat matrix = Cv2.GetRotationMatrix2D(new Point2f(src.Width / 2, src.Height / 2), 40, 1);

            Cv2.WarpAffine(dst, dst, matrix, new OpenCvSharp.Size(src.Width, src.Height));

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        // 아핀 변환
        public void affine_Transform()
        {
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg", ImreadModes.Color); // 입력
            Mat dst = new Mat(); // 출력

            List<Point2f> src_pts = new List<Point2f>() // 3 * 3
            {
                new Point2f(0.0f,0.0f),
                new Point2f(0.0f,src.Height),
                new Point2f(src.Width,src.Height)
            };

            List<Point2f> dst_pts = new List<Point2f>() // 3 * 3
            {
                new Point2f(300.0f,300.0f),
                new Point2f(300.0f,src.Height),
                new Point2f(src.Width - 400.0f,src.Height-200.0f)
            };

            Mat M = Cv2.GetAffineTransform(src_pts, dst_pts);

            Cv2.WarpAffine(src, dst, M, new OpenCvSharp.Size(src.Width, src.Height), borderValue: new Scalar(127, 127, 127, 0)); // borderValue : 공백 공간의 테두리 색상

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        // 모폴로지 팽창,침식 연산
        // 이진화된 이미지에 사용
        public void Morphological_Transformations()
        {
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg", ImreadModes.Color); // 입력
            Mat dst = new Mat(); // 출력

            // 커널 : 7*7크기 , 십자가 형태의 구조 요소
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Cross, new OpenCvSharp.Size(7, 7));
            // 모폴로지 팽창 : 3회 적용 , Point : 고정점은 커널의 중심점으로 할당 => 밝은영역이 늘어나고 어두운 영역이 줄어든다. 
            Cv2.Dilate(src, dst, kernel, new OpenCvSharp.Point(-1, -1), 3, BorderTypes.Reflect101, new Scalar(0)); // => 팽창 : 어두운곳을 밝게
                                                                                                                   //Cv2.Erode(src, dst, kernel, new OpenCvSharp.Point(-1, -1), 3, BorderTypes.Replicate, new Scalar(0)); // => 침식 : 밝은곳을 어둡게


            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        // 모폴로지 연산
        // 이진화 처리된 이미지라면 팽창과 침식 연산으로도 우수한 결과를 얻을수있지만
        // 그레이스케일이나 다중 채널 이미지를 사용하는 경우 더 복잡한 연산이 필요하기에 모폴로지 연산을 사용한다. 
        public void Morphological_calculation()
        {
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg", ImreadModes.Grayscale); // 입력

            Mat dst = new Mat(); // 출력


            Mat kernel = Mat.Zeros(new OpenCvSharp.Size(7, 7), MatType.CV_8UC1);
            kernel[0, 7, 0, 1] = Mat.Ones(new OpenCvSharp.Size(1, 7), MatType.CV_8UC1);
            kernel[0, 1, 0, 7] = Mat.Ones(new OpenCvSharp.Size(7, 1), MatType.CV_8UC1);

            // (입력이미지,출력이미지,연산함수,커널,반복횟수)
            Cv2.MorphologyEx(src, dst, MorphTypes.HitMiss, kernel, iterations: 10); // 히트미스 : 이진화 처리된 이미지에 커널의 형태를 남겨 모서리(코너)를 검출하는 용도로 활용

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        // 이미지 검출
        // 가장자리 검출(Edge)
        public void Edge()
        {
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg", ImreadModes.Grayscale); // 입력
            Mat dst = new Mat(); // 출력

            //// (xorder,yorder,kernel size, scale,delta , bordertypes)
            //Cv2.Sobel(src, dst, MatType.CV_8UC1, 1, 0, 3, 1, 0, BorderTypes.Reflect101); // 소벨 미분
            //// 메개변수는 소벨 미분과 동일하나 커널의 크기는 사용 안함
            //Cv2.Scharr(src,dst,MatType.CV_8UC1,1,0,1,0,BorderTypes.Reflect101); // 샤르 필터 ( 3*3만 지원) => 커널의 크기가 작으면 정확도가 떨어지는데 그걸 해결하기위해서 사용
            //// 2차 미분 형태 // 라플라시안
            //Cv2.Laplacian(src,dst, MatType.CV_8UC1,1,1,0, BorderTypes.Reflect101);
            //// 캐니 엣지 => 성능이 월등히 좋으며 노이즈에 민감하지 않아 강한 가장자리를 검출하는데 목적을 둔 알고리즘
            // (입력 이미지,출력 이미지,하위 임곗값, 상위 임곗값, 소벨 연산자 마스크 크기, L2 그레이디언트)
            Cv2.Canny(src, dst, 100, 200, 3, true);

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        // 윤곽선 검출
        // 윤곽선 검출을 수행함에 있어 가능한 노이즈가 없거나 적어야한다. 노이즈를 제거하기 위해 이진화를 적용하며, 이후 모폴로지 연산을 통해 스펙클을 제거한다. 
        // 또 윤곽선 검출 함수를 사용하려면 먼저 윤곽선을 저장할 공간과 계층 구조를 저장할 공간을 선언해야 한다. 
        public void Contour()
        {
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg");
            Mat gray = new Mat();
            Mat binary = new Mat();
            Mat morp = new Mat();
            Mat image = new Mat();
            Mat dst = src.Clone();

            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));

            // 윤곽선 검출 함수를 사용하려면 먼저 윤곽선을 저장할 공간과 계층 구조를 저장할 공간을 선언해야 한다. 
            //===================================================================================================
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            //===================================================================================================

            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY); // 색상공간 변환 함수 => (원본 이미지, 결과 이미지, 색상 변환 코드)
            Cv2.Threshold(gray, binary, 230, 255, ThresholdTypes.Binary); // (원본이미지, 결과 이미지, 임계값, 최대값, 임계값 형식) 으로 이진화를 적용!!
            Cv2.MorphologyEx(binary, morp, MorphTypes.Close, kernel, new OpenCvSharp.Point(-1, -1), 2); // 모폴로지 연산 => (입력이미지,출력이미지,연산함수,커널,반복횟수)
            Cv2.BitwiseNot(morp, image);

            // (입력 이미지, 검출된 윤곽선, 계층구조, 검색방법, 근사 방법 , 오프셋)
            // 오프셋 : 반환된 윤곽점들의 좌푯값에 이동할 값을 설정한다. 관심 영역에서 윤곽선을 검출하거나 다른 이미지에 표시하고자 할 때 주로 활용
            Cv2.FindContours(image, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone, null); // 윤곽선 검출
            // (입력 이미지, 윤곽선, 윤곽선 번호, 색상, 두께, 선형 타입, 계층구조, 계층 구조 최대 레벨, 오프셋)
            Cv2.DrawContours(dst, contours, -1, new Scalar(255, 0, 0), 2, LineTypes.AntiAlias, hierarchy, 3, null); // 윤곽선 그리기

            for (int i = 0; i < contours.Length; i++)
            {
                for (int j = 0; j < contours[i].Length; j++)
                {
                    Cv2.Circle(dst, contours[i][j], 1, new Scalar(0, 0, 255), 3);
                }
            }

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        // 다각형 근사
        public void aaa()
        {
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg");
            Mat gray = new Mat();
            Mat binary = new Mat();
            Mat morp = new Mat();
            Mat image = new Mat();
            Mat dst = src.Clone();

            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));

            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;

            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY); // 색상공간 변환 함수 => (원본 이미지, 결과 이미지, 색상 변환 코드)
            Cv2.Threshold(gray, binary, 230, 255, ThresholdTypes.Binary); // (원본이미지, 결과 이미지, 임계값, 최대값, 임계값 형식) 으로 이진화를 적용!!
            Cv2.MorphologyEx(binary, morp, MorphTypes.Close, kernel, new OpenCvSharp.Point(-1, -1), 2); // 모폴로지 연산 => (입력이미지,출력이미지,연산함수,커널,반복횟수)
            Cv2.BitwiseNot(morp, image);

            // (입력 이미지, 검출된 윤곽선, 계층구조, 검색방법, 근사 방법 , 오프셋)
            // 오프셋 : 반환된 윤곽점들의 좌푯값에 이동할 값을 설정한다. 관심 영역에서 윤곽선을 검출하거나 다른 이미지에 표시하고자 할 때 주로 활용
            Cv2.FindContours(image, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone, null); // 윤곽선 검출

            for (int i = 0; i < contours.Length; i++)
            {
                double perimeter = Cv2.ArcLength(contours[i], true);
                double epsilon = perimeter * 0.01;

                OpenCvSharp.Point[] approx = Cv2.ApproxPolyDP(contours[i], epsilon, true);
                OpenCvSharp.Point[][] draw_approx = new OpenCvSharp.Point[][] { approx };
                Cv2.DrawContours(dst, draw_approx, -1, new Scalar(255, 0, 0), 2, LineTypes.AntiAlias);

                for (int j = 0; j < approx.Length; j++)
                {
                    Cv2.Circle(dst, approx[j], 1, new Scalar(0, 0, 255), 3);
                }
            }

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        // 윤곽선 계산
        public void Length()
        {
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg");
            Mat image = new Mat();
            Mat dst = src.Clone();

            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;

            Cv2.InRange(src, new Scalar(0, 127, 127), new Scalar(100, 255, 255), image);
            Cv2.FindContours(image, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxTC89KCOS);

            foreach (OpenCvSharp.Point[] p in contours)
            {
                double length_ = Cv2.ArcLength(p, true);
                double area = Cv2.ContourArea(p, true);

                if (length_ < 100 && area < 1000 && p.Length < 5) continue;

                Rect boundingRect = Cv2.BoundingRect(p);
                RotatedRect rotatedRect = Cv2.MinAreaRect(p);
                RotatedRect ellipse = Cv2.FitEllipse(p);

                Point2f center;
                float radius;
                Cv2.MinEnclosingCircle(p, out center, out radius);

                Cv2.Rectangle(dst, boundingRect, Scalar.Red, 2);
                Cv2.Ellipse(dst, rotatedRect, Scalar.Blue, 2);
                Cv2.Ellipse(dst, ellipse, Scalar.Green, 2);
                Cv2.Circle(dst, (int)center.X, (int)center.Y, (int)radius, Scalar.Yellow, 2);
            }


            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        // 코너 검출 및 픽셀 세밀화
        // 다각형의 꼭짓점을 검출로 이해 => 정확하게는 트래킹 하기 좋은 지점(특징)을 코너라 한다.
        public void Corners()
        {
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg");
            Mat gray = new Mat();
            Mat dst = src.Clone();

            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

            Point2f[] corners = Cv2.GoodFeaturesToTrack(gray, 100, 0.03, 5, null, 3, false, 0);
            Point2f[] sub_corners = Cv2.CornerSubPix(gray, corners, new OpenCvSharp.Size(3, 3), new OpenCvSharp.Size(-1, -1), TermCriteria.Both(10, 0.03));

            for (int i = 0; i < corners.Length; i++)
            {
                OpenCvSharp.Point pt = new OpenCvSharp.Point((int)corners[i].X, (int)corners[i].Y);
                Cv2.Circle(dst, pt, 5, Scalar.Yellow, Cv2.FILLED);
            }

            for (int i = 0; i < sub_corners.Length; i++)
            {
                OpenCvSharp.Point pt = new OpenCvSharp.Point((int)sub_corners[i].X, (int)sub_corners[i].Y);
                Cv2.Circle(dst, pt, 5, Scalar.Red, Cv2.FILLED);
            }

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        // 확률 허프 변환
        public void sss()
        {
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg");
            Mat gray = new Mat();
            Mat binary = new Mat();
            Mat morp = new Mat();
            Mat canny = new Mat();
            Mat dst = src.Clone();

            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));

            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(gray, binary, 150, 255, ThresholdTypes.Binary);
            Cv2.Dilate(binary, morp, kernel, new OpenCvSharp.Point(-1, -1));
            Cv2.Erode(morp, morp, kernel, new OpenCvSharp.Point(-1, -1), 3);
            Cv2.Dilate(morp, morp, kernel, new OpenCvSharp.Point(-1, -1), 2);
            Cv2.Canny(morp, canny, 0, 0, 3);

            LineSegmentPoint[] lines = Cv2.HoughLinesP(canny, 1, Cv2.PI / 180, 140, 50, 10);

            for (int i = 0; i < lines.Length; i++)
            {
                Cv2.Line(dst, lines[i].P1, lines[i].P2, Scalar.Yellow, 2);
            }

            Cv2.ImShow("dst", dst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        public void Blob()
        {
            Mat src = Cv2.ImRead("D:\\03.KKH\\KIMKEUNHO.jpg");
            Mat bin = new Mat();
            Cv2.CvtColor(src, bin, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(bin, bin, 0, 255, ThresholdTypes.Otsu);
            Cv2.ImShow("src", bin);

            Mat result = new Mat(src.Size(), MatType.CV_8UC3);
            CvBlobs blobs = new CvBlobs();
            blobs.Label(bin);
            blobs.RenderBlobs(src, result);

            foreach (var item in blobs)
            {
                CvBlob b = item.Value;

                Cv2.Circle(result, b.Contour.StartingPoint, 4, Scalar.Red, 2, LineTypes.AntiAlias);
                Cv2.PutText(result, b.Label.ToString(), new OpenCvSharp.Point(b.Centroid.X, b.Centroid.Y),
                    HersheyFonts.HersheyComplex, 1, Scalar.Yellow, 2, LineTypes.AntiAlias);
            }

            Cv2.ImShow("result", result);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }
    }
}
