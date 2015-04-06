
namespace Stapel

module Entities = 

    open Microsoft.Xna.Framework
    open Microsoft.Xna.Framework.Graphics
    
    open FarseerPhysics.Dynamics

    open Dasein.Core

    type Drawable = {
        Texture : Texture2D
    }
    
    type Bodyable = {
        Body: Body
    }

    type Positionable = {
        Position : Vector2
    }
    
    type Sizeable = {
        Size : float32 * float32
    }
    
    type StaticBody = {
        Rect : Rectangle
    }

    let socket (texture: Texture2D) rect = 
        let e = newEntity "socket"
        e |> addComponent {Texture = texture} |> addComponent {Rect = rect}
