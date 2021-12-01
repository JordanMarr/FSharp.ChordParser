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
        | SetTranspose of int
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
        | SetInput txt -> { model with InputChordChart = txt }, Cmd.none
        | SetTranspose xpose -> { model with Transpose = xpose }, Cmd.none
        | SetAccidental acc -> { model with Accidental = acc }, Cmd.none
        | Reset -> init ()
        | ParseChart ->
            { model with 
                OutputChordChart = App.processText model.Transpose model.Accidental model.InputChordChart
            }, Cmd.none
        
    let view (model: Model) dispatch =
        View.ContentPage(title = "Chord Parser", content = 
            View.Grid(
                rowdefs = [ Dimension.Absolute 20.; Dimension.Star; Dimension.Absolute 30. ],
                coldefs = [ Dimension.Star; Dimension.Absolute 50.; Dimension.Star ],
                padding = Thickness 20.0,
                children = [ 

                    // Row: labels
                    View.Label(text = "Input Chord Chart", horizontalOptions = LayoutOptions.Center, horizontalTextAlignment=TextAlignment.Start).Column(0)
                    View.Label(text = "Output Chord Chart", horizontalOptions = LayoutOptions.Center, horizontalTextAlignment=TextAlignment.Start).Column(2)

                    // Row: inputs
                    // Input Chord Chart
                    View.Entry(
                        text = model.InputChordChart, 
                        textChanged = (fun e -> dispatch (SetInput e.NewTextValue)),
                        verticalTextAlignment = TextAlignment.Start
                    ).Column(0).Row(1)

                    // Settings
                    View.StackLayout(
                        children = [
                            let transpose = [-11..-1] @ [1..11]
                            let transposeIdxLookup = transpose |> List.mapi (fun idx t -> t, idx)|> Map.ofList
                                
                            View.Picker(
                                items = (transpose |> List.map string),
                                selectedIndex = (transposeIdxLookup.TryFind model.Transpose |> Option.defaultValue 12),
                                selectedIndexChanged = (fun (idx, value) -> 
                                    match value with 
                                    | Some xpose -> dispatch (SetTranspose (int xpose))
                                    | None -> dispatch (SetTranspose initModel.Transpose)
                                )
                            )

                            View.Picker(
                                items = ["b"; "#"],                                 
                                selectedIndex = (if model.Accidental = "b" then 0 else 1),
                                selectedIndexChanged = (fun (idx, value) -> 
                                    match value with 
                                    | Some acc -> dispatch (SetAccidental acc)
                                    | None -> dispatch (SetAccidental "b")
                                )
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


