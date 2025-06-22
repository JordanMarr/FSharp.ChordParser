module ChordParser.Console

open System.IO
open FSharp.SystemCommandLine
open Input

let run (chordChart: FileInfo, semitones: int, preferredAccidental: string) =
    chordChart.FullName
    |> System.IO.File.ReadAllText
    |> App.processText semitones preferredAccidental true
    |> App.saveOutput chordChart.FullName

[<EntryPoint>]
let main argv =
    let filepath = argument "filepath" |> desc "Path to the input song text file."
    let semitones = option "--semitones" |> alias "-s" |> def 0 |> desc "The number of semitones to transpose (+/-)."
    let preferredAccidental = option "--preferred-accidental" |> alias "-a" |> def "b" |> desc "Preferred accidental (#|b). Defaults to b."

    rootCommand argv {
        description "Chord Parser"
        inputs (filepath, semitones, preferredAccidental)
        setAction run
    }
