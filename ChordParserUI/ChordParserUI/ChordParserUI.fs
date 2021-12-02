// Copyright Fabulous contributors. See LICENSE.md for license.
namespace ChordParserUI

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms

module App = 
    type Model = 
        { 
            InputChordChart: string
            OutputChordChart: string 
            Transpose: int
            Accidental: string
        }

    type Msg = 
        | SetInput of string
        | TransposeUp
        | TransposeDown
        | SetAccidental of string
        | ParseChart
        | Reset

    let initModel = 
        { 
            InputChordChart = 
#if DEBUG
                "(Bmaj7) Ooo Gustens,    you just (A#) so  (G)\n" +
                "Dang   (Dmin7 /G) Baaad."
#else
                ""
#endif
            OutputChordChart = ""
            Transpose = 1
            Accidental = "b"
        }

    let init () = initModel, Cmd.none

    let update msg model =
        match msg with
        | SetInput txt -> { model with InputChordChart = txt }, Cmd.ofMsg ParseChart
        | TransposeUp -> 
            if model.Transpose < 11 
            then { model with Transpose = model.Transpose + 1 }, Cmd.ofMsg ParseChart
            else model, Cmd.none
        | TransposeDown ->
            if model.Transpose > -11 
            then { model with Transpose = model.Transpose - 1 }, Cmd.ofMsg ParseChart
            else model, Cmd.none
        | SetAccidental acc -> { model with Accidental = acc }, Cmd.ofMsg ParseChart
        | Reset -> init ()
        | ParseChart ->
            { model with 
                OutputChordChart = App.processText model.Transpose model.Accidental model.InputChordChart
            }, Cmd.none
        
    let view (model: Model) dispatch =
        View.ContentPage(title = "Chord Parser", content = 
            View.Grid(
                rowdefs = [ Dimension.Absolute 20.; Dimension.Star; Dimension.Absolute 30. ],
                coldefs = [ Dimension.Star; Dimension.Absolute 40.; Dimension.Star ],
                padding = Thickness 20.0,
                children = [ 

                    // Row: labels
                    View.Label(text = "Input Chord Chart", horizontalOptions = LayoutOptions.Center, horizontalTextAlignment=TextAlignment.Start).Column(0)
                    View.Label(text = "Output Chord Chart", horizontalOptions = LayoutOptions.Center, horizontalTextAlignment=TextAlignment.Start).Column(2)

                    // Row: inputs
                    // Input Chord Chart
                    View.Editor(
                        text = model.InputChordChart, 
                        textChanged = (fun e -> dispatch (SetInput e.NewTextValue))
                    ).Column(0).Row(1)

                    // Middle Column Settings
                    View.StackLayout(
                        children = [
                            View.Button(text = "▲", width = 20., command = (fun e -> dispatch TransposeUp))
                            View.Label(text = string model.Transpose, horizontalOptions = LayoutOptions.Center)
                            View.Button(text = "▼", width = 20., command = (fun e -> dispatch TransposeDown))

                            View.StackLayout(
                                children = [
                                    View.RadioButton(
                                        content = Content.Value.String "♭", 
                                        isChecked = (model.Accidental = "b"),
                                        checkedChanged = (fun e -> if e.Value then dispatch (SetAccidental "b"))
                                    )
                                    View.RadioButton(
                                        content = Content.Value.String "♯", 
                                        isChecked = (model.Accidental = "#"),
                                        checkedChanged = (fun e -> if e.Value then dispatch (SetAccidental "#"))
                                    )
                                ]
                            )
                        ]
                    ).Column(1).Row(1)

                    // Input Chord Chart
                    View.Entry(
                        text = model.OutputChordChart,
                        verticalTextAlignment = TextAlignment.Start,
                        isReadOnly = true
                    ).Column(2).Row(1)

                    // Row: buttons
                    View.StackLayout(
                        orientation = StackOrientation.Horizontal,
                        horizontalOptions = LayoutOptions.End,
                        children = [
                            View.Button(
                                text = "Reset", width = 100., horizontalOptions = LayoutOptions.Center, command = (fun () -> dispatch Reset), commandCanExecute = (model <> initModel)
                            )
                            View.Button(
                                text = "Parse", width = 100., horizontalOptions = LayoutOptions.Center, command = (fun () -> dispatch ParseChart), commandCanExecute = (model.InputChordChart <> "")
                            )
                        ]
                    ).Row(2).Column(2)
                ])
            )

    // Note, this declaration is needed if you enable LiveUpdate
    let program =
        XamarinFormsProgram.mkProgram init update view
#if DEBUG
        |> Program.withConsoleTrace
#endif

type App () as app = 
    inherit Application ()

    let runner = 
        App.program
        |> XamarinFormsProgram.run app

#if DEBUG
    // Uncomment this line to enable live update in debug mode. 
    // See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/tools.html#live-update for further  instructions.
    //
    do runner.EnableLiveUpdate()
#endif    

    // Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
    // See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/models.html#saving-application-state for further  instructions.
#if APPSAVE
    let modelId = "model"
    override __.OnSleep() = 

        let json = Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)
        Console.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

        app.Properties.[modelId] <- json

    override __.OnResume() = 
        Console.WriteLine "OnResume: checking for model in app.Properties"
        try 
            match app.Properties.TryGetValue modelId with
            | true, (:? string as json) -> 

                Console.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)
                let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

                Console.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
                runner.SetCurrentModel (model, Cmd.none)

            | _ -> ()
        with ex -> 
            App.program.onError("Error while restoring model found in app.Properties", ex)

    override this.OnStart() = 
        Console.WriteLine "OnStart: using same logic as OnResume()"
        this.OnResume()
#endif


