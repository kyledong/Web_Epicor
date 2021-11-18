using System;
using System.Diagnostics;
using System.IO;

using Web_Epicor.Data.AppJson;

namespace Web_Epicor.Results
{
    public class SuccessfulLog
    {
        public static void SaveFile(string name)
        {

            string date = DateTime.Now.ToString("dd-MM-yyyy");
            string time = DateTime.Now.ToString("HH:mm:ss");
            string route = @"" + LoadJsonData.Route() + "Fecha_" + date + ".txt";


            if (File.Exists(route))
            {
                Debug.WriteLine("Existe");

                using (StreamWriter sw = File.AppendText(route))
                {
                    sw.WriteLine("Hora: " + time + " " + name + " Successful registration.");
                }
            }
            else
            {
                File.WriteAllText(route, "Hora: " + time + " " + name + " Successful registration." + "\r\n");
            }
        }
    }
}
