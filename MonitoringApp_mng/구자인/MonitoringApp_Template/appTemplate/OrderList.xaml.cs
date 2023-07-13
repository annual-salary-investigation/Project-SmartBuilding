using MartApp.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System;
using System.Windows.Controls;
using System.Diagnostics;
using appTemplate.Logics;

namespace MartApp
{
    /// <summary>
    /// OrderList.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OrderList : Page
    {
        public OrderList()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            {
                this.DataContext = null;
                List<OrderItem> list = new List<OrderItem>();

                try
                {
                    using (MySqlConnection conn = new MySqlConnection(Commons.MyConnString))
                    {
                        if (conn.State == ConnectionState.Closed) { conn.Open(); }

                        var query = @"SELECT ProductId,
                                             Product,
                                             Price,
                                             Count,
                                             DateTime
                                        FROM paymentdb;";
                        var cmd = new MySqlCommand(query, conn);
                        var adapter = new MySqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds, "paymentdb");

                        foreach (DataRow row in ds.Tables["paymentdb"].Rows)
                        {
                            //var TimeDate = DateTime.
                            list.Add(new OrderItem
                            {
                                Id = Convert.ToString(row["Id"]),
                                Product = Convert.ToString(row["Product"]),
                                Price = Convert.ToInt32(row["Price"]),
                                Count = Convert.ToInt32(row["Count"]),
                                Category = Convert.ToString(row["Category"]),
                                Image = Convert.ToString(row["Image"]),
                                DateTime = Convert.ToDateTime(row["DateTime"]),
                            });
                        }
                        this.DataContext = list;
                        GrdUserInfo.ItemsSource = list; // 이미지 띄움
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine("오류남 오류남");
                }
            }
        }
    }
}
