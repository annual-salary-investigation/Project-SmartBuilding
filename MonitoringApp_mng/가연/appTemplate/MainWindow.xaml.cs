using appTemplate.Models;
using appTemplate.Views;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace appTemplate
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized; // 실행시 전체화면
            WindowStartupLocation = WindowStartupLocation.CenterScreen; // 스크린 정 중앙에 창 띄우기 

            #region < 대시보드1 날씨영역>
            // 날짜, 요일, 시간
            Txtdate.Text = DateTime.Today.ToShortDateString();
            Txtday.Text = DateTime.Now.DayOfWeek.ToString();
            TxtTime.Text = DateTime.Now.ToShortTimeString();

            
            #endregion


        }

        #region <로그인 창 로드 영역 - 앱 구현 마지막 단계에 주석 지우고 사용!>
        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //    var loginWindow = new Login();
            //    loginWindow.Owner = this; // LoginWindow의 부모는 MainWindow
            //    loginWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner; // MainWindow의 정중앙에 위치
            //    loginWindow.ShowDialog();  // 모달창


            //OpenAPI로 날씨값 받아오기
            try
            {
                await CheckWeatehr();
            }
            catch(Exception ex)
            { 
                await Logics.Commons.ShowMessageAsync("오류", $"오류 발생 : {ex.Message}");
            }
        }
        #endregion

        #region < 차량 관리 버튼 이벤트 영역 - 자식창 띄우기>
        private void BtnMngCar_Click(object sender, RoutedEventArgs e)
        {
            var mngCarWindow = new MngCar();
            mngCarWindow.Owner = this;
            mngCarWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner; // 부모창 정중앙에 띄우기
            mngCarWindow.ShowDialog(); // 모달창
        }
        #endregion

        #region < 실제 OpenAPI 검색 영역 >
        public async Task CheckWeatehr() 
        {
            string serviceKey = "6U1OgOHXoO56%2FqdyFMr%2F5XyU8d6H2iWdFnZmtNuLA%2BhDq3mNlkfOIxbEpgVMVWrU9cb5HM8NAs2iNA0UcXE8ag%3D%3D"; // 기상청 API 인증키

            string openApiUri = $"http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtNcst?serviceKey={serviceKey}&numOfRows=10&dataType=JSON&pageNo=1&base_date=20230712&base_time=1200&nx=98&ny=74"; // openAPI 요청
            string result = string.Empty; //결과값 초기화

            // API 실행할 WebRequest, WebResponse 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try // API 요청
            {
                req = WebRequest.Create(openApiUri); // URL을 넣어서 객체를 생성
                res = await req.GetResponseAsync(); // 요청한 결과를 응답에 할당
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd(); // json결과 텍스트로 저장
            }
            catch (Exception ex)
            {
                throw ex; //  MetroWindow_Loaded에서 오류메세지 보여주기 때문에 여기선 그냥 던져주기
            }
            finally
            {
                reader.Close();
                res.Close();
            }

            //result를 json으로 변경
            var jsonResult = JObject.Parse(result); // string -> json
            ///Debug.WriteLine(jsonResult.ToString());
            var item = jsonResult["response"]["body"]["items"]["item"]; // json 객체 key값인 item 으로 접근
            //Debug.WriteLine(item.ToString());
            var json_array = item as JArray;
            Debug.WriteLine(json_array.ToString());
            Debug.WriteLine(json_array.Type);

            var weatherResult = new List<Weather>(); // json에서 넘어온 배열 담을 리스트
            foreach (var val in json_array)
            {
                var WeatherResult = new Weather()
                {
                    BaseDate = Convert.ToInt32(val["baseDate"]),
                    BaseTime = Convert.ToInt32(val["baseTime"]),
                    Category = Convert.ToString(val["category"]),
                    NX = Convert.ToInt32(val["nx"]),
                    NY = Convert.ToInt32(val["ny"]),
                    ObsrValue = Convert.ToDouble(val["obsrValue"])
                };

                weatherResult.Add(WeatherResult);
            }

            /* category로 접근 - obsrValue 값 띄워야함.. category 중 실제로 사용할 것
             PTY(강수형태) : 없음(0), 비(1), 비/눈(2), 눈(3), 빗방울(5) -- 흐림으로 정의 , 빗방울눈날림(6) -- 흐림으로 정의, 눈날림(7) -- 흐림으로 정의
             REH(습도) : 실수값으로 반환
             T1H(기온) : 실수값으로 반환
             WSD(풍속) : 실수값으로 반환 / 4미만 : 바람약함 / 4 ~ 9 : 약간강(안면감촉, 나뭇잎 조금 흔들림) / 9 ~ 14 : 강(나무가지 깃발 가볍게 흔들림) / 14~ : 매우강(먼지 일고 나무 전체 흔들림)
             
             */

            foreach (var weather in weatherResult)
            {
                Debug.WriteLine($"BaseDate: {weather.BaseDate}");
                Debug.WriteLine($"BaseTime: {weather.BaseTime}");
                Debug.WriteLine($"Category: {weather.Category}");
                Debug.WriteLine($"NX: {weather.NX}");
                Debug.WriteLine($"NY: {weather.NY}");
                Debug.WriteLine($"ObsrValue: {weather.ObsrValue}");
                Debug.WriteLine("============");

                if (weather.Category == "PTY") // 이미지 부분 아직 안됨
                {
                    if (weather.ObsrValue == 0)
                    {
                        ImgWeather.Source = new BitmapImage(new Uri("sunny.png", UriKind.Relative));
                    }
                    else if (weather.ObsrValue == 1)
                    {
                        ImgWeather.Source = new BitmapImage(new Uri("rainy.png", UriKind.Relative));
                    }
                    else if (weather.ObsrValue == 2)
                    {
                        ImgWeather.Source = new BitmapImage(new Uri("rainy.png", UriKind.Relative));
                    }
                    else if(weather.ObsrValue == 3)
                    {
                        ImgWeather.Source = new BitmapImage(new Uri("snowy.png", UriKind.Relative));
                    }
                    else if(weather.ObsrValue == 5)
                    {
                        ImgWeather.Source = new BitmapImage(new Uri("cloud.png", UriKind.Relative));
                    }
                    else if(weather.ObsrValue == 6)
                    {
                        ImgWeather.Source = new BitmapImage(new Uri("cloud.png", UriKind.Relative));
                    }
                    else if (weather.ObsrValue == 7)
                    {
                        ImgWeather.Source = new BitmapImage(new Uri("cloud.png", UriKind.Relative));
                    }
                }

                if (weather.Category == "T1H")
                {
                    TxtTemp.Text = $"{weather.ObsrValue} ℃";
                }

                if (weather.Category == "REH")
                {
                    TxtHumid.Text = $"{weather.ObsrValue} %";
                }

                if (weather.Category == "WSD")
                {
                    TxtWind.Text = $"{weather.ObsrValue} m/s";
                }
            }
        }
        #endregion


    }
}
