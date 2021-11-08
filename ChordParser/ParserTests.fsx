#r "nuget: FParsec"
#load "Model.fs"
#load "Parser.fs"

open FParsec
open Model
open Parser

let test parser text =
    match (run parser text) with
    | Success(result,_,_) -> printfn "Success: %A" result
    | Failure(_,error,_) -> printfn "Error: %A" error

test chord "(Bmaj7)"
test chord "(A)"
test chord "(G)"
test chord "(Dmin7)"
test chord "(D min7)"
test chord "(Bmaj)"
test chord "(C)"
test chord "(C#)"
test chord "(D#)"
test chord "(D#min7)"
test chord "(Db)" /// createChord was changing to C# because they have the same int!
test chord "(C#maj7)"
test chord "(B)"
test chord "(Cmaj7)"
test chord "(Cmaj11)"
test chord "(Cmaj7 /F)"
test chord "(Cmaj7/F)"

let lyric = many1Chars (noneOf "(") .>> spaces |>> (string >> Lyrics)
//let lyric = manyTill anyChar chord |>> (string >> Lyric)

//let lyric = skipAnyChar |>> string
//let lyric = manyTill anyChar chord |>> string
//let lyric = many anyChar |>> string
//let chordSheet = many (manyTill lyric chord) |>> string //<|> lyric)
let chordSheet = many (lyric <|> chord)
test chordSheet "The (BMaj7) quick brown fox (AMin7) jumped over the lazy (DMin7) dog."

// Print tests
let chord = { Root = "C"; Tonality = Some "min"; Extension = None; BassNote = Some "F" }
printf "Print: %s" (chord |> printChord)

// Transpose tests
chord |> transpose 1 "#"

// createChord test
let x = createChord ((("D", Some "min"), Some "7"), None)
match x with 
| Chord chord -> Model.transpose -0 chord.Root
| _ -> failwith ""
