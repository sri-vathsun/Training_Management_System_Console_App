using TrainingManagementSystem.Models;
using TrainingManagementSystem.Utilities;

namespace TrainingManagementSystem.Services;

public sealed class ReportService
{
    private readonly TrainingService trainingService;
    private readonly EnrollmentService enrollmentService;
    private readonly PaymentService paymentService;

    public ReportService(TrainingService trainingService, EnrollmentService enrollmentService,
        PaymentService paymentService)
    {
        this.trainingService = trainingService;
        this.enrollmentService = enrollmentService;
        this.paymentService = paymentService;
    }

    public void DisplayAllReports()
    {
        DisplayCourseOccupancy();
        DisplayExpectedFees();
        DisplayPaymentSummary();
        DisplayCourseEnrollmentSummary();
    }

    public void DisplayCourseOccupancy()
    {
        Console.WriteLine("\nCourse occupancy");
        for (int i = 0; i < trainingService.Courses.Count; i++)
        {
            Course course = trainingService.Courses[i];
            decimal percentage = course.TotalSeats == 0
                ? 0
                : (decimal)course.EnrolledCount / course.TotalSeats * 100m;
            Console.WriteLine($"{course.CourseCode} - Total: {course.TotalSeats}, " +
                $"Occupied: {course.EnrolledCount}, Available: {course.TotalSeats - course.EnrolledCount}, " +
                $"Occupancy: {percentage:0.00}%");
        }
    }

    public void DisplayExpectedFees()
    {
        decimal total = 0m;
        for (int i = 0; i < enrollmentService.Enrollments.Count; i++)
        {
            Enrollment enrollment = enrollmentService.Enrollments[i];
            if (enrollment.Status == EnrollmentStatus.Active)
            {
                total += enrollment.FinalFee;
            }
        }

        Console.WriteLine($"\nExpected fees from active enrollments: {total:C}");
    }

    public void DisplayPaymentSummary()
    {
        int successfulCount = 0;
        int failedCount = 0;
        decimal collected = 0m;

        for (int i = 0; i < paymentService.Payments.Count; i++)
        {
            Payment payment = paymentService.Payments[i];
            if (payment.Status == PaymentStatus.Successful)
            {
                successfulCount++;
                collected += payment.Amount;
            }
            else if (payment.Status == PaymentStatus.Failed)
            {
                failedCount++;
            }
        }

        Console.WriteLine($"\nPayments - Successful attempts: {successfulCount}, " +
            $"Failed attempts: {failedCount}, Collected: {collected:C}");
    }

    public void DisplayCourseEnrollmentSummary()
    {
        Console.WriteLine("\nCourse enrollment summary");
        for (int i = 0; i < trainingService.Courses.Count; i++)
        {
            Course course = trainingService.Courses[i];
            int activeCount = 0;
            for (int j = 0; j < enrollmentService.Enrollments.Count; j++)
            {
                Enrollment enrollment = enrollmentService.Enrollments[j];
                if (enrollment.Status == EnrollmentStatus.Active &&
                    string.Equals(enrollment.CourseCode, course.CourseCode, StringComparison.OrdinalIgnoreCase))
                {
                    InputValidator.Increment(ref activeCount);
                }
            }

            string fullMessage = course.EnrolledCount >= course.TotalSeats ? " (no seats remaining)" : string.Empty;
            Console.WriteLine($"{course.CourseCode} - Active enrollments: {activeCount}{fullMessage}");
        }
    }
}
