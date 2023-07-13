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
using System.Windows.Threading;

namespace appTemplate
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow   // 솔루션 패키지에서 MahApp.Metro 삭제 후 다시 설치 필수!!! 
    {
        public ViewModel ViewModel { get; set; }
        private DispatcherTimer _timer;
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new ViewModel();
            DataContext = ViewModel;

            // 더미 데이터를 업데이트하는 타이머 설정
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Tick += Timer_Tick2;

            _timer.Start();
        }
        //습도
        private void Timer_Tick(object sender, EventArgs e)
        {
            // 더미 데이터를 생성하고 ViewModel 속성에 할당
            Random random = new Random();
            double dummyValue1 = random.Next(20, 80); // LvcLivingHumid의 더미 데이터
            double dummyValue2 = random.Next(20, 80); // LvcLivingHumid2의 더미 데이터
            ViewModel.Value1 = dummyValue1;
            ViewModel.Value2 = dummyValue2;
        }
        //온도
        private void Timer_Tick2(object sender, EventArgs e)
        {
            // 더미 데이터를 생성하고 ViewModel 속성에 할당
            Random random = new Random();
            double dummyTempValue = random.Next(20, 50); // LvcTemp의 더미 데이터
            double dummyTemp2Value = random.Next(20, 50); // LvcTemp2의 더미 데이터
            ViewModel.TempValue = dummyTempValue;
            ViewModel.Temp2Value = dummyTemp2Value;
        }

    }
}
