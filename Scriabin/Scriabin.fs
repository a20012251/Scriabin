// Copyright 2018 Fabulous contributors. See LICENSE.md for license.
namespace Scriabin

open Fabulous.Core
open Fabulous.DynamicViews
open Xamarin.Forms

module App = 
    type Game =
        | SimpleNoteGame
        | KeyboardNoteGame
        member this.DisplayName =
            match this with
            | SimpleNoteGame -> "Simple Notes"
            | KeyboardNoteGame -> "Keyboard Notes"

    type Model = 
      { ActiveGame: Game option
        WaitTimeInSec: int }

    type Msg = 
        | GameToggled of Game * isOn: bool 
        | GameRepetition of Game
        | WaitTimeValueChanged of waitTime: int

    let initModel = { ActiveGame = None; WaitTimeInSec = 4}

    let init () = initModel, Cmd.none

    let speak game =
        match game with
        | SimpleNoteGame -> NotePlayer.playRandomSimpleNote ()
        | KeyboardNoteGame -> NotePlayer.playRandomKeyboardNote ()

    let repetitionCmd game waitTimeInSec = 
        async { 
            do! Async.AwaitIAsyncResult (speak game) |> Async.Ignore
            do! Async.Sleep (waitTimeInSec * 1000)
            return GameRepetition game }
        |> Cmd.ofAsyncMsg


    let update msg (model: Model) =
        match msg, model.ActiveGame with
        | WaitTimeValueChanged value, _ -> {model with WaitTimeInSec = value}, Cmd.none
        | GameRepetition _, None ->  model, Cmd.none
        | GameRepetition game, Some activeGame  when game <> activeGame ->  model, Cmd.none
        | GameRepetition _, Some activeGame -> 
                model, repetitionCmd activeGame model.WaitTimeInSec
        | GameToggled (game, true), _ -> {model with ActiveGame = Some game}, Cmd.ofMsg (GameRepetition game)
        | GameToggled (game, false), Some activeGame when activeGame = game -> {model with ActiveGame = None}, Cmd.none
        | GameToggled (_, false), _ -> model, Cmd.none


    let createLabel (game: Game) =
        View.Label(text = game.DisplayName, horizontalOptions = LayoutOptions.Center)
        
    let createToggleForGame dispatch model game =
        View.Switch(
            isToggled = (model.ActiveGame = (Some game)),
            toggled = (fun on -> dispatch <| GameToggled (game, on.Value)),
            horizontalOptions = LayoutOptions.Center)

    let createGameView dispatch model game =
        [ createLabel game
          createToggleForGame dispatch model game
        ]

    let createWaitTimeSlider dispatch model =
        [View.Slider(
            minimum = 0.0, 
            maximum = 10.0, 
            value = double model.WaitTimeInSec, 
            valueChanged = (fun args -> 
                let newWaitTime = int (args.NewValue + 0.5)
                if newWaitTime <> model.WaitTimeInSec then
                    dispatch (WaitTimeValueChanged newWaitTime)),
            isEnabled = (model.ActiveGame = None))
         View.Label(text = sprintf "Delay: %d seconds" model.WaitTimeInSec)  
        ]

    let view (model: Model) dispatch =
        View.ContentPage(
          content = View.StackLayout(padding = 20.0, verticalOptions = LayoutOptions.Center,
            children = 
                createGameView dispatch model SimpleNoteGame @
                createGameView dispatch model KeyboardNoteGame @
                createWaitTimeSlider dispatch model 
            )
        )

    // Note, this declaration is needed if you enable LiveUpdate
    let program = Program.mkProgram init update view

type App () as app = 
    inherit Application ()

    let runner = 
        App.program
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> Program.runWithDynamicView app

#if DEBUG
    // Uncomment this line to enable live update in debug mode. 
    // See https://fsprojects.github.io/Fabulous/tools.html for further  instructions.
    //
    //do runner.EnableLiveUpdate()
#endif    

    // Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
    // See https://fsprojects.github.io/Fabulous/models.html for further  instructions.
#if APPSAVE
    let modelId = "model"
    override __.OnSleep() = 

        let json = Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)
        Console.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

        app.Properties.[modelId] <- json

    override __.OnResume() = 
        Console.WriteLine "OnResume: checking for model in app.Properties"
        try 
            match app.Properties.TryGetValue modelId with
            | true, (:? string as json) -> 

                Console.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)
                let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

                Console.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
                runner.SetCurrentModel (model, Cmd.none)

            | _ -> ()
        with ex -> 
            App.program.onError("Error while restoring model found in app.Properties", ex)

    override this.OnStart() = 
        Console.WriteLine "OnStart: using same logic as OnResume()"
        this.OnResume()
#endif


