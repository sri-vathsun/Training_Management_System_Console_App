namespace TrainingManagementSystem.Models;

public readonly struct SessionDuration : IEquatable<SessionDuration>
{
    public int Days { get; }

    public SessionDuration(int days)
    {
        if (days <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(days), "Duration must be positive.");
        }

        Days = days;
    }

    public bool Equals(SessionDuration other) => Days == other.Days;

    public override bool Equals(object? obj) => obj is SessionDuration other && Equals(other);

    public override int GetHashCode() => Days.GetHashCode();

    public override string ToString() => $"{Days} days";

    public static bool operator ==(SessionDuration left, SessionDuration right) => left.Equals(right);

    public static bool operator !=(SessionDuration left, SessionDuration right) => !left.Equals(right);
}
