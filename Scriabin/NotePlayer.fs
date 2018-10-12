module Scriabin.NotePlayer

open Xamarin.Essentials
open System
open Scriabin.Types

let settings = 
    let set = new SpeakSettings()
    set.Pitch <- Nullable 0.5f
    set

let play (rand: Random) =
    let simpleNote = 
        let index = rand.Next(Counts.simpleNoteCount)
        Enum.ToObject(typeof<SimpleNote>, index) |> unbox<SimpleNote>
    let modifier = 
        let index = rand.Next(Counts.modifierCount)
        Enum.ToObject(typeof<Modifier>, index) |> unbox<Modifier>

    let modifierName = Modifier.toString modifier
    TextToSpeech.SpeakAsync(simpleNote.ToString() + " " + modifierName, settings) |> ignore
