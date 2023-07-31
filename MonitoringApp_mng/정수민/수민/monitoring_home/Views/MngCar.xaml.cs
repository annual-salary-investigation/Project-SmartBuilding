using appTemplate.Logics;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace appTemplate.Views
{
    /// <summary>
    /// MngCar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MngCar : MetroWindow
    {
        public MngCar()
        {
            InitializeComponent();
        }

        #region < 데이터그리드에 차량 리스트 DB 연동 -- Loaded 이벤트로 창 열면 바로 조회됨 >
        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 창 실행시 바로 발생하는 이벤트이기 때문에 datacontext 초기화해서 처음에는 안뜨게함
            this.DataContext = null;
            TxtRoomNum.DataContext = null;
            TxtCarNum.DataContext = null;
            TxtPhoneNum.DataContext = null;
            TxtSpecialNote.DataContext = null;

            await ShowCarList(); // 저장 후 DB 자동 재조회 위해서 DB 연동 ShowCarList() 메서드에서 

        }
        #endregion

        #region < 데이터그리드(차량 목록) 텍스트박스에 입혀주기 >
        private void GrdCarlist_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

            foreach (CarList item in GrdCarlist.SelectedItems)
            {
                TxtRoomNum.Text = Convert.ToString(item.RoomNum);
                TxtCarNum.Text = Convert.ToString(item.CarNum);
                TxtPhoneNum.Text = Convert.ToString(item.PhoneNum);
                TxtSpecialNote.Text = Convert.ToString(item.SpecialNote);
            }
        }
        #endregion

        #region < 차량 등록 >
        private async void BtnAddCar_Click(object sender, RoutedEventArgs e)
        {
            int RoomNum = Convert.ToInt32(TxtRoomNum.Text);
            string CarNum = Convert.ToString(TxtCarNum.Text);
            string PhoneNum = Convert.ToString(TxtPhoneNum.Text);
            string SpecialNote = Convert.ToString(TxtSpecialNote.Text);

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.MyConnString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                    // 중복 체크 위해서 SELECT 쿼리 먼저 수행
                    var checkquery = @"SELECT COUNT(*) FROM carmng WHERE CarNum = @CarNum"; // CarNum의 수를 체크
                    MySqlCommand checkcmd = new MySqlCommand(checkquery, conn);
                    checkcmd.Parameters.AddWithValue("@CarNum", CarNum);

                    int checkcount = Convert.ToInt32(checkcmd.ExecuteScalar());

                    if (checkcount > 0) // CarNum의 수가 0이상이면 => 이미 등록된 차량( RoomNum, PhoneNum 등은 중복될 수 있다고 가정)
                    {
                        await this.ShowMessageAsync("오류", "이미 등록된 차량입니다.");
                        return;
                    }

                    // 중복 체크 후 저장
                    var query = @"INSERT INTO carmng
                                             (RoomNum,
                                              CarNum,
                                              PhoneNum,
                                              SpecialNote)
                                         VALUES
                                             (@RoomNum,
                                              @CarNum,
                                              @PhoneNume,
                                              @SpecialNote)";

                    var insRes = 0;

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue(@"RoomNum", RoomNum);
                    cmd.Parameters.AddWithValue(@"CarNum", CarNum);
                    cmd.Parameters.AddWithValue(@"PhoneNume", PhoneNum);
                    cmd.Parameters.AddWithValue(@"SpecialNote", SpecialNote);

                    insRes += cmd.ExecuteNonQuery();

                    if (insRes == 1)
                    {
                        await this.ShowMessageAsync("저장", "DB저장 성공");
                    }
                    else
                    {
                        await this.ShowMessageAsync("저장", "DB저장오류 관리자에게 문의하세요.");
                    }

                    await ShowCarList();
                    
                }

            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"DB저장 오류 {ex.Message}");
            }
        }
        #endregion

        #region < 차량 수정 >
        private async void BtnUpdateCar_Click(object sender, RoutedEventArgs e)
        {
            int RoomNum = Convert.ToInt32(TxtRoomNum.Text);
            string CarNum = Convert.ToString(TxtCarNum.Text);
            string PhoneNum = Convert.ToString(TxtPhoneNum.Text);
            string SpecialNote = Convert.ToString(TxtSpecialNote.Text);

            if (GrdCarlist.SelectedItems.Count == 0)
            {
                await this.ShowMessageAsync("오류", "수정할 차량을 선택하세요", MessageDialogStyle.Affirmative, null);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.MyConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    var query = @"UPDATE carmng
                                     SET 
                                        RoomNum = @RoomNum,
                                        CarNum = @CarNum,
                                        PhoneNum = @PhoneNum,
                                        SpecialNote = @SpecialNote
                                        WHERE Id = @Id;
                                  SELECT * FROM carmng WHERE Id = @Id";
                    var upRes = 0;

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@RoomNum", RoomNum);
                    cmd.Parameters.AddWithValue("@CarNum", CarNum);
                    cmd.Parameters.AddWithValue("@PhoneNum", PhoneNum);
                    cmd.Parameters.AddWithValue("@SpecialNote", SpecialNote);
                    cmd.Parameters.AddWithValue("@Id", ((CarList)GrdCarlist.SelectedItem).Id);

                    upRes = cmd.ExecuteNonQuery();

                    if (upRes == 1)
                    {
                        await this.ShowMessageAsync("수정", "DB 수정 성공!", MessageDialogStyle.Affirmative, null);
                    }

                    await ShowCarList(); // 수정 후 데이터 재조회
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"DB수정 오류 {ex.Message}");
            }
        }
        #endregion

        #region< 차량 삭제 >
        private async void BtnDeleteCar_Click(object sender, RoutedEventArgs e)
        {
            if (GrdCarlist.SelectedItems.Count == 0)
            {
                await this.ShowMessageAsync("오류", "삭제할 차량을 선택하세요", MessageDialogStyle.Affirmative, null);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.MyConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    var query = "DELETE FROM carmng WHERE Id = @Id";
                    var delRes = 0;

                    foreach (CarList item in GrdCarlist.SelectedItems)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", item.Id);

                        delRes += cmd.ExecuteNonQuery();

                    }

                    if (delRes == GrdCarlist.SelectedItems.Count)
                    {
                        await this.ShowMessageAsync("삭제", "DB 삭제 성공!", MessageDialogStyle.Affirmative, null);
                    }
                }

                await ShowCarList(); // 삭제 후 데이터 재조회
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB삭제 오류 {ex.Message}");
            }
        }
        #endregion

        #region < DB 연동 - 데이터그리드에 뿌려주는 메서드 >
        private async Task ShowCarList()
        {
            this.DataContext = null;

            // DB연동
            List<CarList> cars = new List<CarList>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.MyConnString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                    var query = $@"SELECT Id,
                                         RoomNum,
                                         CarNum,
                                         PhoneNum,
                                         SpecialNote
                                    FROM carmng";

                    var cmd = new MySqlCommand(query, conn);
                    var adapter = new MySqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "CarList");

                    foreach (DataRow dr in dSet.Tables["CarList"].Rows)
                    {
                        cars.Add(new CarList
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            RoomNum = Convert.ToInt32(dr["RoomNum"]),
                            CarNum = Convert.ToString(dr["CarNum"]),
                            PhoneNum = Convert.ToString(dr["PhoneNum"]),
                            SpecialNote = Convert.ToString(dr["SpecialNote"])
                        });
                    }
                    this.DataContext = cars; // DataContext에 cars 리스트 뿌려주기

                    // 데이터 조회 후에 TextBox 내용 비워주기
                    TxtRoomNum.Text = String.Empty;
                    TxtCarNum.Text = String.Empty;
                    TxtPhoneNum.Text = String.Empty;
                    TxtSpecialNote.Text = String.Empty;
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"DB조회 오류 {ex.Message}", MessageDialogStyle.Affirmative, null);
            }
        }

        #endregion

        
    }
}
