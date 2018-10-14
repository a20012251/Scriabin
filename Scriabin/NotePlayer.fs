module Scriabin.NotePlayer

open Xamarin.Essentials
open System
open Scriabin.Types
open Scriabin.Random

let settings = 
    let set = new SpeakSettings()
    set.Pitch <- Nullable 0.5f
    set

let simpleNoteToIndex simpleNote =
    match simpleNote with
    | A -> 0
    | B -> 2
    | C -> 3
    | D -> 5
    | E -> 7
    | F -> 8
    | G -> 10

let modifierToDelta modifier =
    match modifier with
    | Flat -> -1
    | Natural -> 0
    | Sharp -> 1

let noteToIndex (note, modifier) =
   (simpleNoteToIndex note) + (modifierToDelta modifier)

let keyboardNoteToIndex (note, modifier, octave) =
    noteToIndex (note, modifier) + octave * 12
    
let isKeyboardNoteValid keyboardNote =
    let index = keyboardNoteToIndex keyboardNote
    index >= 0 && index < 88

let randomNote () =
    let simpleNote = Random.randDiscrimatedUnion<SimpleNote> ()
    let modifier =  Random.randDiscrimatedUnion<Modifier> ()
    simpleNote, modifier

let randomOctave ()  =
    Random.rand.Next(maxOctave + 1)

let rec randomKeyboardNote () =
    let (simpleNote, modifier) = randomNote ()
    let keyboardNote = (simpleNote, modifier, randomOctave ())
    if not <| isKeyboardNoteValid keyboardNote then
        randomKeyboardNote ()
    else
        keyboardNote

let speak text =
    TextToSpeech.SpeakAsync("Next " + text, settings)

let playRandomSimpleNote () =
    let simpleNote, modifier = randomNote ()
    simpleNote.ToVoice + " " + modifier.ToVoice |> speak

let playRandomKeyboardNote () =
    let simpleNote, modifier, octave = randomKeyboardNote ()
    simpleNote.ToVoice + " " + modifier.ToVoice + " " + octave.ToString() |> speak
