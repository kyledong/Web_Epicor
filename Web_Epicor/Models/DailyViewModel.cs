
using System.ComponentModel.DataAnnotations;


namespace Web_Epicor.Models
{
    public class DailyViewModel
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
        public string password { get; set; }
        [Required]
        public string time { get; set; }        
    }
}
