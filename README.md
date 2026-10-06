# Afutrans

A translation application with **two front ends sharing one core**:

| Project | What it is |
|---|---|
| `Afutrans.Core` | Class library with the **shared view models**, navigation, validation, localization and DI wiring. |
| `Translator` | Desktop GUI ([Avalonia](https://avaloniaui.net/)). |
| `TUI.Translator` | Terminal UI ([Terminal.Gui](https://github.com/tui-cs/Terminal.Gui) v2). |
| `Afutrans.Core.Tests` | Unit tests of the shared core. |
| `TUI.Translator/Tests` | Headless tests of the TUI views and bindings. |

```bash
dotnet build Afutrans.slnx
dotnet test  Afutrans.slnx

dotnet run --project Translator            # GUI
dotnet run --project TUI.Translator        # TUI (Ctrl+Q quits, F2/F3/F4 navigate)
dotnet run --project TUI.Translator -- --culture cs   # TUI in Czech
```

## What the skeleton demonstrates

* **MVVM Toolkit** (`CommunityToolkit.Mvvm`) — `ObservableObject`, `ObservableValidator`,
  `[ObservableProperty]`, `[RelayCommand]` — used only through the shared core.
* **Static shell chrome** — header (title + navigation) and footer are rendered once by the shell;
  navigation swaps **only** the content area (`ContentControl` in Avalonia, the content `View` in
  the TUI). Navigation items come from the shared route table, so adding a page is
  `AddAfutransPage<TPage>("Nav.Key")` + one view.
* **Three pages** — `Main` (injected `IHelloService`, counter command), `Setup` (language picker
  driving the shared `ILocalizer`), `Test` (**reactive registration form**).
* **Reactive validation** — the `Test` page validates on every keystroke: login must be a valid
  e-mail, password needs length + character classes, confirmation must match. The Register button
  is bound to `SubmitCommand.CanExecute` and stays disabled until the form is valid — in both front
  ends, from the same view model.
* **Dependency injection** — `Microsoft.Extensions.DependencyInjection` container built by
  `AddAfutransCore()`; pages are transient and created per navigation in a scope that is disposed
  when you leave the page.
* **Localization-ready** — `ILocalizer` with English (default/fallback) and Czech catalogs; no view
  hard-codes UI text. Views bind to view model properties that refresh when the culture changes.

## Learn more

* Terminal.Gui v2 rules that this repo follows: [`TUI.Translator/AGENTS.md`](./TUI.Translator/AGENTS.md)
* Current work package and open gaps: [`IMPLEMENTATION_PLAN.md`](./IMPLEMENTATION_PLAN.md)
