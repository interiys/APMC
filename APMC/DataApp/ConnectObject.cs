using APMC.DataApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace APMC.DataApp {
    public static class ConnectObject
    {
private static dairyqEntities s_connect;
        public static dairyqEntities GetConnect()
        {
             if (s_connect == null)
             {

            s_connect = new dairyqEntities();
             }
                return s_connect;
                    }
        }
    }