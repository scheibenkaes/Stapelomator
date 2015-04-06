module Stapel.Core

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

open Dasein.Core

open FarseerPhysics.Dynamics
open FarseerPhysics.Factories

open Stapel.Entities

let windowHeight = 600
let windowWidth = 800
let (socketWidth, socketHeight) = 200, 100
let pieceCreationTimeout = System.TimeSpan.FromSeconds 1.


let rnd = new System.Random()

let randomPiece () = (float32(rnd.Next(50, 150)), float32(rnd.Next(40, 60)))

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
        
    let mutable sock = Unchecked.defaultof<Entity>
    let mutable socketBody = Unchecked.defaultof<Body>
    
    let mutable pieces : Entity list = []
    let mutable gameState = Running
    let mutable lastPieceCreatedAt = System.DateTime.Now
    
    let createTexture () =
        let t = new Texture2D(this.GraphicsDevice, 1, 1)
        t.SetData [| Color.White |]
        t
    
    let renderSocket s (sb: SpriteBatch) =
        let t = getComponentValue<Drawable> s
        let r = getComponentValue<StaticBody> s
        let p = socketBody.Position
        sb.Draw(t.Texture, new Rectangle(int(p.X), int(p.Y), r.Rect.Width, r.Rect.Height), Color.Beige)
    
    let renderPieces pieces (sb: SpriteBatch) = 
        for p in pieces do
            let body = getComponentValue<Bodyable>(p).Body
            let t = getComponentValue<Drawable>(p).Texture
            let s = getComponentValue<Sizeable>(p).Size
            let r = toRectangle body.Position.X body.Position.Y (fst(s)) (snd(s))
            sb.Draw(t, r, Color.IndianRed)
    
    let spawnNewPiece at =
        let now = System.DateTime.Now
        
        if now >= lastPieceCreatedAt.Add(pieceCreationTimeout)
        then
            let (w, h) = randomPiece()
            let b = BodyFactory.CreateRectangle(world, w, h, 1.f)
            b.Position <- at
            b.IsStatic <- false
            let e = (newEntity "piece" 
                |> addComponent {Body = b} 
                |> addComponent {Texture = createTexture()} 
                |> addComponent {Size = (w, h)})
            pieces <- e :: pieces
            lastPieceCreatedAt <- now
        
        
    let checkInput () =
        let mouseState = Mouse.GetState()
        if mouseState.LeftButton = ButtonState.Pressed
        then
            spawnNewPiece (new Vector2(float32(mouseState.Position.X), float32(mouseState.Position.Y)))
        
    override x.Draw(gametime: GameTime) =
        this.GraphicsDevice.Clear(Color.CornflowerBlue)
        spriteBatch.Begin()
        renderSocket sock spriteBatch |> ignore
        renderPieces pieces spriteBatch |> ignore
        spriteBatch.End()
        ()

    override x.Update(gameTime) =
        if gameState = Running
        then
            world.Step(0.001f * float32(gameTime.ElapsedGameTime.TotalMilliseconds))
            checkInput()
        ()
        
    override x.LoadContent () =
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        let texture = createTexture()
        let bounds = this.Window.ClientBounds
        let x = windowWidth / 2 - socketWidth / 2
        let y = windowHeight - socketHeight - 25
        let rect = new Rectangle(x, y, socketWidth, socketHeight)
        sock <- socket texture (rect)
        
        socketBody <- BodyFactory.CreateRectangle(world, float32(rect.Width), float32(rect.Height), 1.f, new Vector2(float32(x), float32(y)))
        ()


let runGame () =
    use g = new MyGame()
    g.Run()