module Stapel.Core

open System

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

open Dasein.Core

open FarseerPhysics
open FarseerPhysics.Dynamics
open FarseerPhysics.Factories
open FarseerPhysics.DebugView

open Stapel.Entities

let windowHeight = 600
let windowWidth = 800
let (socketWidth, socketHeight) = 200, 100
let pieceCreationTimeout = System.TimeSpan.FromSeconds 1.


let rnd = new System.Random()

let randomPiece () = (float32(rnd.Next(50, 150)), float32(rnd.Next(40, 60)))

let pieceColors = [Color.ForestGreen; Color.Aquamarine; Color.BurlyWood]

let toRectangle x y w h = new Rectangle(int(x), int(y), int(w), int(h))

type GameState = 
    | Running
    | GameOver


type MyGame () as this =
    inherit Game()
    
    let graphics = new GraphicsDeviceManager(this)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    
    do this.Window.Title <- "Stack that s%$t"
    do graphics.PreferredBackBufferWidth <- windowWidth
    do graphics.PreferredBackBufferHeight <- windowHeight
    do this.IsMouseVisible <- true

    let world = new World(new Vector2(0.f, 9.82f))
    
    
    let debugView = new DebugViewXNA(world)
        
    let mutable sock = Unchecked.defaultof<Entity>
    
    let mutable pieces : Entity list = []
    let mutable gameState = Running
    let mutable lastPieceCreatedAt = System.DateTime.Now
    
    let createTexture () =
        let t = new Texture2D(this.GraphicsDevice, 1, 1)
        t.SetData [| Color.White |]
        t
    
    let renderSocket s (sb: SpriteBatch) =
        let t = getComponentValue<Drawable>(s).Texture
        let body = getComponentValue<Bodyable>(s).Body
        let pos = ConvertUnits.ToDisplayUnits(body.Position) - Vector2(100.f, 50.f)
        let rr = toRectangle pos.X pos.Y (float(socketWidth)) (float(socketHeight))
        sb.Draw(t, rr, Color.Bisque)
    
    let renderPieces pieces (sb: SpriteBatch) = 
        for p in pieces do
            let body = getComponentValue<Bodyable>(p).Body
            let t = getComponentValue<Drawable>(p).Texture
            let s = getComponentValue<Sizeable>(p).Size
            let color = getComponentValue<Colorable>(p).Color
            
            let w = int(fst(s))
            let h = int(snd(s))
            
            let position = ConvertUnits.ToDisplayUnits(body.Position)
            sb.Draw(t, 
                new Rectangle(int(position.X), int(position.Y), w, h),
                System.Nullable(), 
                color, body.Rotation, Vector2(0.5f, 0.5f), SpriteEffects.None, 1.f)
    
    let spawnNewPiece (at: Vector2) =
        let now = System.DateTime.Now
        if now >= lastPieceCreatedAt.Add(pieceCreationTimeout)
        then
            let (w, h) = randomPiece()
            let pos = ConvertUnits.ToSimUnits(at)
            printfn "Creating body at %A size %A" pos (w,h)
            printfn "in world: %A" (ConvertUnits.ToSimUnits(w), ConvertUnits.ToSimUnits(h))
            let b = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(w), ConvertUnits.ToSimUnits(h), 1.f, pos)
            b.IsStatic <- false
            b.Friction <- 0.9f
            b.Restitution <- 0.0f
            
            let e = (newEntity "piece" 
                |> addComponent {Body = b} 
                |> addComponent {Texture = createTexture()} 
                |> addComponent {Size = (ConvertUnits.ToSimUnits(w), ConvertUnits.ToSimUnits(h))}
                |> addComponent {Color = pieceColors.[(rnd.Next(0, pieceColors.Length - 1))]})
            pieces <- e :: pieces
            lastPieceCreatedAt <- now
        
        
    let checkInput () =
        let mouseState = Mouse.GetState()
        if mouseState.LeftButton = ButtonState.Pressed
        then
            printfn "clicked at %A" mouseState.Position
            spawnNewPiece (new Vector2(float32(mouseState.Position.X), float32(mouseState.Position.Y)))
        
    override x.Draw(gametime: GameTime) =
        this.GraphicsDevice.Clear(Color.CornflowerBlue)
        spriteBatch.Begin()
        renderSocket sock spriteBatch |> ignore
        renderPieces pieces spriteBatch |> ignore
        
        spriteBatch.End()

    override x.Update(gameTime) =
        if gameState = Running
        then
            world.Step(0.01f * float32(gameTime.ElapsedGameTime.TotalMilliseconds))
            checkInput()
        
    override x.LoadContent () =
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        let texture = createTexture()
        let bounds = this.GraphicsDevice.Viewport
        
        let x = bounds.Width / 2 - socketWidth / 2
        let y = bounds.Height - socketHeight - 25
        
        ConvertUnits.SetDisplayUnitToSimUnitRatio(1.f)
        let p = new Vector2(float32(x), float32(y)) |> ConvertUnits.ToSimUnits
        
        let socketBody = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(socketWidth), ConvertUnits.ToSimUnits(socketHeight), 1.f, p)
        socketBody.IsStatic <- true
        socketBody.BodyType <- BodyType.Static
        
        sock <- socket texture socketBody



let runGame () =
    use g = new MyGame()
    g.Run()