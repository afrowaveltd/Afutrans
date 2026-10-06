using Afutrans.Core.ViewModels;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TUI.Translator.Binding;

namespace TUI.Translator.Views.Pages;

/// <summary>
/// The <c>Setup</c> page of the TUI: a reactive text box and the language picker that drives the
/// shared <see cref="Afutrans.Core.Localization.ILocalizer"/>.
/// </summary>
public sealed class SetupPageView : View
{
    private readonly List<IDisposable> _bindings = [];

    /// <summary>Initializes the setup page view.</summary>
    public SetupPageView(SetupViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

        Width = Dim.Fill();
        Height = Dim.Fill();

        Label title = new() { X = 1, Y = 2, Text = viewModel.Title };
        Label description = new() { X = 1, Y = 3, Width = Dim.Fill(), Text = viewModel.Description };
        Label nameLabel = new() { X = 1, Y = 4, Text = viewModel.NameLabel };

        NameInput = new TextField { X = 1, Y = 6, Width = 40, Text = viewModel.UserName };

        Label previewCaption = new() { X = 1, Y = 7, Text = viewModel.PreviewLabel };
        PreviewLabel = new Label { X = 3, Y = 8 };
        HintLabel = new Label { X = 1, Y = 9, Width = Dim.Fill(), Text = viewModel.NameHint };

        Label languageLabel = new() { X = 1, Y = 10, Text = viewModel.LanguageLabel };

        LanguageList = new ListView
        {
            X = 1,
            Y = 11,
            Width = 30,
            Height = 4,
        };

        LanguageList.SetSource(new System.Collections.ObjectModel.ObservableCollection<LanguageOption>(viewModel.LanguageOptions));

        if(viewModel.SelectedLanguage is not null)
        {
            LanguageList.SelectedItem = viewModel.LanguageOptions.ToList().IndexOf(viewModel.SelectedLanguage);
        }

        LanguageList.Accepted += (_, _) =>
        {
            var selected = LanguageList.SelectedItem;

            if(selected is >= 0 && selected < viewModel.LanguageOptions.Count)
            {
                viewModel.SelectedLanguage = viewModel.LanguageOptions[selected.Value];
            }
        };

        CurrentLanguageLabel = new Label { X = 1, Y = 16, Width = Dim.Fill(), Text = viewModel.CurrentLanguageText };

        _bindings.Add(ViewBinder.TwoWay<string>(NameInput, viewModel, nameof(SetupViewModel.UserName)));
        _bindings.Add(ViewBinder.OneWay<string>(viewModel, nameof(SetupViewModel.GreetingPreview), value => PreviewLabel.Text = value));
        _bindings.Add(ViewBinder.OneWay<string>(viewModel, nameof(SetupViewModel.CurrentLanguageText), value => CurrentLanguageLabel.Text = value));
        _bindings.Add(ViewBinder.OneWay<string>(viewModel, nameof(SetupViewModel.Title), value => title.Text = value));
        _bindings.Add(ViewBinder.OneWay<string>(viewModel, nameof(SetupViewModel.Description), value => description.Text = value));

        Add(title, description, nameLabel, NameInput, previewCaption, PreviewLabel, HintLabel, languageLabel, LanguageList, CurrentLanguageLabel);
    }

    /// <summary>Gets the view model this view is bound to.</summary>
    public SetupViewModel ViewModel { get; }

    /// <summary>Gets the reactive name input.</summary>
    public TextField NameInput { get; }

    /// <summary>Gets the greeting preview label.</summary>
    public Label PreviewLabel { get; }

    /// <summary>Gets the hint label.</summary>
    public Label HintLabel { get; }

    /// <summary>Gets the language picker.</summary>
    public ListView LanguageList { get; }

    /// <summary>Gets the label showing the active language.</summary>
    public Label CurrentLanguageLabel { get; }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            foreach(var binding in _bindings)
            {
                binding.Dispose();
            }

            _bindings.Clear();
        }

        base.Dispose(disposing);
    }
}
