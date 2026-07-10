using APMC.DataApp;
using System;
using System.Linq;
using System.Net;

namespace APMC.Helpers
{
    public static class SimpleDbLogger
    {
        public static void SetUser(string login) { }

        public static void LogAction(string type, string description, string tableName = null, int? recordId = null)
        {
            try
            {
                var context = ConnectObject.GetConnect();
                var log = new UserLog
                {
                    UserlogID = Session.s_userID,
                    UserName = Session.s_userFirstName,
                    ActionType = type,
                    ActionDescription = description,
                    TableName = tableName,
                    RecordID = recordId,
                    ActionTime = DateTime.Now,
                    Ip= Dns.GetHostEntry(Dns.GetHostName()).AddressList
                        .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString() ?? "127.0.0.1"
                };

                context.UserLogs.Add(log);
                context.SaveChanges();
            }
            catch { /* NoCrashLogger */ }
        }
    }
}