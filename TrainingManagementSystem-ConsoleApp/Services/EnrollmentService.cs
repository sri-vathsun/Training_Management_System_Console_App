using TrainingManagementSystem.Models;
using TrainingManagementSystem.Sessions;

namespace TrainingManagementSystem.Services;

public sealed class EnrollmentService
{
    private readonly List<Enrollment> enrollments;
    private readonly LearnerService learnerService;
    private readonly TrainingService trainingService;
    private readonly FeeCalculator feeCalculator;
    private int nextEnrollmentId;

    public EnrollmentService(List<Enrollment> enrollments, LearnerService learnerService,
        TrainingService trainingService, FeeCalculator feeCalculator)
    {
        this.enrollments = enrollments;
        this.learnerService = learnerService;
        this.trainingService = trainingService;
        this.feeCalculator = feeCalculator;
        nextEnrollmentId = 1;
        for (int i = 0; i < enrollments.Count; i++)
        {
            if (enrollments[i].EnrollmentId >= nextEnrollmentId)
            {
                nextEnrollmentId = enrollments[i].EnrollmentId + 1;
            }
        }
    }

    public IReadOnlyList<Enrollment> Enrollments => enrollments;

    public Enrollment? FindById(int enrollmentId)
    {
        for (int i = 0; i < enrollments.Count; i++)
        {
            if (enrollments[i].EnrollmentId == enrollmentId)
            {
                return enrollments[i];
            }
        }

        return null;
    }

    public Enrollment? CreateEnrollment(string learnerId, string courseCode, CourseSession session,
        bool qualifiesForEarlyBird, int eligibleGroupLearnerCount, out string message)
    {
        if (learnerService.FindById(learnerId) is null)
        {
            message = "Learner does not exist.";
            return null;
        }

        Course? course = trainingService.FindByCode(courseCode);
        if (course is null)
        {
            message = "Course does not exist.";
            return null;
        }

        for (int i = 0; i < enrollments.Count; i++)
        {
            Enrollment existing = enrollments[i];
            if (existing.Status == EnrollmentStatus.Active &&
                string.Equals(existing.LearnerId, learnerId, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(existing.CourseCode, courseCode, StringComparison.OrdinalIgnoreCase))
            {
                message = "The learner is already actively enrolled in this course.";
                return null;
            }
        }

        if (course.EnrolledCount >= course.TotalSeats)
        {
            message = "The course has no available seats.";
            return null;
        }

        FeeSummary summary = feeCalculator.CalculateFee(
            course.Fee,
            session.CalculateSessionCharge(),
            qualifiesForEarlyBird,
            eligibleGroupLearnerCount >= 3);

        Enrollment enrollment = new Enrollment(
            nextEnrollmentId++, learnerId, course.CourseCode, DateTime.Now,
            summary.BaseFee, summary.DiscountAmount, FeeCalculator.GetFinalFee(in summary),
            EnrollmentStatus.Active);

        enrollments.Add(enrollment);
        course.EnrolledCount++;
        message = "Enrollment created successfully.";
        return enrollment;
    }

    public bool CancelEnrollment(int enrollmentId, out string message)
    {
        Enrollment? enrollment = FindById(enrollmentId);
        if (enrollment is null)
        {
            message = "Enrollment does not exist.";
            return false;
        }

        if (enrollment.Status == EnrollmentStatus.Cancelled)
        {
            message = "Enrollment is already cancelled.";
            return false;
        }

        Course? course = trainingService.FindByCode(enrollment.CourseCode);
        if (course is null)
        {
            message = "The associated course no longer exists.";
            return false;
        }

        enrollment.Status = EnrollmentStatus.Cancelled;
        if (course.EnrolledCount > 0)
        {
            course.EnrolledCount--;
        }

        message = "Enrollment cancelled successfully and the seat was released.";
        return true;
    }
}
