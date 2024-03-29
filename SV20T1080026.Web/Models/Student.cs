﻿namespace SV20T1080026.Web.Models
{
    public class Student
    {
        public string StudentId { get; set; }
        public string StudentName { get; set;}
    }

    public class StudentDAL
    {
        public List<Student> List()
        {
            List<Student> students = new List<Student>();

            students.Add(new Student
            {
                StudentId = "20T1080026",
                StudentName = "Nhat GAY"
            });

            students.Add(new Student
            {
                StudentId = "20T1080027",
                StudentName = "Tu BESO"
            });

            students.Add(new Student
            {
                StudentId = "20T1080028",
                StudentName = "HAU CUTE"
            });

            students.Add(new Student
            {
                StudentId = "20T1080029",
                StudentName = "Chuong DANGiu"
            });


            return students;
        }
    }
}
