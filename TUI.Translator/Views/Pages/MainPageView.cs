using Afutrans.Core.ViewModels;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TUI.Translator.Binding;

namespace TUI.Translator.Views.Pages;

/// <summary>
/// The <c>Main</c> page of the TUI: demonstrates the injected <c>IHelloService</c>, a two-way
/// bound text box, a command driven button and a counter.
/// </summary>
public sealed class MainPageView : View
{
    private readonly List<IDisposable> _bindings = [];

    /// <summary>Initializes the main page view.</summary>
    public MainPageView(MainViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

        Width = Dim.Fill();
        Height = Dim.Fill();

        Label title = new() { X = Pos.Center(), Y = 0, Text = viewModel.Title };
        Label description = new() { X = Pos.Center(), Y = 1, Width = Dim.Fill(), Text = viewModel.Description };
        Label greetingCaption = new() { X = 1, Y = 3, Text = viewModel.GreetingCaption };
        Label greeting = new() { X = 3, Y = 4, Text = viewModel.GreetingFromService };
        Label serviceCaption = new() { X = 3, Y = 5, Text = viewModel.ServiceCaption };
        Label nameLabel = new() { X = 1, Y = 7, Text = viewModel.NameLabel };

        // Two-way binding: typing here writes straight into the shared view model.
        NameInput = new TextField { X = 1, Y = 8, Width = 30, Text = viewModel.Name };

        PreviewLabel = new Label { X = 1, Y = 10 };
        HintLabel = new Label { X = 1, Y = 11, Width = Dim.Fill(), Text = viewModel.ClickHint };

        GreetButton = new Button { X = 1, Y = 13, Text = viewModel.ClickButtonLabel, IsDefault = true };
        ResetButton = new Button { X = Pos.Right(GreetButton) + 2, Y = 13, Text = viewModel.ResetLabel };
        CounterLabel = new Label { X = 1, Y = 14 };

        GreetButton.Accepted += (_, _) => viewModel.ClickCommand.Execute(null);
        ResetButton.Accepted += (_, _) => viewModel.ResetCommand.Execute(null);

        _bindings.Add(ViewBinder.TwoWay<string>(NameInput, viewModel, nameof(MainViewModel.Name)));
        _bindings.Add(ViewBinder.OneWay<string>(viewModel, nameof(MainViewModel.GreetingPreview), value => PreviewLabel.Text = value));
        _bindings.Add(ViewBinder.OneWay<string>(viewModel, nameof(MainViewModel.ClickCountText), value => CounterLabel.Text = value));
        _bindings.Add(ViewBinder.OneWay<string>(viewModel, nameof(MainViewModel.Title), value => title.Text = value));
        _bindings.Add(ViewBinder.OneWay<string>(viewModel, nameof(MainViewModel.Description), value => description.Text = value));

        // The reset button follows the command instead of being enabled all the time.
        _bindings.Add(ViewBinder.Command(viewModel.ResetCommand, ResetButton));

        Add(title, description, greetingCaption, greeting, serviceCaption, nameLabel, NameInput, PreviewLabel, HintLabel, GreetButton, ResetButton, CounterLabel);
    }

    /// <summary>Gets the view model this view is bound to.</summary>
    public MainViewModel ViewModel { get; }

    /// <summary>Gets the name input (two-way bound).</summary>
    public TextField NameInput { get; }

    /// <summary>Gets the label that shows the greeting preview.</summary>
    public Label PreviewLabel { get; }

    /// <summary>Gets the demo button.</summary>
    public Button GreetButton { get; }

    /// <summary>Gets the reset button (disabled while the counter is zero).</summary>
    public Button ResetButton { get; }

    /// <summary>Gets the label that shows the click counter.</summary>
    public Label CounterLabel { get; }

    /// <summary>Gets the hint label.</summary>
    public Label HintLabel { get; }

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
