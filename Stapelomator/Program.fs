
// NOTE: If warnings appear, you may need to retarget this project to .NET 4.0. Show the Solution
// Pad, right-click on the project node, choose 'Options --> Build --> General' and change the target
// framework to .NET 4.0 or .NET 4.5.

module Stapelomator.Main

open System

open Stapel.Core

[<EntryPoint>]
let main args =
    Console.WriteLine("Starting game")
    runGame() |> ignore
    0

