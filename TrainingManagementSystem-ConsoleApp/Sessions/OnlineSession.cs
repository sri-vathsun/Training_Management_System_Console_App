using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Sessions;

public sealed class OnlineSession : CourseSession
{
    public OnlineSession(string courseCode, DateTime startDate, SessionDuration duration)
        : base(courseCode, startDate, duration)
    {
    }

    public override decimal CalculateSessionCharge() => 200m;
}
