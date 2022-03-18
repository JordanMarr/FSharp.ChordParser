module ChordParser.App

open System.Text
open Model
open System.IO

/// Parses and processes the chordchart items.
let processText (semitones: int) (preferredAccidental: string) (ucase: bool) (text: string) = 
    Parser.parse text
    |> List.map (function
        | Parser.Lyrics lyrics -> if ucase then lyrics.ToUpper() else lyrics
        | Parser.Chord chord -> chord |> transpose semitones preferredAccidental |> printChord)
    |> List.fold (fun (sb: StringBuilder) txt -> sb.Append txt) (StringBuilder())
    |> string

let tryProcessText (semitones: int) (preferredAccidental: string) (ucase: bool) (text: string) = 
    try 
        let output = processText semitones preferredAccidental ucase text
        Ok output
    with ex ->
        Error ex.Message


/// Creates a new ChordParser file and saves the output.
let saveOutput filepath output =
    let file = FileInfo(filepath)
    let newPath = 
        Path.Combine(
            file.DirectoryName, 
            Path.GetFileNameWithoutExtension(filepath) + " ChordParser" + file.Extension)

    File.WriteAllText(newPath, output)