using System;

namespace Web_Epicor.Entities
{
    public class Error
    {
        public int id { get; set; }
        public string part_num { get; set; }
        public string record { get; set; }
        public string name_table { get; set; }
        public string error { get; set; }
        public DateTime run_at { get; set; }
        public string query { get; set; }
    }
}
