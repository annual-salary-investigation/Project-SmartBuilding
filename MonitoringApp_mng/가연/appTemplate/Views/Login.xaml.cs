using appTemplate.Logics;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace appTemplate.Views
{
    /// <summary>
    /// Login.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Login : MetroWindow
    {
        public Login()
        {
            InitializeComponent();
        }

        private bool isLogined = false; // 로그인 성공했는지 확인하는 변수

        // 아이디 입력 후 엔터키 누르면 비밀번호로 입력창으로 포커스 변경
        private void TxtUserId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) PbPassword.Focus();
        }

        #region < 로그인 프로세스 >
        // 버튼 클릭 이벤트 핸들러
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            isLogined = LoginProcess(); // 실제 로그인 처리는 LoginProcess 메서드에서

            if (isLogined) this.Close(); // 로그인 되면 해당 창 닫고 MainWindow로 진입
        }

        // 패스워드 입력 후 엔터키 눌러도 로그인
        private void PbPassword_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnLogin_Click(sender, e); // 엔터키 누르면 BtnLogin_Click(sender, e); 호출-실행
            }
        }

        // 실제 로그인 처리
        private bool LoginProcess()
        {
            // 검증 사항이 두가지라 한꺼번에 예외 메세지 띄우기 위한 변수
            bool isValid = true;
            string errorMsg = string.Empty;

            if (string.IsNullOrEmpty(TxtUserId.Text))    // 아이디 입력검증
            {
                isValid = false;
                errorMsg += "아이디를 입력하세요.\n";

            }

            if (string.IsNullOrEmpty(PbPassword.Password))   // 비밀번호 입력검증 - PasswordBox는 .Password사용
            {
                isValid = false;
                errorMsg += "비밀번호를 입력하세요.";
            }

            if (isValid == false)
            {
                MessageBox.Show($"{errorMsg}", "오류");
                // this.ShowMessageAsync("오류", $"{errorMsg}", MessageDialogStyle.Affirmative, null);
                // 창이 작아서 우선 ShowMessageAsync 말고 MessageBox 사용
                return false;
            }

            string strMngId = "";
            string strMngPass = "";

            try
            {
                // DB처리
                using (MySqlConnection conn = new MySqlConnection(Commons.MyConnString))
                {
                    conn.Open();

                    string selQuery = @"SELECT MngId
	                                         , MngPass
                                           FROM mngtb
                                          WHERE MngId = @MngId
                                            AND MngPass = @MngPass";
                    MySqlCommand selCmd = new MySqlCommand(selQuery, conn);

                    // @MngId, @MngPass 파라미터 할당
                    MySqlParameter prmMngId = new MySqlParameter("@MngId", TxtUserId.Text);
                    MySqlParameter prmMngPass = new MySqlParameter("@MngPass", PbPassword.Password);
                    selCmd.Parameters.Add(prmMngId);
                    selCmd.Parameters.Add(prmMngPass);

                    MySqlDataReader reader = selCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        strMngId = reader["MngId"] != null ? reader["MngId"].ToString() : "-";
                        strMngPass = reader["MngPass"] != null ? reader["MngPass"].ToString() : "--";
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("로그인 정보가 없습니다.", "오류");
                        // this.ShowMessageAsync("오류", "로그인 정보가 없습니다.", MessageDialogStyle.Affirmative, null);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("오류", $"DB조회 오류 {ex.Message}", MessageDialogStyle.Affirmative, null);
                return false;
            }
        }
        #endregion

        #region < 로그인 안하고 창닫을 때 MainWindow로 진입할 수 없도록 프로그램 종료>
        // 취소버튼 누르면 프로그램 종료
        private void BtnCancle_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        // 로그인 안됐을때 창 닫으면 프로그램 종료
        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            if (isLogined != true)
            {
                Process.GetCurrentProcess().Kill();
            }
        }
        #endregion      
    }
}
