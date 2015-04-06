
namespace Stapel

module Entities = 

    open Microsoft.Xna.Framework
    open Microsoft.Xna.Framework.Graphics

    open Dasein.Core

    type World = {
        Bounds : Entity
    }

    type Drawable = {
        Texture : Texture2D
    }

    type Positionable = {
        Position : Vector2
    }
    
    type Sizeable = {
        Size : int * int
    }
    
    type StaticBody = {
        Body : Rectangle
    }

    let socket (texture: Texture2D) rect = 
        let e = newEntity "socket"
        e |> addComponent {Texture = texture} |> addComponent {Body = rect}
