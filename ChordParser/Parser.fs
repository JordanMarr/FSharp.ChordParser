module ChordParser.Parser
open FParsec
open Model

type ChordChart =
    | Lyrics of string
    | Chord of Chord    

let str s = pstring s
let strCI s = pstringCI s
let ws = spaces

let createChord (((root, tonality), ext), bassNote) =
    { Root = root
      Tonality = tonality
      Extension = ext
      BassNote = bassNote }
    |> ChordChart.Chord

let chord = 
    let note = 
        rootNotes
        |> List.sortByDescending (fun n -> n.Length) // "C#" must be before "C" to avoid a "partial" consumption of "C" that prevents "C#" from matching.
        |> List.map str
        |> choice
        |>> string

    let tonality = 
        [strCI "maj"; str "M"; strCI  "min"; str "m"; str "-"; strCI "dim"; str "o"; strCI "aug"; str "+"; str "+5"] // Order matters to avoid partial consumption bugs
        |> choice 
        |>> string

    let tonality = opt (skipChar ' ') >>. tonality
    let extension = many1Chars digit |>> string
    let bassNote = opt (skipChar ' ') >>. skipChar '/' >>. note
    
    skipChar '(' >>. note .>>. opt tonality .>>. opt extension .>>. opt bassNote .>> skipChar ')' |>> createChord

let lyric = many1Chars (noneOf "(") .>> spaces |>> (string >> ChordChart.Lyrics)

let chordChart = many (lyric <|> chord)

/// Parses a chord chart.
let parse text = 
    match run chordChart text with
    | Success (ast,_,_) -> ast
    | Failure (_,error,_) -> failwith (error.ToString())
