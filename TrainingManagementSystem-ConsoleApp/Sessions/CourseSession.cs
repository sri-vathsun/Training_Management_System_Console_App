using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Sessions;

public abstract class CourseSession
{
    public string CourseCode { get; }

    public DateTime StartDate { get; }

    public SessionDuration Duration { get; }

    protected CourseSession(string courseCode, DateTime startDate, SessionDuration duration)
    {
        CourseCode = courseCode;
        StartDate = startDate;
        Duration = duration;
    }

    public abstract decimal CalculateSessionCharge();
}
