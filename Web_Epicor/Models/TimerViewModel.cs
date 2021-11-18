using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Epicor.Models
{
    public class TimerViewModel
    {
        [Required]
        public int id { get; set; }
        [Required]
        public string task_name { get; set; }        
        [Required]
        public string aTrigger { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public string program_script { get; set; }
        [Required]
        public string arguments { get; set; }
        [Required]
        public string username { get; set; }       
        [Required]
        public int delay { get; set; }
        [Required]
        public string time_unit { get; set; }
        [Required]
        public string dateTime { get; set; }       
        [Required]
        public string password { get; set; }
    }
}
