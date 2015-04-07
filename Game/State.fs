module Stapel.State

type GameState = {
    Points : int
}

let initialState () = {Points = 0}
