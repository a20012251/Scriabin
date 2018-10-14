[<AutoOpen>]
module Scriabin.Types

type SimpleNote =
    | A 
    | B
    | C 
    | D 
    | E 
    | F 
    | G
    member this.ToVoice =
        match this with
        | A -> "ay"
        | B -> "bee"
        | C -> "see"
        | D -> "dee"
        | E -> "e"
        | F -> "ef"
        | G -> "gee"

type Modifier =
    | Natural 
    | Flat
    | Sharp 
    member this.ToVoice =
        match this with
        | Natural -> ""
        | s -> s.ToString()

type Note = SimpleNote * Modifier

type Octave = int

type KeyboardNote = SimpleNote * Modifier * Octave

type Interval = Note * Note

let maxOctave = 8