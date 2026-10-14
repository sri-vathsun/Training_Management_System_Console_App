namespace TrainingManagementSystem.Utilities;

public static class MenuHandler
{
    public static void DisplayItems<T>(IReadOnlyList<T> items, Action<T> display)
    {
        for (int i = 0; i < items.Count; i++)
        {
            display(items[i]);
        }
    }

    public static T? FindByText<T>(IReadOnlyList<T> items, Func<T, string> textSelector, string searchText)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (textSelector(items[i]).Contains(searchText, StringComparison.OrdinalIgnoreCase))
            {
                return items[i];
            }
        }

        return default;
    }

    public static int ReadMenuChoice()
    {
        Console.Write("Choose an option: ");
        return int.TryParse(Console.ReadLine(), out int choice) ? choice : -1;
    }
}
