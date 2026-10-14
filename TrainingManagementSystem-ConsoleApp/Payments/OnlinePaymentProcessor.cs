using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Payments;

public sealed class OnlinePaymentProcessor : IPaymentProcessor
{
    public bool ProcessPayment(Payment payment)
    {
        return payment.Amount > 0 && !payment.SimulateFailure;
    }
}
