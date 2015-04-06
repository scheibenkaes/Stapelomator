module Stapel.Core

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

open Dasein.Core

open Stapel.Entities

let windowHeight = 600
let windowWidth = 800

type MyGame () as this =
    inherit Game()
    
    let graphics = new GraphicsDeviceManager(this)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    
    do this.Window.Title <- "Stack that s%$t"
    do graphics.PreferredBackBufferWidth <- windowWidth
    do graphics.PreferredBackBufferHeight <- windowHeight
    
    do this.IsMouseVisible <- true
    
    let (socketWidth, socketHeight) = 200, 100
    
    let mutable sock = Unchecked.defaultof<Entity>
    
    let renderSocket s (sb: SpriteBatch) =
        let t = getComponentValue<Drawable> s
        let {Body = rect} = getComponentValue<StaticBody> s
        sb.Draw(t.Texture, rect, Color.Beige)
    

    override x.Draw(gametime: GameTime) =
        this.GraphicsDevice.Clear(Color.CornflowerBlue)
        spriteBatch.Begin()
        renderSocket sock spriteBatch |> ignore
        spriteBatch.End()
        ()

    override x.Update(gameTime) = 
        ()
        
    override x.LoadContent () =
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        let texture = new Texture2D(this.GraphicsDevice, 1, 1)
        texture.SetData [| Color.White |]
        let bounds = this.Window.ClientBounds
        let x = windowWidth / 2 - socketWidth / 2
        let y = windowHeight - socketHeight - 25
        sock <- socket texture (new Rectangle(x, y, socketWidth, socketHeight))
        
        ()


let runGame () =
    use g = new MyGame()
    g.Run()