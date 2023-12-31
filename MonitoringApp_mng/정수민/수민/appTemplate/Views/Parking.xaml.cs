﻿using appTemplate.Logics;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;


namespace appTemplate.Views
{
    /// <summary>
    /// Parking.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Parking : MetroWindow
    {
        public Parking()
        {
            InitializeComponent();
        }

        #region < Loaded로 바로 조회 >

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await ShowParkingList();
        }

        #endregion

        #region < DB 연동 >

        private async Task ShowParkingList()
        {
            GrdParking.DataContext = null;

            //DB
            List<ParkingList>  cars = new List<ParkingList>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.MyConnString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                    var query = $@"SELECT Number,
                                        CarName,
                                        EntranceTime,
                                        ExitTime,
                                        Fee,
                                        CarId,
                                        IsExit,
                                        Reason
                                    FROM parking";
                    var cmd = new MySqlCommand(query, conn);
                    var adapter = new MySqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "ParkingList");

                    foreach (DataRow dr in dSet.Tables["ParkingList"].Rows)
                    {
                        cars.Add(new ParkingList
                        {
                            Number = Convert.ToInt32(dr["Number"]),
                            CarName = Convert.ToString(dr["CarName"]),
                            EntranceTime = Convert.ToDateTime(dr["EntranceTime"]),
                            CarId = Convert.ToString(dr["CarId"]),
                            IsExit = Convert.ToString(dr["IsExit"])
                        });
                    }
                }
                GrdParking.DataContext = cars;
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"DB조회 오류 {ex.Message}", MessageDialogStyle.Affirmative, null);
            }
        }
        #endregion

        #region < 차량 정보 삭제 >
        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            
            if (GrdParking.SelectedItems.Count == 0) 
            {
                await this.ShowMessageAsync("오류", "삭제할 차량을 선택하세요", MessageDialogStyle.Affirmative, null);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.MyConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    var query = "DELETE FROM parking WHERE Number = @Number";
                    var delRes = 0;

                    foreach (ParkingList item in GrdParking.SelectedItems)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Number", item.Number);

                        delRes += cmd.ExecuteNonQuery();
                    }

                    if (delRes == GrdParking.SelectedItems.Count)
                    {
                        await this.ShowMessageAsync("삭제", "DB 삭제 성공!", MessageDialogStyle.Affirmative, null);
                    }
                }

                await ShowParkingList();
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB삭제 오류 {ex.Message}");
            }
        }
    }

        #endregion
        
}
