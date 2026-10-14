using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Payments;

public sealed class ClassroomPaymentProcessor : IPaymentProcessor
{
    bool IPaymentProcessor.ProcessPayment(Payment payment)
    {
        return payment.Amount > 0 && !payment.SimulateFailure;
    }
}
