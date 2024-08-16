using Avalonia.Input;
using EpubReaderP.Models;
using EpubReaderP.ViewModels;
using FluentAvalonia.UI.Windowing;

namespace EpubReaderP.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        InitializeComponent();

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
    }

    private void OnBookDoubleClicked(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount >= 2)
        {
            if (DataContext is not MainWindowViewModel vm) return;

            if (BookList.SelectedItem is not Book SelectedBook) return;
            ReadingPageView ReadingWindow = new ReadingPageView(SelectedBook);

            ReadingWindow.Show(this);
        }
    }
}
