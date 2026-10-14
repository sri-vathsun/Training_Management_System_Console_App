using TrainingManagementSystem.Models;
using TrainingManagementSystem.Payments;

namespace TrainingManagementSystem.Services;

public sealed class PaymentService
{
    private readonly List<Payment> payments;
    private readonly EnrollmentService enrollmentService;
    private readonly IPaymentProcessor onlineProcessor;
    private readonly IPaymentProcessor classroomProcessor;
    private int nextPaymentId;

    public PaymentService(List<Payment> payments, EnrollmentService enrollmentService,
        IPaymentProcessor onlineProcessor, IPaymentProcessor classroomProcessor)
    {
        this.payments = payments;
        this.enrollmentService = enrollmentService;
        this.onlineProcessor = onlineProcessor;
        this.classroomProcessor = classroomProcessor;
        nextPaymentId = 1;
    }

    public IReadOnlyList<Payment> Payments => payments;

    public Payment? ProcessPayment(int enrollmentId, bool online, bool simulateFailure, out string message)
    {
        Enrollment? enrollment = enrollmentService.FindById(enrollmentId);
        if (enrollment is null)
        {
            message = "Enrollment does not exist.";
            return null;
        }

        if (enrollment.Status == EnrollmentStatus.Cancelled)
        {
            message = "Payment cannot be processed for a cancelled enrollment.";
            return null;
        }

        Payment payment = new Payment(nextPaymentId++, enrollmentId, enrollment.FinalFee,
            DateTime.Now, PaymentStatus.Pending, simulateFailure);
        IPaymentProcessor processor = online ? onlineProcessor : classroomProcessor;
        bool successful = processor.ProcessPayment(payment);
        payment.Status = successful ? PaymentStatus.Successful : PaymentStatus.Failed;
        payments.Add(payment);

        if (successful)
        {
            enrollment.PaymentCompleted = true;
            message = "Payment succeeded.";
        }
        else
        {
            message = "Payment failed. The attempt was recorded.";
        }

        return payment;
    }
}
