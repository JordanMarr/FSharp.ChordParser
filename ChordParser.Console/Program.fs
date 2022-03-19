module ChordParser.Console

open FSharp.SystemCommandLine

let run (filepath: string, semitones: int, preferredAccidental: string) =
    filepath
    |> System.IO.File.ReadAllText
    |> App.processText semitones preferredAccidental true
    |> App.saveOutput filepath

[<EntryPoint>]
let main argv =

    let filepath = Input.Argument<string>("filepath", "Path to the input song text file..")
    let semitones = Input.Option(["--semitones"; "-s"], (fun () -> 0), "The number of semitones to transpose (+/-).")
    let preferredAccidental = Input.Option(["--preferred-accidental"; "-a"], (fun () -> "b"), "# or b (defaults to b)")

    rootCommand argv {
        description "Chord Parser"
        inputs (filepath, semitones, preferredAccidental)
        setHandler run
    }
