using TrainingManagementSystem.Models;
using TrainingManagementSystem.Payments;
using TrainingManagementSystem.Services;
using TrainingManagementSystem.Sessions;
using TrainingManagementSystem.Utilities;

namespace TrainingManagementSystem;

public sealed class Program
{
    private readonly TrainingService trainingService;
    private readonly LearnerService learnerService;
    private readonly EnrollmentService enrollmentService;
    private readonly PaymentService paymentService;
    private readonly ReportService reportService;

    private Program()
    {
        List<Course> courses = new List<Course>
        {
            new Course("CC1", "Java", 10000m, 50, 20, 0, DateTime.Today.AddDays(30)),
            new Course("CC2", "Python", 20000m, 50, 20, 0, DateTime.Today.AddDays(45)),
            new Course("CC3", ".NET", 15000m, 50, 15, 0, DateTime.Today.AddDays(60)),
            new Course("CC4", "AI", 30000m, 50, 30, 0, DateTime.Today.AddDays(75)),
            new Course("CC5", "C#", 40000m, 50, 40, 0, DateTime.Today.AddDays(90))
        };

        List<Learner> learners = new List<Learner>
        {
            new Learner("L001", "Vathsun", "9898365732", "abc@gmail.com"),
            new Learner("L002", "Suresh", "7478365732", "xyz@gmail.com"),
            new Learner("L003", "Ramesh", "8898657320", "def@gmail.com")
        };

        List<Enrollment> enrollments = new List<Enrollment>();
        List<Payment> payments = new List<Payment>();
        trainingService = new TrainingService(courses);
        learnerService = new LearnerService(learners);
        FeeCalculator feeCalculator = new FeeCalculator();
        enrollmentService = new EnrollmentService(enrollments, learnerService, trainingService, feeCalculator);
        paymentService = new PaymentService(payments, enrollmentService,
            new OnlinePaymentProcessor(), new ClassroomPaymentProcessor());
        reportService = new ReportService(trainingService, enrollmentService, paymentService);
    }

    public static void Main()
    {
        try
        {
            new Program().Run();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Unexpected error: {exception.Message}");
        }
        finally
        {
            Console.WriteLine("Application closed.");
        }
    }

    private void Run()
    {
        bool running = true;
        while (running)
        {
            DisplayMenu();
            int choice = MenuHandler.ReadMenuChoice();
            Console.WriteLine();

            switch (choice)
            {
                case 1: DisplayCourses(trainingService.Courses); break;
                case 2: SearchCourses(); break;
                case 3: SortCourses(); break;
                case 4: RegisterLearner(); break;
                case 5: DisplayLearners(); break;
                case 6: SearchLearner(); break;
                case 7: EnrollLearner(); break;
                case 8: DisplayEnrollments(); break;
                case 9: CancelEnrollment(); break;
                case 10: DisplayFeeBreakdown(); break;
                case 11: ProcessPayment(); break;
                case 12: DisplayPaymentHistory(); break;
                case 13: reportService.DisplayAllReports(); break;
                case 14: DemonstrateRecordsAndSessions(); break;
                case 0: running = false; break;
                default: Console.WriteLine("Invalid menu choice."); break;
            }

            if (running)
            {
                Console.WriteLine("\nPress Enter to continue.");
                Console.ReadLine();
            }
        }
    }

    private static void DisplayMenu()
    {
        Console.Clear();
        Console.WriteLine("Training Management System");
        Console.WriteLine("1. List all courses");
        Console.WriteLine("2. Search courses");
        Console.WriteLine("3. Sort courses by fee");
        Console.WriteLine("4. Register a learner");
        Console.WriteLine("5. List learners");
        Console.WriteLine("6. Search for a learner");
        Console.WriteLine("7. Enroll in a course");
        Console.WriteLine("8. View enrollment records");
        Console.WriteLine("9. Cancel an enrollment");
        Console.WriteLine("10. Display a fee breakdown");
        Console.WriteLine("11. Process a simulated payment");
        Console.WriteLine("12. View payment history");
        Console.WriteLine("13. Display reports");
        Console.WriteLine("14. Demonstrate records and sessions");
        Console.WriteLine("0. Exit");
    }

