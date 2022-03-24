module ChordParser.Console

open System.IO
open FSharp.SystemCommandLine

let run (chordChart: FileInfo, semitones: int, preferredAccidental: string) =
    chordChart.FullName
    |> System.IO.File.ReadAllText
    |> App.processText semitones preferredAccidental true
    |> App.saveOutput chordChart.FullName

[<EntryPoint>]
let main argv =

    let filepath = Input.Argument("filepath", "Path to the input song text file.")
    let semitones = Input.Option(["--semitones"; "-s"], 0, "The number of semitones to transpose (+/-).")
    let preferredAccidental = Input.Option(["--preferred-accidental"; "-a"], "b", "# or b (defaults to b)")

    rootCommand argv {
        description "Chord Parser"
        inputs (filepath, semitones, preferredAccidental)
        setHandler run
    }
