using System.Globalization;

namespace TrainingManagementSystem.Utilities;

public static class InputValidator
{
    public static string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string value = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            Console.WriteLine("Value is required.");
        }
    }

    public static bool TryReadInt(string prompt, out int value)
    {
        Console.Write(prompt);
        return int.TryParse(Console.ReadLine(), out value);
    }

    public static bool TryReadDecimal(string prompt, out decimal value)
    {
        Console.Write(prompt);
        return decimal.TryParse(Console.ReadLine(), NumberStyles.Number,
            CultureInfo.CurrentCulture, out value);
    }

    public static bool TryReadLearnerId(out string learnerId)
    {
        Console.Write("Learner ID: ");
        learnerId = (Console.ReadLine() ?? string.Empty).Trim();
        return !string.IsNullOrWhiteSpace(learnerId);
    }

    public static bool TryReadDate(string prompt, out DateTime date)
    {
        Console.Write(prompt);
        return DateTime.TryParse(Console.ReadLine(), out date);
    }

    public static void Increment(ref int count)
    {
        count++;
    }
}
