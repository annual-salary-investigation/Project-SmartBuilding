using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace appTemplate.Logics
{
    internal class Commons
    {
        public static bool Islogin = false;
        public static bool isManager = false;
        public static string Id = string.Empty;
        // DB 연결 (MySQL)
        public static readonly string MyConnString = "Server=210.119.12.72;" +
                                                     "Port=3306;" +
                                                     "Database=martdb;" +
                                                     "Uid=root;" +
                                                     "Pwd=12345;";
        // 메트로 다이얼로그창을 위한 정적 메서드
        public static async Task<MessageDialogResult> ShowMessageAsync(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            return await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync(title, message, style, null);
        }
    }
}
