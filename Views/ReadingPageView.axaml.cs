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
        //Book = new Book();
    }

    public ReadingPageView(Book book)
    {
        InitializeComponent();

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
        //Book = book;
        DataContext = new ReadingPageViewModel(book);
    }

    //Book Book { get; set; }

    private void SwitchPaneTo0(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not ReadingPageViewModel vm) return;
        vm.CurrentPane = 0;

        Debug.WriteLine("Pane0");
    }

    private void SwitchPaneTo1(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not ReadingPageViewModel vm) return;
        vm.CurrentPane = 1;
        Debug.WriteLine("Pane1");
    }
}