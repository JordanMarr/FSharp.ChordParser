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
        }

    type Msg = 
        | SetInput of string
        | ParseChart
        | Reset

    let initModel = { InputChordChart = ""; OutputChordChart = "" }

    let init () = initModel, Cmd.none

    let update msg model =
        match msg with
        | SetInput txt -> { model with InputChordChart = txt }, Cmd.none
        | Reset -> init ()
        | ParseChart ->
            { model with 
                OutputChordChart = model.InputChordChart |> App.processText 2 "b"
            }, Cmd.none
        
    let view (model: Model) dispatch =
        View.ContentPage(content = 
            View.Grid(padding = Thickness 20.0, //verticalOptions = LayoutOptions.Center,
                children = [ 
                    View.Label(text = "Input Chord Chart", horizontalOptions = LayoutOptions.Center, horizontalTextAlignment=TextAlignment.Start).Column(0)
                    View.Label(text = "Output Chord Chart", horizontalOptions = LayoutOptions.Center, horizontalTextAlignment=TextAlignment.Start).Column(1)
                    View.Entry(text = model.InputChordChart, textChanged = (fun e -> dispatch (SetInput e.NewTextValue))).Column(0).Row(1)
                    View.Entry(text = model.OutputChordChart).Column(1).Row(1)

                    View.Button(
                        text = "Reset", horizontalOptions = LayoutOptions.Center, command = (fun () -> dispatch Reset), commandCanExecute = (model <> initModel)
                    ).Row(3).Column(0)
                    View.Button(
                        text = "Parse", horizontalOptions = LayoutOptions.Center, command = (fun () -> dispatch ParseChart), commandCanExecute = (model.InputChordChart <> "")
                    ).Row(3).Column(1)
                ])
            ).RowDefinitions([Dimension.Absolute 30; Dimension.Star; Dimension.Star])

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


