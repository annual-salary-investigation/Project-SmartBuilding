using appTemplate.Logics;
using appTemplate.Models;
using appTemplate.Views;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

        #region <메인 창 로드 영역 - 로그인 창 부분은 앱 구현 마지막 단계에 주석 지우고 사용!>
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
            catch
            { 
                await Logics.Commons.ShowMessageAsync("오류", $"오류 발생 : 날씨 정보를 받아올 수 없습니다.");
                // 기상청 API 초단기실황 자체가 생성, 조회되는 기준시간이 있기 때문에 시간이 안맞으면 오류 발생 할 수 있음
                // 1. 24시간 동안만의 결과값을 제공  그 이전 값은 조회 오류 => 현재 날짜로 조회하기 때문에 이 문제는 해당X
                // 2. 기준시간 00시의 값은 00시 30분에 생성되어 제공되기 때문에 새벽 12시~12시 30분 사이에 조회하면 값이 없음
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

        #region < 실제 OpenAPI 불러오는 함수 >
        public async Task CheckWeatehr() 
        {
            // openAPI 요청 uri
            string openApiUri = $"http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtNcst?serviceKey={Commons.apiKey}&numOfRows=10&dataType=JSON&pageNo=1&base_date={Commons.convertedToday}&base_time={Commons.currentTime}&nx=98&ny=74";
            // 하루 동안만의 결과값 제공함!
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
             WSD(풍속) : 실수값으로 반환 / 4미만 : 바람약함 / 4 ~ 8 : 약간강(안면감촉, 나뭇잎 조금 흔들림) / 9 ~ 13 : 강(나무가지 깃발 가볍게 흔들림) / 14~ : 매우강(먼지 일고 나무 전체 흔들림) 
             */

            foreach (var weather in weatherResult)
            {
                switch (weather.Category) // Category 기준으로 화면에 값 띄워줌
                {
                    case "PTY": // 날씨 이미지
                        GetWeatherImagePath(weather.ObsrValue, weather);
                        break;

                    case "T1H": // 기온
                        TxtTemp.Text = $"{weather.ObsrValue} ℃";
                        break;

                    case "REH": // 습도
                        TxtHumid.Text = $"{weather.ObsrValue} %";
                        break;

                    case "WSD": // 풍속
                        TxtWind.Text = $"{weather.ObsrValue} m/s";
                        if (weather.ObsrValue < 4) { Txtalarm.Text = "바람 약함"; }
                        else if (weather.ObsrValue >= 4 && weather.ObsrValue < 9) { Txtalarm.Text = "바람 약간 강함"; }
                        else if (weather.ObsrValue >= 9 && weather.ObsrValue < 14) { Txtalarm.Text = "바람 강함"; }
                        else { Txtalarm.Text = "주의! 바람 매우 강함"; }
                        break;
                }
            }
        }

        private void GetWeatherImagePath(double obsrValue, Weather weather)
        {
            if (weather.Category == "PTY") // 이미지 부분
            {
                if (weather.ObsrValue == 0)
                {
                    ImgWeather.Source = new BitmapImage(new Uri("/Resources/sunny.png", UriKind.Relative));
                }
                else if (obsrValue == 1 || obsrValue == 5 || obsrValue == 6 || obsrValue == 7)
                {
                    ImgWeather.Source = new BitmapImage(new Uri("/Resources/cloud.png", UriKind.Relative));
                }
                else if (weather.ObsrValue == 2)
                {
                    ImgWeather.Source = new BitmapImage(new Uri("/Resources/rainy.png", UriKind.Relative));
                }
                else if (weather.ObsrValue == 3)
                {
                    ImgWeather.Source = new BitmapImage(new Uri("/Resources/snowy.png", UriKind.Relative));
                }
            }
        }
        #endregion


    }
}
