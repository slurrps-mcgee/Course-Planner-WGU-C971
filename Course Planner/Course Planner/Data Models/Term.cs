using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Course_Planner.Data_Models
{
    //Tables
    [Table("Terms")]
    public class Term
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
