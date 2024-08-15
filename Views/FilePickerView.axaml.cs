using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using EpubReaderP.ViewModels;
using ReactiveUI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EpubReaderP.Views;

public partial class FilePickerView : ReactiveUserControl<FilePickerViewModel>
{
    public FilePickerView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            d(ViewModel!.SelectFileInteraction.RegisterHandler(FilePickerHandler));
        });
    }

    private async Task FilePickerHandler(InteractionContext<string?, IStorageFile?> context)
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(this);

        IReadOnlyList<IStorageFile>? storageFile = await topLevel!.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions()
            {
                AllowMultiple = false,
                Title = context.Input,
                FileTypeFilter = new[] { EpubFileFilter }
            }
        );

        if (storageFile is null || storageFile.Count != 1)
        {
            context.SetOutput(default(IStorageFile));
        }
        else
        {
            context.SetOutput(storageFile[0]);
        }
    }

    public static FilePickerFileType EpubFileFilter { get; } = new("Epub Files")
    {
        Patterns = new[] { "*.epub" },
        AppleUniformTypeIdentifiers = new[] { "com.apple.ibooks.epub" },
        MimeTypes = new[] { "application/epub+zip" }
    };
}