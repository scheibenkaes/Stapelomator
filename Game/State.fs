module Stapel.State

open Microsoft.Xna.Framework
open FarseerPhysics.Dynamics

open Dasein.Core

type RunningState = 
    | Running
    | GameOver

type GameState = {
    Points : int;
    World: World;
    Pieces: Entity list;
    RunningState: RunningState
}

let addPiece state piece = {state with Pieces = piece :: state.Pieces}

let initialState () = 
    {Points = 0; World = new World(Vector2(0.f, 9.82f)); Pieces = []; RunningState = Running}
