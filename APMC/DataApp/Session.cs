using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace APMC.DataApp
{
    public static class Session
    {

        public static User CurrentUser { get; set; }
        public static int s_userID;
        public static string s_userFirstName;
        public static string s_userPatronymic;

        public static int s_fas;
        public static int s_str;
        public static int s_bet;
        public static int s_acc;
    }
}