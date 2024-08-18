using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using EpubReaderP.Models;
using EpubReaderP.ViewModels;
using FluentAvalonia.UI.Windowing;
using System.Diagnostics;

namespace EpubReaderP.Views;

public partial class ReadingPageView : AppWindow
{
    public ReadingPageView()
    {
        InitializeComponent();
        DataContext = this;

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
    }

    public ReadingPageView(Book book)
    {
        InitializeComponent();

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
        DataContext = new ReadingPageViewModel(book);
    }

    private void SwitchPaneTo0(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not ReadingPageViewModel vm) return;
        vm.CurrentPane = 0;
    }

    private void SwitchPaneTo1(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not ReadingPageViewModel vm) return;
        vm.CurrentPane = 1;
    }
}