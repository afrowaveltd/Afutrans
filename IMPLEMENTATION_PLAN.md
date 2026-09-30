# Afutrans — Implementation Plan

> **Source of truth** for the current work package. Keep it up to date after every step.

## 1. Goal (from the user request)

Both applications (`Translator` = Avalonia GUI, `TUI.Translator` = Terminal.Gui console app) must get
a *learning skeleton* before real translation data is added:

1. **MVVM Toolkit** (CommunityToolkit.Mvvm) in both apps.
2. **Navigation tooling** so that **header and footer stay static** while only the content view swaps.
3. **Three pages**: `Main`, `Setup`, `Test`.
   * `Test` = small registration form (login = valid e-mail, password = length + complexity),
     **validated on every change**, submit button **disabled until the form is valid** —
     a hands-on binding/validation demo.
4. **Dependency injection** infrastructure with one practice service (`IHelloService`) used in a view.
5. **Shared class library** with the ViewModels, shared by the GUI *and* the console app.
6. **Localization-ready** (English is the default language everywhere), all code/comments/UI in English.

## 2. Architecture

```
Afutrans.slnx
├── Afutrans.Core              (new, net10.0 class library — SHARED)
│   ├── Localization/          ILocalizer, Localizer, StringKeys, EnglishStrings
│   ├── Navigation/            NavigationRoute, INavigationService, NavigationService, INavigableViewModel
│   ├── Services/              IHelloService, HelloService
│   ├── Validation/            PasswordRules, PasswordComplexityAttribute, PasswordConfirmationAttribute, LocalizedMessage
│   ├── ViewModels/            ViewModelBase, PageViewModelBase, ShellViewModel, NavigationItemViewModel,
│   │                          MainViewModel, SetupViewModel, TestViewModel
│   └── DependencyInjection/   ServiceCollectionExtensions.AddAfutransCore()
├── Translator                 (Avalonia GUI — consumes Afutrans.Core)
│   ├── Views/MainWindow.axaml (shell: static header + ContentControl + static footer)
│   ├── Views/MainView/SetupView/TestView (UserControls, resolved by ViewLocator)
│   ├── ViewLocator.cs         (VM type → View type registry, built in App.axaml.cs)
│   └── Tests/Translator.Tests (Avalonia headless tests)
└── TUI.Translator             (Terminal.Gui v2 — consumes Afutrans.Core)
    ├── Views/ShellWindow.cs   (static header + swappable content + static status bar footer)
    ├── Views/Pages/*.cs       (MainPageView, SetupPageView, TestPageView)
    ├── Binding/Bind.cs        (tiny one-way/two-way binding helper for Terminal.Gui views)
    ├── Navigation/PageViewFactory.cs
    └── Tests/                 (headless view tests)
```

Key decisions
* **Shared ViewModels, thin views.** All state, commands, validation and navigation live in `Afutrans.Core`
  so GUI and TUI behave identically. Views only render + forward input.
* **Navigation** is a route table (`NavigationRoute`: key + VM type + localized title key) owned by
  `INavigationService`. `ShellViewModel` exposes the route list as `NavigationItems` so a front end can
  render its nav strip without knowing the pages.
* **Static chrome.** The header/footer views are created once by the shell and are *never* part of the
  swap. Only the content host swaps (`ContentControl` in Avalonia, a content `View` in the TUI).
* **DI** is a normal `Microsoft.Extensions.DependencyInjection` container built in each app entry point
  from a shared `AddAfutransCore()` extension. Pages are transient and resolved from a per-navigation
  scope (disposed automatically on the next navigation).
* **Localization** = `ILocalizer` with per-culture catalogs, English as default + fallback. Views never
  hard-code UI text; they bind to `Title`/`Error`/… properties that re-raise `PropertyChanged` when the
  culture changes.
* **Binding in the TUI** is provided by a small `Bind` helper (one-way + two-way) that mirrors what
  Avalonia does in XAML, so both front ends bind to the *same* VM properties.

## 3. Steps & progress

