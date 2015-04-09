module Stapel.State

open Microsoft.Xna.Framework
open FarseerPhysics.Dynamics

type GameState = {
    Points : int;
    World: World
}

let initialState () = 
    {Points = 0; World = new World(Vector2(0.f, 9.82f))}
