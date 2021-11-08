module Parser
open FParsec
open Model

/// The ChordChart AST
type ChordChart =
    | Lyrics of string
    | Chord of Chord    

//type UserState = unit
//type Parser<'T> = Parser<'T, UserState>

let test parser text =
    match (run parser text) with
    | Success(result,_,_) -> printfn "Success: %A" result
    | Failure(_,error,_) -> printfn "Error: %A" error

let str s = pstring s
let strCI s = pstringCI s
let ws = spaces

let createChord (((root, tonality), ext), bassNote) =
    { Root = root
      Tonality = tonality
      Extension = ext
      BassNote = bassNote }
    |> ChordChart.Chord

/// Parses a chord.
let chord = 
    /// Parses a root note
    let note = 
        rootNotes
        |> List.sortByDescending (fun n -> n.Length) // "C#" must be before "C" to avoid a "partial" consumption of "C" that prevents "C#" from matching.
        |> List.map str
        |> choice
        |>> string

    /// Parses the chord tonality
    let tonality = 
        [strCI "maj"; str "M"; strCI  "min"; str "m"; str "-"; strCI "dim"; str "o"; strCI "aug"; str "+"; str "+5"] // Order matters to avoid partial consumption bugs
        |> choice 
        |>> string

    let tonality = opt (skipChar ' ') >>. tonality

    /// Parses the extension
    let extension = many1Chars digit |>> string

    /// Parses the bass note
    let bassNote = opt (skipChar ' ') >>. skipChar '/' >>. note
    
    skipChar '(' >>. note .>>. opt tonality .>>. opt extension .>>. opt bassNote .>> skipChar ')' |>> createChord

/// Parses lyrics text
let lyric = many1Chars (noneOf "(") .>> spaces |>> (string >> ChordChart.Lyrics)

/// Parses a chord chart
let chordChart = many (lyric <|> chord)

/// Parses the chord chart.
let parse text = 
    match run chordChart text with
    | Success (ast,_,_) -> ast
    | Failure (_,error,_) -> failwith (error.ToString())
