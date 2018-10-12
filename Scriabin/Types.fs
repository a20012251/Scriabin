[<AutoOpen>]
module Scriabin.Types

open System

type SimpleNote =
    | A = 0
    | B = 1
    | C = 2
    | D = 3
    | E = 4 
    | F = 5
    | G = 6

type Modifier =
    | Natural = 0
    | Flat = 1
    | Sharp = 2

type Note = SimpleNote * Modifier

type Interval = Note * Note

module Counts =
    let simpleNoteCount = Enum.GetValues(typeof<SimpleNote>).Length
    let modifierCount = Enum.GetValues(typeof<Modifier>).Length

module Modifier =
    let toString (modifier: Modifier) =
        match modifier.ToString() with
        | "Natural" -> ""
        | s -> s

module SimpleNote =
    let toString (simpleNote: SimpleNote) =
        match simpleNote with
        | SimpleNote.A -> "ay"
        | SimpleNote.B -> "bee"
        | SimpleNote.C -> "cee"
        | SimpleNote.D -> "dee"
        | SimpleNote.E -> "e"
        | SimpleNote.F -> "ef"
        | SimpleNote.G -> "gee"
        | _ -> ""