    private static void DisplayCourses(IReadOnlyList<Course> courses)
    {
        Console.WriteLine("{0,-8} {1,-14} {2,12} {3,10} {4,10} {5,14}",
            "Code", "Title", "Fee", "Seats", "Available", "Start date");
        Console.WriteLine(new string('-', 78));
        MenuHandler.DisplayItems(courses, course => Console.WriteLine(
            $"{course.CourseCode,-8} {course.CourseName,-14} {course.Fee,12:C} " +
            $"{course.TotalSeats,10} {course.TotalSeats - course.EnrolledCount,10} " +
            $"{course.StartDate:yyyy-MM-dd}"));
    }

    private void SearchCourses()
    {
        string searchText = InputValidator.ReadRequired("Enter course code or title: ");
        List<Course> results = trainingService.Search(searchText);
        if (results.Count == 0)
        {
            Console.WriteLine("No course found.");
            return;
        }

        DisplayCourses(results);
    }

    private void SortCourses()
    {
        string order = InputValidator.ReadRequired("Ascending or descending (A/D): ");
        trainingService.SortByFee(!string.Equals(order, "D", StringComparison.OrdinalIgnoreCase));
        DisplayCourses(trainingService.Courses);
    }

    private void RegisterLearner()
    {
        string learnerId = ReadUniqueLearnerId();
        string name = InputValidator.ReadRequired("Name: ");
        string phone = ReadPhoneNumber();
        string email = InputValidator.ReadRequired("Email: ");

        if (learnerService.Register(new Learner(learnerId, name, phone, email)))
        {
            Console.WriteLine("Learner registered successfully.");
        }
        else
        {
            Console.WriteLine("Learner ID already exists.");
        }
    }

    private string ReadUniqueLearnerId()
    {
        while (true)
        {
            string learnerId = InputValidator.ReadRequired("Learner ID: ");
            if (learnerService.FindById(learnerId) is null)
            {
                return learnerId;
            }

            Console.WriteLine("Duplicate learner ID. Please enter a unique ID.");
        }
    }

    private static string ReadPhoneNumber()
    {
        while (true)
        {
            string phone = InputValidator.ReadRequired("Contact number: ");
            if (phone.Length == 10)
            {
                bool allDigits = true;
                for (int i = 0; i < phone.Length; i++)
                {
                    if (!char.IsDigit(phone[i]))
                    {
                        allDigits = false;
                        break;
                    }
                }

                if (allDigits)
                {
                    return phone;
                }
            }

            Console.WriteLine("Contact number must contain exactly 10 digits.");
        }
    }

    private void DisplayLearners()
    {
        MenuHandler.DisplayItems(learnerService.Learners, learner =>
            Console.WriteLine($"{learner.LearnerId} | {learner.Name} | {learner.ContactNumber} | {learner.Email}"));
    }

    private void SearchLearner()
    {
        string searchText = InputValidator.ReadRequired("Enter learner ID or name: ");
        Learner? learner = MenuHandler.FindByText(learnerService.Learners,
            item => $"{item.LearnerId} {item.Name}", searchText);
        Console.WriteLine(learner is null
            ? "Learner not found."
            : $"{learner.LearnerId} | {learner.Name} | {learner.ContactNumber} | {learner.Email}");
    }

    private void EnrollLearner()
    {
        if (!InputValidator.TryReadLearnerId(out string learnerId) ||
            learnerService.FindById(learnerId) is null)
        {
            Console.WriteLine("Learner does not exist or the ID is invalid.");
            return;
        }

        string courseCode = InputValidator.ReadRequired("Course code: ");
        Course? course = trainingService.FindByCode(courseCode);
        if (course is null)
        {
            Console.WriteLine("Course does not exist.");
            return;
        }

        if (!InputValidator.TryReadDate("Session start date (yyyy-MM-dd): ", out DateTime startDate))
        {
            Console.WriteLine("Invalid date.");
            return;
        }

        if (!InputValidator.TryReadInt("Session duration in days: ", out int durationDays) || durationDays <= 0)
        {
            Console.WriteLine("Duration must be a positive number.");
            return;
        }

        string sessionType = InputValidator.ReadRequired("Session type (O for online, C for classroom): ");
        CourseSession session = string.Equals(sessionType, "O", StringComparison.OrdinalIgnoreCase)
            ? new OnlineSession(course.CourseCode, startDate, new SessionDuration(durationDays))
            : new ClassroomSession(course.CourseCode, startDate, new SessionDuration(durationDays));

        bool earlyBird = (course.StartDate - DateTime.Today).TotalDays >= 14;
        if (!InputValidator.TryReadInt("Number of eligible learners enrolling together: ", out int groupCount) || groupCount < 1)
        {
            Console.WriteLine("Group count must be positive.");
            return;
        }

        Enrollment? enrollment = enrollmentService.CreateEnrollment(learnerId, course.CourseCode,
            session, earlyBird, groupCount, out string message);
        Console.WriteLine(message);
        if (enrollment is not null)
        {
            Console.WriteLine($"Enrollment ID: {enrollment.EnrollmentId}");
            Console.WriteLine($"Fee: {enrollment.FinalFee:C} (base {enrollment.BaseFee:C}, discount {enrollment.DiscountAmount:C})");
        }
    }

