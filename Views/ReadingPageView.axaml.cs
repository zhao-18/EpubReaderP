using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EpubReaderP.Models;
using EpubReaderP.ViewModels;
using FluentAvalonia.UI.Windowing;

namespace EpubReaderP.Views;

public partial class ReadingPageView : AppWindow
{
    public ReadingPageView()
    {
        InitializeComponent();
        DataContext = this;

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
        Book = new Book();
    }

    public ReadingPageView(Book book)
    {
        InitializeComponent();

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
        Book = book;
        DataContext = new ReadingPageViewModel(book);
    }

    Book Book { get; set; }
}