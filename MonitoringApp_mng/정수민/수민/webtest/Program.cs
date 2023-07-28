using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace webtest
{
    class Program
    {
        static void Main(string[] args)
        {
            // 크롤링할 웹 사이트 URL 설정
            string url = "https://www.weather.go.kr/w/obs-climate/land/city-obs.do";

            // 웹 페이지 다운로드
            string htmlContent = DownloadWebPage(url);

            // HTML 파싱하여 부산의 기온과 습도 추출
            ParseWeatherData(htmlContent, "부산");
        }

        static string DownloadWebPage(string url)
        {
            string htmlContent;
            using (WebClient client = new WebClient())
            {
                // UTF-8로 인코딩된 웹 페이지 다운로드
                client.Encoding = System.Text.Encoding.UTF8;
                htmlContent = client.DownloadString(url);
            }
            return htmlContent;
        }

        static void ParseWeatherData(string htmlContent, string city)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            // 테이블의 모든 행을 가져옴
            HtmlNodeCollection rows = doc.DocumentNode.SelectNodes("//table[@class='table-col']//tr");

            if (rows != null)
            {
                bool foundCity = false;
                foreach (HtmlNode row in rows)
                {
                    // 행에서 모든 셀을 가져옴
                    HtmlNodeCollection cells = row.SelectNodes("td");

                    if (cells != null && cells.Count >= 7)
                    {
                        string location = cells[0].InnerText.Trim();
                        string temperature = cells[5].InnerText.Trim();
                        string humidity = cells[6].InnerText.Trim();
                        // Get the content inside the <script> tag
                        string windScript = cells[11].InnerHtml;
                        string windSpeed = ExtractWindSpeedFromScriptTag(windScript);


                        // 지역 이름이 부산이면 데이터를 출력하고 반복을 종료함
                        if (location.Contains(city))
                        {
                            Console.WriteLine($"지역: {location}, 기온: {temperature}°C, 습도: {humidity}%, 풍속: {windSpeed}");
                            foundCity = true;
                            break;
                        }
                    }
                }

                if (!foundCity)
                {
                    Console.WriteLine($"'{city}'의 데이터를 찾을 수 없습니다.");
                }
            }
            else
            {
                Console.WriteLine("데이터 테이블을 찾을 수 없습니다.");
            }
        }

        private static string ExtractWindSpeedFromScriptTag(string windScript)
        {
            string pattern = @"writeWindSpeed\('([^']+)'";
            Match match = Regex.Match(windScript, pattern);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return "N/A"; // Return "N/A" if the wind speed is not found or can't be extracted.
        }
    }
}
