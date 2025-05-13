using System.Windows;
using System.Windows.Controls;


namespace MilDB_WPF;

public static class PlaceholderService
{
    // Приєднана властивість для зберігання тексту підказки
    public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.RegisterAttached(
            "Placeholder", // Ім'я властивості
            typeof(object), // Тип значення (object, щоб можна було ставити будь-що, не тільки текст)
            typeof(PlaceholderService), // Тип класу, що реєструє властивість
            new PropertyMetadata(null)); // Метадані властивості (значення за замовчуванням - null)

    // Метод "getter" для приєднаної властивості
    public static object GetPlaceholder(DependencyObject obj)
    {
        return (object)obj.GetValue(PlaceholderProperty);
    }

    // Метод "setter" для приєднаної властивості
    public static void SetPlaceholder(DependencyObject obj, object value)
    {
        obj.SetValue(PlaceholderProperty, value);
    }
}