| # | Step | Status |
|---|------|--------|
| 0 | Recon: inspect solution, projects, Terminal.Gui v2 API (`IValue<T>`, `Command.Accept`, drivers) | ✅ done |
| 1 | Write this plan | ✅ done |
| 2 | Create `Afutrans.Core` class library (MVVM Toolkit + DI abstractions) | ✅ done |
| 3 | Localization layer (`ILocalizer`, English + Czech catalogs, string keys) | ✅ done |
| 4 | Navigation tooling (`NavigationService`, route table, `INavigableViewModel`) | ✅ done |
| 5 | `IHelloService` + `AddAfutransCore()` / `AddAfutransPage<T>()` DI extensions | ✅ done |
| 6 | Shared ViewModels: Shell, NavigationItem, Main, Setup, Test (+ validation rules) | ✅ done |
| 7 | `Afutrans.Core.Tests` — 38 tests for VMs, validation, navigation, localization, DI | ✅ done (38/38) |
| 8 | TUI: DI bootstrap, `ShellWindow` with static header/footer, 3 page views, `ViewBinder` | ✅ done |
| 9 | TUI tests (nav swaps only content, binding updates VM, submit gating, language switch) | ✅ done (6/6) |
| 10 | GUI: DI bootstrap, shell layout in `MainWindow.axaml`, 3 page views, `ViewLocator` registry | ✅ done |
| 11 | GUI tests (`Avalonia.Headless`) — shell/static chrome + navigation + form gating | ⛔ **not done** (see §6) |
| 12 | Solution wiring, full `dotnet build` + `dotnet test` | ✅ done (0 warnings, 0 errors, 44/44 tests) |
| 13 | README / AGENTS touch-up for the new architecture | ⏳ next step |

### Build / test evidence (last run)

```text
dotnet build Afutrans.slnx   ->  succeeded, 0 warnings, 0 errors
Afutrans.Core.Tests          ->  Passed! 38/38
TUI.Translator.Tests         ->  Passed!  6/6
```

## 4. Verification

```bash
dotnet build Afutrans.slnx
dotnet test  Afutrans.slnx
dotnet run --project TUI.Translator        # Esc / Ctrl+Q quits, F2/F3/F4 navigate
dotnet run --project Translator            # Avalonia GUI
```

* A clean build is **not** proof of UI correctness → GUI is verified with Avalonia headless tests,
  TUI with in-process Terminal.Gui view tests (no PTY needed for logic).
* Every claim in section 1 has at least one automated test (see step 7/9/11).

## 6. Known gaps / next steps

1. **GUI (Avalonia) has no automated UI tests yet** — step 11 was not delivered in this pass.
   Add a `Translator.Tests` project referencing `Avalonia.Headless.XUnit` 12.1.3 (already on NuGet)
   and assert: header/footer stay identical after `NavigateCommand.Execute(...)`, the
   `ContentControl.Content` type follows the route, and `SubmitCommand.CanExecute` gates the
   `Button` in `TestView`. Until then, GUI behaviour is only verified by the shared-core tests
   plus a manual `dotnet run --project Translator`.
2. **Visual verification of both front ends** was not possible in this environment (no PTY recorder
   `tuirec`, no display). Run `dotnet run --project TUI.Translator` and
   `dotnet run --project Translator` locally to eyeball the layout.
3. `MainWindow.axaml` has no `Design.DataContext` because `ShellViewModel` intentionally has no
   parameterless constructor (it is DI-built). Use the Avalonia designer preview only after adding
   a design-time factory if that becomes annoying.
4. Localization ships with `en` + `cs`. Adding a language = one catalog class + one line in
   `Localizer.CreateDefault()`.
5. `TUI.Translator/Tests` currently has `Terminal.Gui 2.4.17` + project reference only; the
   `Avalonia.Headless.XUnit` work above is a separate project for the GUI.

## 5. Notes / gotchas discovered

* Terminal.Gui **2.4.17** has `Terminal.Gui.Testing.InputInjector`, `Terminal.Gui.Time.VirtualTimeProvider`,
  `Terminal.Gui.Drivers.Driver` — but a view can also be exercised **in-process** (build the view, call
  `InvokeCommand(Command.Accept)`, assert on VM state) which is the fast red–green loop used here.
* `TextField` implements `IValue<string>`: `Value` + `ValueChanged(EventHandler<ValueChangedEventArgs<string>>)`
  (namespace `Terminal.Gui.App`) — that is the change notification the `Bind` helper hooks into.
* `StatusBar` is a `View`, so it can be used as the static footer; it keeps its `Shortcut`s on navigation.
