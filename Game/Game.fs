module Game.Core

open Microsoft.Xna.Framework

type MyGame () as this =
    inherit Game()
    
    let graphics = new GraphicsDeviceManager(this)
    
    do this.Window.Title <- "Stack that s%$t"
    do graphics.PreferredBackBufferWidth <- 800
    do graphics.PreferredBackBufferHeight <- 600
    do this.IsMouseVisible <- true
    
    override x.Draw(gametime: GameTime) =
        this.GraphicsDevice.Clear(Color.CornflowerBlue)
        ()

    override x.Update(gameTime) = 
        ()
        
    override x.LoadContent () =
        ()


let runGame () =
    use g = new MyGame()
    g.Run()