﻿open System.IO
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

/// Creates a new UCASE file and saves the output.
let saveOutput filepath output =
    let file = FileInfo(filepath)
    let newPath = 
        Path.Combine(
            file.DirectoryName, 
            Path.GetFileNameWithoutExtension(filepath) + " UCASE" + file.Extension
        )

    File.WriteAllText(newPath, output)

[<EntryPoint>]
let main argv =

    match argv with
    | [||] ->
        printfn "To run, type:\nucase \"hello world.txt\"\nor:\nucase \"c:\myfolder\myfile.txt\""
        printfn "To transpose down 1 semitone preferring flats, type: \"\nucase \"myfile.txt\" -1 b"
        printfn "To transpose up 2 semitones preferring sharps, type: \"\nucase \"myfile.txt\" 2 #"

    | [| filepath |] ->

        filepath
        |> File.ReadAllText
        |> processText 0 "b"
        |> saveOutput filepath

    | [| filepath; semitones; preferredAccidental |] ->
        
        filepath
        |> File.ReadAllText
        |> processText (int semitones) preferredAccidental
        |> saveOutput filepath

    | _ -> 
        failwith "Expected ucase {filepath} OR ucase {filepath} {semitones +/-} {preferredAccidental}"

    0
