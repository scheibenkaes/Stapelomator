
namespace Tests
open System
open NUnit.Framework
open FsUnit

open Stapel.Entities
open Stapel.Calculation

open Dasein.Core
open Microsoft.Xna.Framework

open FarseerPhysics.Dynamics
open FarseerPhysics.Collision
open FarseerPhysics.Factories

[<TestFixture>]
type ``Farseer to monogame calculations``() = 
        [<Test>]
        member this.``getRenderRectangle sets off the coordinate as expected``  () =
            let world = World(Vector2(0.f, 10.f))
            let position = Vector2(200.f, 200.f)
            let body = BodyFactory.CreateRectangle(world, 100.f, 50.f, 1.f, position)
            let e = newEntity "test" |> addComponent {Size = (100.f<display>, 50.f<display>)} |> addComponent {Body = body}
            
            let r = getRenderRectangle e
            let expected = Rectangle(150, 175, 100, 50)
            r |> should equal expected
            
            
        [<Test>]
        member this.``getRenderRectangle throws when passed an entity without the needed components``  () =
            let world = World(Vector2(0.f, 10.f))
            let position = Vector2(200.f, 200.f)
            let body = BodyFactory.CreateRectangle(world, 100.f, 50.f, 1.f, position)
            let e = newEntity "test" |> addComponent {Size = (100.f<display>, 50.f<display>)}
            
            (fun () -> getRenderRectangle e |> ignore) |> should throw typeof<ArgumentException>
