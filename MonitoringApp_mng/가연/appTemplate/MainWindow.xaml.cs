using appTemplate.Views;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            //OpenAPI로 날씨값 받아오기
            #endregion


        }

        #region <로그인 창 로드 영역 - 앱 구현 마지막 단계에 주석 지우고 사용!>
        //private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var loginWindow = new Login();
        //    loginWindow.Owner = this; // LoginWindow의 부모는 MainWindow
        //    loginWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner; // MainWindow의 정중앙에 위치
        //    loginWindow.ShowDialog();  // 모달창
        //}
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
            return;
        }
        #endregion


    }
}
