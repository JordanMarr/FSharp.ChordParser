module ChordParser.Console

open System.IO
open Model

/// Creates a new ChordParser file and saves the output.
let saveOutput filepath output =
    let file = FileInfo(filepath)
    let newPath = 
        Path.Combine(
            file.DirectoryName, 
            Path.GetFileNameWithoutExtension(filepath) + " ChordParser" + file.Extension
        )

    File.WriteAllText(newPath, output)

[<EntryPoint>]
let main argv =

    match argv with
    | [||] ->
        printfn "To run, type:\nChordParser \"my song.txt\"\nor:\nChordParser \"c:\myfolder\song.txt\""
        printfn "To transpose down 1 semitone preferring flats, type: \"\nChordParser \"song.txt\" -1 b"
        printfn "To transpose up 2 semitones preferring sharps, type: \"\nChordParser \"song.txt\" 2 #"

    | [| filepath |] ->

        filepath
        |> File.ReadAllText
        |> App.processText 0 "b" true
        |> saveOutput filepath

    | [| filepath; semitones; preferredAccidental |] ->
        
        filepath
        |> File.ReadAllText
        |> App.processText (int semitones) preferredAccidental true
        |> saveOutput filepath

    | _ -> 
        failwith "Expected ChordParser {filepath} OR ChordParser {filepath} {semitones +/-} {preferredAccidental}"

    0
