namespace Afutrans.Core.Navigation;

/// <summary>
/// Implemented by view models that want to know when they become (or stop being) the active page.
/// </summary>
public interface INavigableViewModel
{
    /// <summary>Called after this view model became the current page.</summary>
    void OnNavigatedTo();

    /// <summary>Called before another page replaces this view model.</summary>
    void OnNavigatedFrom();
}