    private void DisplayEnrollments()
    {
        if (enrollmentService.Enrollments.Count == 0)
        {
            Console.WriteLine("No enrollment records found.");
            return;
        }

        for (int i = 0; i < enrollmentService.Enrollments.Count; i++)
        {
            Enrollment enrollment = enrollmentService.Enrollments[i];
            Console.WriteLine($"{enrollment.EnrollmentId} | Learner: {enrollment.LearnerId} | " +
                $"Course: {enrollment.CourseCode} | Fee: {enrollment.FinalFee:C} | " +
                $"Status: {enrollment.Status} | Paid: {enrollment.PaymentCompleted}");
        }
    }

    private void CancelEnrollment()
    {
        if (!InputValidator.TryReadInt("Enrollment ID: ", out int enrollmentId))
        {
            Console.WriteLine("Invalid enrollment ID.");
            return;
        }

        enrollmentService.CancelEnrollment(enrollmentId, out string message);
        Console.WriteLine(message);
    }

    private void DisplayFeeBreakdown()
    {
        if (!InputValidator.TryReadInt("Enrollment ID: ", out int enrollmentId))
        {
            Console.WriteLine("Invalid enrollment ID.");
            return;
        }

        Enrollment? enrollment = enrollmentService.FindById(enrollmentId);
        if (enrollment is null)
        {
            Console.WriteLine("Enrollment does not exist.");
            return;
        }

        Console.WriteLine($"Base fee: {enrollment.BaseFee:C}");
        Console.WriteLine($"Total discount: {enrollment.DiscountAmount:C}");
        Console.WriteLine($"Final fee: {enrollment.FinalFee:C}");
    }

    private void ProcessPayment()
    {
        if (!InputValidator.TryReadInt("Enrollment ID: ", out int enrollmentId))
        {
            Console.WriteLine("Invalid enrollment ID.");
            return;
        }

        string type = InputValidator.ReadRequired("Payment type (O for online, C for classroom): ");
        string outcome = InputValidator.ReadRequired("Simulate success or failure (S/F): ");
        Payment? payment = paymentService.ProcessPayment(enrollmentId,
            string.Equals(type, "O", StringComparison.OrdinalIgnoreCase),
            string.Equals(outcome, "F", StringComparison.OrdinalIgnoreCase), out string message);
        Console.WriteLine(message);
        if (payment is not null)
        {
            Console.WriteLine($"Payment {payment.PaymentId}: {payment.Status}, amount {payment.Amount:C}");
        }
    }

    private void DisplayPaymentHistory()
    {
        if (paymentService.Payments.Count == 0)
        {
            Console.WriteLine("No payment attempts found.");
            return;
        }

        for (int i = 0; i < paymentService.Payments.Count; i++)
        {
            Payment payment = paymentService.Payments[i];
            Console.WriteLine($"{payment.PaymentId} | Enrollment: {payment.EnrollmentId} | " +
                $"Amount: {payment.Amount:C} | {payment.PaymentDate:g} | {payment.Status}");
        }
    }

    private void DemonstrateRecordsAndSessions()
    {
        Course course = trainingService.Courses[0];
        CourseSummary first = new CourseSummary(course.CourseCode, course.CourseName, course.Fee,
            course.TotalSeats - course.EnrolledCount);
        CourseSummary second = new CourseSummary(course.CourseCode, course.CourseName, course.Fee,
            course.TotalSeats - course.EnrolledCount);
        CourseSummary modified = first with { AvailableSeats = first.AvailableSeats + 1 };
        CourseSession session = new OnlineSession(course.CourseCode, course.StartDate,
            new SessionDuration(course.DurationDays));

        Console.WriteLine($"Equal summaries: {first == second}");
        Console.WriteLine($"Modified summary available seats: {modified.AvailableSeats}");
        Console.WriteLine($"Session duration: {session.Duration}");
        Console.WriteLine($"Polymorphic session charge: {session.CalculateSessionCharge():C}");
    }
}
