module App

open System.IO
open System.Text
open Model

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