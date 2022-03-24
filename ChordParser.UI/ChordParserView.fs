﻿module ChordParser.UI.ChordParserView

open Avalonia.Layout
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.AvaloniaExtensions
open Avalonia.FuncUI.Types
open Avalonia.FuncUI.Elmish.ElmishHook

type Model = 
    { 
        InputChordChart: string
        OutputChordChart: Result<string, string> 
        Transpose: int
        Accidental: string
        UCase: bool
    }

type Msg = 
    | ParseChart
    | TransposeUp
    | TransposeDown
    | SetInputChart of chart: string
    | SetAccidental of string
    | SetUCase of bool
    | Reset

let init = 
    { 
        InputChordChart = SampleCharts.autumnLeaves
        OutputChordChart = Ok ""
        Transpose = 0
        Accidental = "b"
        UCase = false
    }, Cmd.none

let update msg model = 
    match msg with
    | ParseChart ->
        let c = ChordParser.App.tryProcessText model.Transpose model.Accidental model.UCase model.InputChordChart
        { model with OutputChordChart = c }, Cmd.none
    | TransposeUp ->
        if model.Transpose < 11 
        then { model with Transpose = model.Transpose + 1 }, Cmd.ofMsg ParseChart
        else model, Cmd.none
    | TransposeDown ->
        if model.Transpose > -11 
        then { model with Transpose = model.Transpose - 1 }, Cmd.ofMsg ParseChart
        else model, Cmd.none
    | SetInputChart chart -> 
        { model with InputChordChart = chart }, Cmd.ofMsg ParseChart
    | SetAccidental acc -> 
        { model with Accidental = acc }, Cmd.ofMsg ParseChart
    | SetUCase ucase -> 
        { model with UCase = ucase }, Cmd.ofMsg ParseChart
    | Reset ->
        init

let cmp () = Component (fun ctx ->
    let model, dispatch = ctx.useElmish (init, update)
    
    Grid.create [
        Grid.rowDefinitions "20, *"
        Grid.columnDefinitions "*, 80, *"
        Grid.margin 10
        
        Grid.children [
            // Row labels
            TextBlock.create [
                TextBlock.text "Input Chord Chart"
                TextBlock.horizontalAlignment HorizontalAlignment.Center
                Grid.column 0
            ]
            TextBlock.create [
                TextBlock.text "Output Chord Chart"
                TextBlock.horizontalAlignment HorizontalAlignment.Center
                Grid.column 2
            ]

            // Input Chord Chart
            TextBox.create [
                TextBox.text model.InputChordChart
                TextBox.onTextChanged (fun txt -> dispatch (SetInputChart txt))
                Grid.column 0
                Grid.row 1
            ]

            // Middle Column (Settings)
            StackPanel.create [
                Grid.column 1
                Grid.row 1
                StackPanel.margin (5, 0, 5, 0)

                StackPanel.children [
                    // Transpose up
                    Button.create [
                        Button.content "▲"
                        Button.horizontalAlignment HorizontalAlignment.Stretch
                        Button.horizontalContentAlignment HorizontalAlignment.Center
                        Button.onClick (fun e -> dispatch TransposeUp)
                    ]

                    // Current transpose
                    TextBlock.create [
                        TextBlock.text (string model.Transpose)
                        TextBlock.horizontalAlignment HorizontalAlignment.Center
                    ]

                    // Transpose down
                    Button.create [
                        Button.content "▼"
                        Button.horizontalAlignment HorizontalAlignment.Stretch
                        Button.horizontalContentAlignment HorizontalAlignment.Center
                        Button.onClick (fun e -> dispatch TransposeDown)
                    ]

                    // Change case
                    StackPanel.create [
                        StackPanel.orientation Orientation.Horizontal
                        StackPanel.horizontalAlignment HorizontalAlignment.Center
                        StackPanel.children [
                            CheckBox.create [
                                CheckBox.isChecked model.UCase
                                CheckBox.onChecked (fun _ -> dispatch (SetUCase true))
                                CheckBox.onUnchecked (fun _ -> dispatch (SetUCase false))
                            ]
                            TextBlock.create [
                                TextBlock.verticalAlignment VerticalAlignment.Center
                                TextBlock.text <|
                                    if model.UCase
                                    then "A -> a"
                                    else "a -> A"
                            ]
                        ]
                    ]

                    // ♭/ ♯
                    StackPanel.create [
                        StackPanel.orientation Orientation.Horizontal
                        StackPanel.horizontalAlignment HorizontalAlignment.Center
                        StackPanel.children [
                            RadioButton.create [
                                RadioButton.content "♭ "
                                RadioButton.isChecked (model.Accidental = "b")
                                RadioButton.onChecked (fun _ -> dispatch (SetAccidental "b"))
                            ]

                            RadioButton.create [
                                RadioButton.content "♯ "
                                RadioButton.isChecked (model.Accidental = "#")
                                RadioButton.onChecked (fun _ -> dispatch (SetAccidental "#"))
                            ]
                        ]
                    ]

                    Button.create [
                        Button.content "Reset"
                        Button.horizontalAlignment HorizontalAlignment.Stretch
                        Button.horizontalContentAlignment HorizontalAlignment.Center
                        Button.onClick (fun _ -> dispatch Reset)
                        Button.isEnabled (
                            let initialModel, _ = init
                            model <> initialModel
                        )
                    ]
                ]
            ]

            // Output Chord Chart
            TextBox.create [
                TextBox.text <|  
                    match model.OutputChordChart with
                    | Ok output -> output
                    | Error err -> err
                TextBox.verticalAlignment VerticalAlignment.Stretch
                TextBox.isReadOnly true
                Grid.column 2
                Grid.row 1
            ]

        ]
    ]
    :> IView
)