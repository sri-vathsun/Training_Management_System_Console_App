using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Payments;

public interface IPaymentProcessor
{
    bool ProcessPayment(Payment payment);
}
