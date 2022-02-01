using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Course_Planner.Data_Models
{
    [Table("Courses")]
    public class Course
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public int Term { get; set; }
        public string CourseName { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string InstructorName { get; set; }
        public string InstructorPhone { get; set; }
        public string InstructorEmail { get; set; }
        public string Notes { get; set; }
        public int NotificationEnabled { get; set; }
    }
}
