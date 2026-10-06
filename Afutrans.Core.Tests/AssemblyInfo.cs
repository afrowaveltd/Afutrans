using Xunit;

// The validation attributes read their localizer from the ambient ValidationLocalizer.Current slot
// (data annotations cannot use constructor injection), so tests must not run in parallel.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
