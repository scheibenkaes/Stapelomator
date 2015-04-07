
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

    type Colorable = {
        Color: Color
    }
    
    type Sizeable = {
        Size : float32 * float32
    }

    let socket (texture: Texture2D) body = 
        let e = newEntity "socket"
        e |> addComponent {Texture = texture} |> addComponent {Body = body}
