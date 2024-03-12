using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using System;

namespace DiceGame.Network
{
    public class InternetConnection : MonoBehaviour
    {
        public static string googleUrl = "https://google.com";


        /// <summary>
        /// URL에서 HTML 코드를 가져오는 함수
        /// </summary>
        /// <param name="url"> resource </param>
        /// <returns> html </returns>
        public static string GetHtmlFromUrl(string url)
        {
            string html = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); // resource에 대한 Http 프로토콜 웹 요청 객체 생성

            // 웹 요청에 따른 응답 받기
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                // 응답 상태가 OK면
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // 웹에 있는 데이터를 읽기 (웹-Application이므로 Stream의 버퍼에 데이터를 쓰고 읽기하므로... StreamReader로 해당 Stream읽기)
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        char[] buffer = new char[100];
                        streamReader.Read(buffer, 0, buffer.Length);
                        html = new string(buffer);
                    }
                }
            }

            return html;
        }

        public static bool IsGoogleWebsiteReachable()
        {
            string html = GetHtmlFromUrl(googleUrl);

            if (string.IsNullOrEmpty(html))
                return false;
            
            return true;
        }
    }
}
