﻿using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;

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
        // 기상청 API 인증키
        public static readonly string apiKey = "6U1OgOHXoO56%2FqdyFMr%2F5XyU8d6H2iWdFnZmtNuLA%2BhDq3mNlkfOIxbEpgVMVWrU9cb5HM8NAs2iNA0UcXE8ag%3D%3D";

        // 기상청 API 열 때 넣을 현재 날짜 string(yyyy-MM-dd) 타입에서 int(yyyymmdd) 타입으로 변환
        public static readonly string today = DateTime.Today.ToShortDateString();
        public static readonly DateTime parsedToday = DateTime.ParseExact(today, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        public static readonly int convertedToday = int.Parse(parsedToday.ToString("yyyyMMdd"));

        // 기상청 API 열 때 넣을 현재 시간을 HHmm 타입으로 변환
        public static readonly DateTime now = DateTime.Now;
        public static readonly int currentTime = now.Hour * 100 - 70; // 기준 시간에 따른 데이터 못불러오는 문제 해결하기 위해 시간값 *100 후 -70.. 9시 23분에 조회하면 0830으로 들어가게
        public static readonly string formattedTime = currentTime.ToString("0000"); // 이렇게 안하면 12시 08분의 경우 8로 값 들어감 이렇게 해야 0008로 됨

    }
}
