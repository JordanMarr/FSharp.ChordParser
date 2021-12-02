module Model

let rootNotes = ["A"; "A#"; "Bb"; "B"; "C"; "C#"; "Db"; "D"; "D#"; "Eb"; "E"; "F"; "F#"; "Gb"; "G"; "G#"; "Ab"]

type Chord = 
    {
        Root: string
        Tonality: string option
        Extension: string option
        BassNote: string option
    }

/// Transposes the root note of a chord and optional bass note to the given number of semitones, using the preferred accidental (# or b).
let transpose semitones preferredAccidental chord =
    let sharps = rootNotes |> List.filter (fun n -> not (n.EndsWith "b"))
    let flats = rootNotes |> List.filter (fun n -> not (n.EndsWith "#"))
    let notes  = 
        match preferredAccidental with 
        | "#" -> sharps 
        | "b" -> flats
        | _ -> failwith "Preferred Accidental must be either # or b."

    let transposeNote note =
        let sharpIdx = sharps |> List.tryFindIndex(fun n -> n = note)
        let flatIdx = flats |> List.tryFindIndex(fun n -> n = note)
        let idx = sharpIdx |> Option.orElse flatIdx |> Option.defaultValue -1
        if idx = -1 then failwith $"Invalid note: '{note}'"
        let transposedIdx = (idx + semitones) % 12
        let transposedIdx = if transposedIdx < 0 then transposedIdx + 12 else transposedIdx
        notes.[transposedIdx]

    { chord with 
        Root = chord.Root |> transposeNote
        BassNote = chord.BassNote |> Option.map transposeNote }

/// Prints chord to string.
let printChord chord =
    let tonality = chord.Tonality |> Option.defaultValue ""
    let extension = chord.Extension |> Option.defaultValue ""
    let bassNote = chord.BassNote |> Option.map (sprintf " /%s") |> Option.defaultValue ""
    $"({chord.Root}{tonality}{extension}{bassNote})"
