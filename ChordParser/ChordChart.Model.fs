module ChordChart.Model

open System

let rootNotes = ["A"; "A#"; "Bb"; "B"; "C"; "C#"; "Db"; "D"; "D#"; "Eb"; "E"; "F"; "F#"; "Gb"; "G"; "G#"; "Ab"]

type Chord = {
    Root: string
    Tonality: string option
    Extension: string option
}

type ChordChart =
    | Lyrics of string
    | Chord of Chord    


let transpose semitones chord =
    let notes = 
        if semitones > 0 // if positive, remove flats; else remove sharps.
        then rootNotes |> List.filter (fun n -> n.EndsWith("b") = false)
        else rootNotes |> List.filter (fun n -> n.EndsWith("#") = false)

    let idxByNote = notes |> List.mapi (fun idx note -> note, idx) |> Map.ofList
        
    let rootIdx = ((idxByNote.[chord.Root]) + semitones) % 12
    let rootIdx = if rootIdx < 0 then rootIdx + 12 else rootIdx
    { chord with Root =  notes.[rootIdx] }
    //{ chord with Root = enum<Notes>(root) }

let printChord chord =
    sprintf "(%s%s%s)" chord.Root (chord.Tonality |> Option.defaultValue "") (chord.Extension |> Option.defaultValue "")

