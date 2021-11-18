using Microsoft.Win32.TaskScheduler;
using System;
using Web_Epicor.Entities;
using Web_Epicor.Results;

namespace Web_Epicor.TaskScheduler_BAQS
{
    public class Scheduler
    {
        public static bool CreateOrUpdateTaskDaily(string name, string task, string description, string userName, string password, string time, string argument)
        {
            bool rpta;
            try
            {
                using (TaskService ts = new TaskService())
                {             
                    //Cree una nueva definición de tarea y asigne propiedades
                    TaskDefinition td = ts.NewTask();
                    td.RegistrationInfo.Description = description;
                    td.RegistrationInfo.Author = "Scheduler";

                    DailyTrigger tt = new DailyTrigger();

                    DateTime today = DateTime.Today;
                    string date = today.ToString("dd/MM/yyyy") + " " + time;
                    DateTime dat = Convert.ToDateTime(date);
                    tt.StartBoundary = dat;

                    //Crea un disparador que active la tarea
                    td.Triggers.Add(tt);
                    td.Settings.WakeToRun = true;

                    // Crea una acción que lanzará la tarea cada vez que se active el disparador.
                    td.Actions.Add(new ExecAction(task, argument, null));
                    if (ts.HighestSupportedVersion > new Version(1, 2))
                    {
                        td.Principal.LogonType = TaskLogonType.Password;
                        td.Principal.RunLevel = TaskRunLevel.LUA;
                    }

                    // Registre la tarea en la carpeta raíz.
                    string contents = null;
                    if (password != null)
                    {
                        contents = password;
                    }
                    ts.RootFolder.RegisterTaskDefinition(name, td, TaskCreation.CreateOrUpdate,
                        userName, contents, TaskLogonType.Password, null);

                    rpta = true;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.SaveFile("Modificar tarea", ex);
                ErrorLog.SendMail("Modificar tarea", ex);
                Console.WriteLine(ex.Message);
                rpta = false;
            }
            return rpta;
        }
        public static bool CreateOrUpdateTaskTimer(string name, string task, string description, string userName, string password, string dateTime, int delay, string time_unit, string argument)
        {
            bool rpta;
            try
            {
                using (TaskService ts = new TaskService())
                {
                    //Cree una nueva definición de tarea y asigne propiedades
                    TaskDefinition td = ts.NewTask();
                    td.RegistrationInfo.Description = description;
                    td.RegistrationInfo.Author = "Scheduler";

                    DailyTrigger dt = new DailyTrigger();                                      
                    DateTime timer = Convert.ToDateTime(dateTime);
                    dt.StartBoundary = timer;

                    if (time_unit.Equals("Minutes"))
                    {
                        dt.Repetition.Interval = TimeSpan.FromMinutes(delay);
                    }
                    else if (time_unit.Equals("Hours"))
                    {
                        dt.Repetition.Interval = TimeSpan.FromHours(delay);
                    }
                    else if (time_unit.Equals("Days"))
                    {
                        dt.Repetition.Interval = TimeSpan.FromDays(delay);
                    }
                  
                    dt.Repetition.Duration = TimeSpan.FromDays(365);


                    //Crea un disparador que active la tarea
                    td.Triggers.Add(dt);
                    td.Settings.WakeToRun = true;

                    // Crea una acción que lanzará la tarea cada vez que se active el disparador.
                    td.Actions.Add(new ExecAction(task, argument, null));
                    if (ts.HighestSupportedVersion > new Version(1, 2))
                    {
                        td.Principal.LogonType = TaskLogonType.Password;
                        td.Principal.RunLevel = TaskRunLevel.LUA;

                    }

                    // Registre la tarea en la carpeta raíz.
                    string contents = null;
                    if (password != null)
                    {
                        contents = password;
                    }
                    ts.RootFolder.RegisterTaskDefinition(name, td, TaskCreation.CreateOrUpdate,
                        userName, contents, TaskLogonType.Password, null);
                }
                rpta = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                rpta = false;
            }
            return rpta;
        }
    }
}
