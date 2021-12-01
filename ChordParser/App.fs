module App

open System.IO
open System.Text
open Model

/// Parses and processes the chordchart items.
let processText semitones preferredAccidental text = 
    Parser.parse text
    |> List.map (function
        | Parser.Lyrics lyrics -> lyrics.ToUpper()
        | Parser.Chord chord -> chord |> transpose semitones preferredAccidental |> printChord)
    |> List.fold (fun (sb: StringBuilder) txt -> sb.Append txt) (StringBuilder())
    |> string
