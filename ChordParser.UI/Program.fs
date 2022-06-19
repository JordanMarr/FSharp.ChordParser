open Avalonia
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts
open Avalonia.Controls.ApplicationLifetimes
open ChordParser.UI
open Avalonia.FuncUI.LiveView
open Avalonia.Controls

module LiveView =
    [<Literal>]
    let FUNCUI_LIVEPREVIEW = "FUNCUI_LIVEPREVIEW"

    let enabled =
        match System.Environment.GetEnvironmentVariable FUNCUI_LIVEPREVIEW with
        | null -> false
        | "1" -> true
        | _ -> false

type MainWindow() as this =
    inherit HostWindow()
    do
        base.Title <- "Chord Parser"
        base.Content <- ChordParserView.render ()

#if DEBUG
        this.AttachDevTools()
#endif

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add (FluentTheme(baseUri = null, Mode = FluentThemeMode.Light))

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.MainWindow <-
                if LiveView.enabled then
                    LiveViewWindow() :> Window
                else
                    MainWindow()
        | _ -> ()

module Program =

    [<EntryPoint>]
    let main(args: string[]) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)