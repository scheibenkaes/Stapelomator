module Stapel.Core

open System

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

open Dasein.Core

open FarseerPhysics
open FarseerPhysics.Dynamics
open FarseerPhysics.Factories
open FarseerPhysics.Collision

open Stapel.Entities
open Stapel.State
open Stapel.Calculation

let windowHeight = 600
let windowWidth = 800
let (socketWidth, socketHeight) = 200, 100
let pieceCreationTimeout = System.TimeSpan.FromSeconds 0.5

let rnd = Random()

let randomPiece () = (float32(rnd.Next(50, 150)) * 1.0f<display>, float32(rnd.Next(40, 60)) * 1.0f<display>)

let pieceColors = [Color.ForestGreen; Color.Aquamarine; Color.BurlyWood; Color.DarkCyan; Color.Firebrick]

let toRectangle x y w h = new Rectangle(int(x), int(y), int(w), int(h))

type RunningState = 
    | Running
    | GameOver
    
type GameEvent =
    | ScoredPoints of int

type MyGame () as this =
    inherit Game()
    
    let graphics = new GraphicsDeviceManager(this)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    
    do this.Window.Title <- "Stack that s%$t up!!1!"
    do graphics.PreferredBackBufferWidth <- windowWidth
    do graphics.PreferredBackBufferHeight <- windowHeight
    do this.IsMouseVisible <- true

    let world = new World(new Vector2(0.f, 9.82f))
        
    let mutable sock = Unchecked.defaultof<Entity>
    
    let mutable pieces : Entity list = []
    let mutable gameState = Running
    let mutable lastPieceCreatedAt = DateTime.Now
    let mutable floor = Unchecked.defaultof<Entity>
    let mutable font = Unchecked.defaultof<SpriteFont>
    let mutable state = initialState()
    
    let initGame() =
        state <- initialState()
    
    let handleEvent (e: GameEvent) =    
        match e with
        | ScoredPoints p -> state <- {state with Points = state.Points + p}
    
    let createTexture () =
        let t = new Texture2D(this.GraphicsDevice, 1, 1)
        t.SetData [| Color.White |]
        t
    
    let renderSocket s (sb: SpriteBatch) =
        let t = getComponentValue<Drawable>(s).Texture
        let body = getComponentValue<Bodyable>(s).Body
        let pos = ConvertUnits.ToDisplayUnits(body.Position) // - Vector2(100.f, 50.f)
        let rr = getRenderRectangle s //toRectangle pos.X pos.Y (float(socketWidth)) (float(socketHeight))
        sb.Draw(t, rr, Color.Bisque)
    
    let renderPieces pieces (sb: SpriteBatch) = 
        for p in pieces do
            let body = getComponentValue<Bodyable>(p).Body
            let t = getComponentValue<Drawable>(p).Texture
            let (w,h) = getComponentValue<Sizeable>(p).Size
            let color = getComponentValue<Colorable>(p).Color
            
            let position = ConvertUnits.ToDisplayUnits(body.Position)
            sb.Draw(t, 
                new Rectangle(int(position.X), int(position.Y), int(w), int(h)),
                Nullable(), 
                color, body.Rotation, Vector2(0.5f, 0.5f), SpriteEffects.None, 1.f)
    
    let collisionWithGround (f1: Fixture) (f2: Fixture) contact = 
        gameState <- GameOver
        true
    
    let spawnNewPiece (at: Vector2) =
        let now = DateTime.Now
        if now >= lastPieceCreatedAt.Add(pieceCreationTimeout)
        then
            let (w, h) = randomPiece()
            handleEvent(ScoredPoints(int(w * h) / 10))
            let pos = ConvertUnits.ToSimUnits(at)
            let b = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(float w), ConvertUnits.ToSimUnits(float h), 1.f, pos)
            b.IsStatic <- false
            b.Friction <- 0.9f
            b.Restitution <- 0.0f
            b.CollidesWith <- Category.Cat1 ||| Category.Cat10
            
            
            let e = (newEntity "piece" 
                |> addComponent {Body = b} 
                |> addComponent {Texture = createTexture()} 
                |> addComponent {Size = (w, h)}
                |> addComponent {Color = pieceColors.[(rnd.Next(0, pieceColors.Length - 1))]})
            pieces <- e :: pieces
            lastPieceCreatedAt <- now
        
        
    let checkInput () =
        let mouseState = Mouse.GetState()
        if mouseState.LeftButton = ButtonState.Pressed
        then
            match (mouseState.Position.X, mouseState.Position.Y) with
            | (x, y) when x >= 0 && y >= 0 ->
                spawnNewPiece (new Vector2(float32(mouseState.Position.X), float32(mouseState.Position.Y)))
            | _ -> ()
            
    let createFloor() =
        let bounds = this.GraphicsDevice.Viewport.Bounds
        let e = newEntity "floor"
        
        let center = Vector2(float32(bounds.Width / 2), float32(bounds.Height - 10))
        let body = BodyFactory.CreateRectangle(world, float32(bounds.Width), 20.f, 1.f, center)
        body.CollisionCategories <- Category.Cat10
        body.IsStatic <- true
        body.add_OnCollision (fun f1 f2 c -> collisionWithGround f1 f2 c)
        
        floor <- e |> addComponent {Body = body} |> addComponent {Texture = createTexture()}
        
    let renderFloor floor (sb: SpriteBatch) =
        let body = getComponentValue<Bodyable>(floor).Body
        let texture = getComponentValue<Drawable>(floor).Texture
        sb.Draw(texture, new Rectangle(0, 600 - 20, 800, 20), Color.MediumVioletRed)
        
    let renderGameOver (sb: SpriteBatch) =
        sb.DrawString(font, "Game Over!!1!", Vector2(400.f, 300.f), Color.Red)

    let renderTexts state (sb: SpriteBatch) =
        let builder = new System.Text.StringBuilder("Points: ")
        builder.Append state.Points |> ignore
        sb.DrawString(font, builder, Vector2(0.f, 0.f), Color.White)
                        
    override x.Draw(gametime: GameTime) =
        this.GraphicsDevice.Clear(Color.CornflowerBlue)
        spriteBatch.Begin()
        renderFloor floor spriteBatch |> ignore
        renderSocket sock spriteBatch |> ignore
        renderPieces pieces spriteBatch |> ignore
        renderTexts state spriteBatch |> ignore
        if gameState = GameOver
        then
            renderGameOver spriteBatch
        spriteBatch.End()

    override x.Update(gameTime) =
        if gameState = Running
        then
            world.Step(0.01f * float32(gameTime.ElapsedGameTime.TotalMilliseconds))
            checkInput()
        
    override x.LoadContent () =
        initGame()
        
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        
        this.Content.RootDirectory <- "Content"
        font <- this.Content.Load<SpriteFont> "Fonts/Font"
        
        let bounds = this.GraphicsDevice.Viewport
        let x = bounds.Width / 2
        let y = bounds.Height - socketHeight + 35
        ConvertUnits.SetDisplayUnitToSimUnitRatio(1.f)
        let p = new Vector2(float32(x), float32(y)) |> ConvertUnits.ToSimUnits
        
        let socketBody = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(socketWidth), ConvertUnits.ToSimUnits(socketHeight), 1.f, p)
        socketBody.IsStatic <- true
        socketBody.BodyType <- BodyType.Static
        sock <- socket (createTexture()) socketBody |> addComponent {Size = (float32(socketWidth) * 1.0f<display>, float32(socketHeight) * 1.0f<display>)}
        
        createFloor()


let runGame () =
    use g = new MyGame()
    g.Run()