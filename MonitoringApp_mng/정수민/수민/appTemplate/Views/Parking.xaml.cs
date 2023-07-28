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
    /// Parking.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Parking : MetroWindow
    {
        public Parking()
        {
            InitializeComponent();
        }

        #region < DB 연동 -- Loaded 이벤트로 창 열면 바로 조회 >

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            private async Task ShowParking()
            {
                this.DataContext = null;

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

                        foreach (DataRow dr in dSet.Tables["Parking"].Rows)
                        {
                            cars.Add(new ParkingList
                            {
                                CarId = Con
                            });
                        }
                    }
                }
            }
        }

        #endregion
        private void GrdParking_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private void GrdParking_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
