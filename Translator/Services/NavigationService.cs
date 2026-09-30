using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using Translator.Services.Interfaces;
using Translator.ViewModels;

namespace Translator.Services;


/// <summary>
/// Implementation of navigation service for managing view navigation.
/// </summary>
public class NavigationService(IServiceProvider serviceProvider) : INavigationService
{
   private readonly IServiceProvider _serviceProvider = serviceProvider;
   private IServiceScope? _currentScope;

   public ViewModelBase? CurrentViewModel { get; private set; }

   public event EventHandler<AppNavigationEventArgs>? Navigated;

   public void RegisterViewForViewModel<TViewModel, TView>()
       where TViewModel : ViewModelBase
       where TView : UserControl
   {
      // View resolution is done via ViewLocator
   }

   public void NavigateTo<T>() where T : ViewModelBase
   {
      // Dispose the current scope - this also disposes any IDisposable services
      // (including transient ViewModels) that were resolved from it. Do NOT call
      // disposable.Dispose() manually first to avoid double-disposal.
      _currentScope?.Dispose();

      var scope = _serviceProvider.CreateScope();
      var viewModel = scope.ServiceProvider.GetService<T>() ?? Activator.CreateInstance<T>();

      if(viewModel is null)
      {
         scope.Dispose();
         throw new InvalidOperationException($"Unable to create ViewModel instance for type {typeof(T).FullName}.");
      }

      _currentScope = scope;
      CurrentViewModel = viewModel;

      // Notify navigation
      if(viewModel is INavigableViewModel navigableViewModel)
      {
         navigableViewModel.OnNavigatedTo();
      }

      Navigated?.Invoke(this, new AppNavigationEventArgs { ViewModel = viewModel });
   }
}

