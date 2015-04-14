module Stapelomator.Windows.Main

open System

open Stapel.Core

[<EntryPoint>]
let main args = 
    use g = new MyGame()
    g.Run() |> ignore
    0

