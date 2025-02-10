using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Code_Dictionary_C_
{
    internal class FTP_Server
    {
        public bool IsConnected { get; set; }

        string ftpPath = @"FTP://ftp.microsoft.com/softlib/index.txt";
        string user = "anonymous";  // FTP 익명 로그인시. 아니면 로그인/암호 지정.
        string pwd = "";
        string outputFile = "index.txt";

        // 폴더 생성
        public void CreateFtpDirectory(string ftpServer, string folderPath, string ftpUsername, string ftpPassword)
        {
            try
            {
                // FTP 전송 요청
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpServer + folderPath);
                ftpRequest.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;// 폴더 덮어쓰기 x

                // * (서버 폴더에 대해서) 폴더는 덮어쓰기가 안되어 에러 발생
                // FTP 처리 응답 에러는 필수로 닫아줘야 다음 실행이 가능.
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
            }

            catch (WebException ex)
            {
                // 덮어쓰기 메시지가 뜨는데 무시해도 된다.
                return;
            }
        }

        // 파일 업로드
        public void UploadFiles(string ftpFolderPath, string ftpServer, string ftpUsername, string ftpPassword)
        {
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpServer + ftpFolderPath);
                ftpRequest.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile; // 파일 덮어쓰기 o

                // 로컬 파일 읽기
                using (FileStream localFileStream = new FileStream("", FileMode.Open))
                {
                    // FTP 스트림 열기
                    using (Stream ftpStream = ftpRequest.GetRequestStream())
                    {
                        // 로컬 파일을 FTP 스트림으로 복사
                        // FTP로 복사되는 개념!!!
                        localFileStream.CopyTo(ftpStream);
                    }
                }
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
            }
            catch (Exception ex)
            {

            }
        }

        //폴더 및 파일 리스트 체크
        /// <summary>
        /// FTP서버에서는 로컬과 달리, 파일과 디렉토리를 따로 읽는 함수가 없다.
        /// ListDirectoryDetails란 녀석을 사용해서 파일, 디렉토리 권한 정보를 토대로
        /// 접근해야 한다.불편하다...
        /// </summary>
        /// <returns>디렉토리, 파일</returns>
        public (string[] directories, string[] files) GetFtpDirectoryList(string ftpUrl, string userName, string password)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(userName, password);

                // FTP 응답 가져오기
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    // 응답 데이터를 읽어와서 폴더와 파일 리스트로 변환 함수로 보내기
                    // * 접근 권한과 이름만 가져올 수 있음.
                    using (System.IO.StreamReader streamReader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        string directoryList = streamReader.ReadToEnd();

                        FtpWebResponse ftpResponse = (FtpWebResponse)request.GetResponse();
                        ftpResponse.Close();

                        return ParseFtpDirectoryList(directoryList);
                    }
                }
            }
            catch (WebException ex)
            {
                // 리턴으로 값을 줘야 갯수를 판단할 수 있다.
                return (null, null);
            }
        }

        /// <summary>
        /// 권한 정보를 토대로 파일인지, 폴더인지 검사해서 반환하는 함수
        /// </summary>
        public (string[] directories, string[] files) ParseFtpDirectoryList(string directoryList)
        {
            // 띄어쓰기와 엔터값을 기준으로 이름을 스플릿
            string[] entries = directoryList.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<string> directories = new List<string>();
            List<string> files = new List<string>();

            foreach (string entry in entries)
            {
                string trimmedEntry = entry.Trim();

                // 폴더인지 파일인지 판단(권한 내용을 토대로)
                // 'd'로 시작하면 폴더
                bool isDirectory = trimmedEntry.StartsWith("d", StringComparison.OrdinalIgnoreCase);

                // 공백으로 분리된 부분들을 가져오기
                string[] parts = trimmedEntry.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 0)
                {
                    // 파일 또는 폴더의 이름 가져오기 (마지막 부분이 아닌 두 번째 열에서 가져옴)
                    string name = parts.Length >= 9 ? parts[8] : parts[parts.Length - 1];

                    if (isDirectory)
                    {
                        // 폴더인 경우 폴더 리스트에 추가
                        directories.Add(name);
                    }
                    else
                    {
                        // 파일인 경우 파일 리스트에 추가
                        files.Add(name);
                    }
                }
            }
            return (directories.ToArray(), files.ToArray());
        }


        // 폴더 및 파일 삭제
        /// <summary>
        /// 삭제 로직
        /// </summary>
        public void DeleteFtpAll(string dirPath, string ftpUsername, string ftpPassword)
        {
            try
            {
                // 1. 하위 폴더 및 파일 리스트를 가져옴(* ftp 서버에서 방법 특이)
                (string[] directories, string[] files) = GetFtpDirectoryList(dirPath, ftpUsername, ftpPassword);

                // 2. 파일들부터 삭제
                foreach (string subFile in files)
                {
                    DeleteFileFromFtp(dirPath, subFile, ftpUsername, ftpPassword);
                }

                // 3. 하위 폴더 삭제
                foreach (string subDir in directories)
                {
                    string nextPath = dirPath + "/" + subDir;
                    DeleteFtpAll(nextPath, ftpUsername, ftpPassword);
                }

                // 4. 마지막으로 하위에 아무것도 없다면, 폴더 삭제
                (directories, files) = GetFtpDirectoryList(dirPath, ftpUsername, ftpPassword);
                if (directories.Length == 0 && files.Length == 0)
                {
                    DeleteFolderFromFtp(dirPath, ftpUsername, ftpPassword);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void DeleteFileFromFtp(string ftpServerUrl, string filePath, string username, string password)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri($"{ftpServerUrl}/{filePath}"));
                request.Credentials = new NetworkCredential(username, password);
                request.Method = WebRequestMethods.Ftp.DeleteFile;

                FtpWebResponse ftpResponse = (FtpWebResponse)request.GetResponse();
                ftpResponse.Close();
            }
            catch (WebException ex)
            {

            }
        }

        public void DeleteFolderFromFtp(string folderPath, string ftpUsername, string ftpPassword)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(folderPath));
                request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                request.Method = WebRequestMethods.Ftp.RemoveDirectory;

                FtpWebResponse ftpResponse = (FtpWebResponse)request.GetResponse();
                ftpResponse.Close();
            }
            catch (WebException ex)
            {

            }
        }
    }
}
