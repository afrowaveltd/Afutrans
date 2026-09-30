using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Terminal.Gui.ViewBase;

namespace TUI.Translator.Binding;

/// <summary>
/// Minimal binding helpers for Terminal.Gui v2 views.
/// </summary>
/// <remarks>
/// <para>The shared view models raise <see cref="INotifyPropertyChanged"/> and expose
/// <see cref="IRelayCommand"/>; Terminal.Gui views expose values through
/// <see cref="IValue{T}"/>. These helpers connect the two, so the TUI binds to exactly the same
/// view model properties that the Avalonia front end binds to in XAML.</para>
/// <para>The two-way helper uses a re-entrancy guard: pushing a value from the view model into the
/// view must not push it straight back into the view model.</para>
/// </remarks>
public static class ViewBinder
{
    /// <summary>Binds a view model property to a view value in both directions.</summary>
    public static IDisposable TwoWay<TValue>(IValue<TValue> view, INotifyPropertyChanged viewModel, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(viewModel);

        var property = GetProperty(viewModel, propertyName);
        var guard = new ReentrancyGuard();

        void ApplyToView()
        {
            if(guard.IsBusy)
            {
                return;
            }

            using(guard.Enter())
            {
                view.Value = (TValue)property.GetValue(viewModel)!;
            }
        }

        void ApplyToViewModel(TValue value)
        {
            if(guard.IsBusy)
            {
                return;
            }

            using(guard.Enter())
            {
                property.SetValue(viewModel, value);
            }
        }

        PropertyChangedEventHandler onViewModelChanged = (_, e) =>
        {
            if(IsRelevant(e.PropertyName, propertyName))
            {
                ApplyToView();
            }
        };

        EventHandler<Terminal.Gui.App.ValueChangedEventArgs<TValue>> onViewChanged = (_, e) => ApplyToViewModel(e.NewValue);

        viewModel.PropertyChanged += onViewModelChanged;
        view.ValueChanged += onViewChanged;

        ApplyToView();

        return new Subscription(() =>
        {
            viewModel.PropertyChanged -= onViewModelChanged;
            view.ValueChanged -= onViewChanged;
        });
    }

    /// <summary>Binds a view model property to <paramref name="apply"/> (one way: view model to view).</summary>
    public static IDisposable OneWay<TValue>(INotifyPropertyChanged viewModel, string propertyName, Action<TValue> apply)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(apply);

        var property = GetProperty(viewModel, propertyName);

        void Push() => apply((TValue)property.GetValue(viewModel)!);

        PropertyChangedEventHandler onViewModelChanged = (_, e) =>
        {
            if(IsRelevant(e.PropertyName, propertyName))
            {
                Push();
            }
        };

        viewModel.PropertyChanged += onViewModelChanged;

        Push();

        return new Subscription(() => viewModel.PropertyChanged -= onViewModelChanged);
    }

    /// <summary>Keeps a view's <see cref="View.Enabled"/> state in sync with a command's <c>CanExecute</c>.</summary>
    public static IDisposable Command(ICommand command, View control)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(control);

        void Sync() => control.Enabled = command.CanExecute(null);

        EventHandler onCanExecuteChanged = (_, _) => Sync();

        command.CanExecuteChanged += onCanExecuteChanged;

        Sync();

        return new Subscription(() => command.CanExecuteChanged -= onCanExecuteChanged);
    }

    internal static PropertyInfo GetProperty(object instance, string propertyName) =>
        instance.GetType().GetProperty(propertyName)
        ?? throw new InvalidOperationException($"Property '{propertyName}' was not found on '{instance.GetType().FullName}'.");

    private static bool IsRelevant(string? changedProperty, string propertyName) =>
        changedProperty is null || string.Equals(changedProperty, propertyName, StringComparison.Ordinal);

    /// <summary>Guards against the echo a two-way binding produces while one side updates the other.</summary>
    private sealed class ReentrancyGuard
    {
        private int _depth;

        public bool IsBusy => _depth > 0;

        public IDisposable Enter()
        {
            _depth++;

            return new Scope(this);
        }

        private sealed class Scope(ReentrancyGuard owner) : IDisposable
        {
            public void Dispose() => owner._depth--;
        }
    }

    private sealed class Subscription(Action unsubscribe) : IDisposable
    {
        private Action? _unsubscribe = unsubscribe;

        public void Dispose()
        {
            var unsubscribeAction = _unsubscribe;
            _unsubscribe = null;

            unsubscribeAction?.Invoke();
        }
    }
}
