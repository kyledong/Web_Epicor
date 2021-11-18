using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace Web_Epicor.Entities
{
    public class BAQ
    {
        public int id { get; set; } 
        public string task_name { get; set; }
        public DateTime last_run { get; set; }
        public string aTrigger { get; set; }
        public string description { get; set; }
        public string program_script { get; set; }
        public string arguments { get; set; }
        public string username { get; set; }
        public bool status_baq { get; set; }
    }
}
