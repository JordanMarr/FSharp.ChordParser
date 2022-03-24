# FSharp.ChordParser

I created this for my dad to allow him to transpose chords and change lyrics to uppercase (and it was also a great opportunity for me to learn FParsec).

## UI
UI created with [Avalonia.FuncUI](https://github.com/fsprojects/Avalonia.FuncUI/). 

![image](https://user-images.githubusercontent.com/1030435/159948606-0b9e9337-67da-4d13-a2d3-252ecc582bd0.png)

```F#
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
```

## Console
CLI created with [FSharp.SystemCommandLine](https://github.com/JordanMarr/FSharp.SystemCommandLine)

To try it out against the sample txt file (Song.txt), just run the app; it will output "Song ChordParser.txt". 

![image](https://user-images.githubusercontent.com/1030435/159951560-c9174a4c-d856-4569-9942-afe22c24a276.png)

