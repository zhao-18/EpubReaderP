using Avalonia.Platform.Storage;
using EpubReaderP.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EpubReaderP.ViewModels
{
    public class FilePickerViewModel : ViewModelBase
    {
        public FilePickerViewModel()
        {
            _selectFileInteraction = new Interaction<string?, IStorageFile?>();
            SelectFileCommand = ReactiveCommand.CreateFromTask(SelectFile);
        }

        private IStorageFile? _selectedFile;

        public IStorageFile? SelectedFile
        {
            get => _selectedFile;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedFile, value);
                if (value is null)
                {
                    FileName = string.Empty;
                }
                else
                {
                    FileName = PathConverter(value.Path.ToString()) ?? string.Empty;
                }
            }
        }

        private string _fileName = string.Empty;
        public string FileName
        {
            get => _fileName;
            set => this.RaiseAndSetIfChanged(ref _fileName, value);
        }

        private readonly Interaction<string?, IStorageFile?> _selectFileInteraction;

        public Interaction<string?, IStorageFile?> SelectFileInteraction => _selectFileInteraction;

        public ICommand SelectFileCommand { get; }

        private async Task SelectFile()
        {
            SelectedFile = await _selectFileInteraction.Handle("Pick File");
            if (SelectedFile != null)
            {
                InvokeFilePickedEvent();
            }
        }

        public event EventHandler<FilePickedEventArgs>? FilePickedEvent;

        public void InvokeFilePickedEvent()
        {
            if (FilePickedEvent != null)
            {
                FilePickedEvent(this, new FilePickedEventArgs(FileName));
            }
        }

        public static string? PathConverter(string? AvaloniaPath)
        {
            if (AvaloniaPath?.StartsWith("file:///") ?? false)
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    return AvaloniaPath.Substring(8);
                }
                return AvaloniaPath.Substring(7);
            }
            return AvaloniaPath;
        }
    }
}
