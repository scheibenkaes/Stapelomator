
module Stapel.Calculation

open Microsoft.Xna.Framework

open Dasein.Core

open Stapel.Entities

let offsetToHalfLength pos len = pos - len / 2.f

let getRenderRectangle (e: Entity) =
    let position = getComponentValue<Bodyable>(e).Body.Position
    let (w, h) = getComponentValue<Sizeable>(e).Size
    Rectangle(int(offsetToHalfLength position.X (float32 w)), int(offsetToHalfLength position.Y (float32 h)), int(float w), int(float h))

