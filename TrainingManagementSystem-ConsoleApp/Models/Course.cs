namespace TrainingManagementSystem.Models;

public class Course
{
    public string CourseCode { get; set; }

    public string CourseName { get; set; }

    public decimal Fee { get; set; }

    public int DurationDays { get; set; }

    public int TotalSeats { get; set; }

    public int EnrolledCount { get; set; }

    public DateTime StartDate { get; set; }

    public Course(string courseCode, string courseName, decimal fee, int durationDays,
        int totalSeats, int enrolledCount, DateTime startDate)
    {
        CourseCode = courseCode;
        CourseName = courseName;
        Fee = fee;
        DurationDays = durationDays;
        TotalSeats = totalSeats;
        EnrolledCount = enrolledCount;
        StartDate = startDate;
    }
}