using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace shmup
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		// Screen sizes
		public const int SCREEN_WIDTH = 640;
		public const int SCREEN_HEIGHT = 480;
		//Lives and Score
		public int lives;
		public int score;
		//Get a list of levels from the Levels directory
		string[] levels = Directory.GetFiles(Directory.GetCurrentDirectory() + "/Levels", "level?.txt");
		public int levelNo;
		public Level currentLevel;
		Boolean debug = true;
		string mode;
		public string playerState;
		KeyboardState state;
		KeyboardState prevState;
		Texture2D background;
		SpriteFont titleFont;
		SpriteFont hudFont;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
			graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
			MediaPlayer.IsRepeating = true;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			MediaPlayer.Stop();
			mode = "title";
			lives = 3;
			score = 0;
			levelNo = 1;
			playerState = "normal";
			currentLevel = new Level(levels[levelNo - 1], this);
			background = this.Content.Load<Texture2D>("background");
			titleFont = this.Content.Load<SpriteFont>("titleFont");
			hudFont = this.Content.Load<SpriteFont>("hudFont");

			//currentLevel = new Level("level" + levelNo + ".txt", this);
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
#endif
			state = Keyboard.GetState();
			// Yeah I could have used enums but I really find no use for them
			switch (mode)
			{
				
				case "title":
					if (state.IsKeyDown(Keys.Enter) || state.IsKeyDown(Keys.Z))
					{
						MediaPlayer.Play(currentLevel.bgm);
						mode = "game";
					}
					else if (state.IsKeyDown(Keys.RightShift) && debug) {
						mode = "debug";
					}
					break;
				case "game":
					this.Window.Title = "Score: " + score + " | Lives: " + lives;
					// Check for key events
					currentLevel.player.changex = currentLevel.player.changey = 0; 

					if (state.IsKeyDown(Keys.Up))
					{
						currentLevel.player.changey = -6;
					}
					else if (state.IsKeyDown(Keys.Down))
					{
						currentLevel.player.changey = 6;
					}
					if (state.IsKeyDown(Keys.Left))
					{
						currentLevel.player.changex = -6;
					}
					else if (state.IsKeyDown(Keys.Right))
					{
						currentLevel.player.changex = 6;
					}
					if (state.IsKeyDown(Keys.Z) && !prevState.IsKeyDown(Keys.Z))
					{
						currentLevel.player.shoot();
					}
					if (state.IsKeyDown(Keys.Enter) && !prevState.IsKeyDown(Keys.Enter))
					{
						mode = "pause";
					}
					// Update game objects
					currentLevel.camera.update();
					currentLevel.player.update(gameTime);
					for (int i = currentLevel.enemyList.Count - 1; i >= 0; i--)
					{
						currentLevel.enemyList[i].update(gameTime);
					}
					for (int i = currentLevel.explosionList.Count - 1; i >= 0; i--)
					{
						currentLevel.explosionList[i].update(gameTime);
					}
					for (int i = currentLevel.tileList.Count - 1; i >= 0; i--)
					{
						currentLevel.tileList[i].update();
					}
					for (int i = currentLevel.bulletList.Count - 1; i >= 0; i--)
					{
						currentLevel.bulletList[i].update();
					}
					for (int i = currentLevel.enemyBulletList.Count - 1; i >= 0; i--)
					{
						currentLevel.enemyBulletList[i].update();
					}

					if (lives < 0)
					{
						mode = "gameover";
					}
					if (currentLevel.player.rect.Right >= currentLevel.levelLimit)
					{
						nextLevel();
					}
					break;
				case "gameover":
					if (state.IsKeyDown(Keys.Enter))
					{
						Initialize();
					}
					break;
				case "levelclear":
					if (state.IsKeyDown(Keys.Enter) && !prevState.IsKeyDown(Keys.Enter))
					{
						mode = "game";
						MediaPlayer.Play(currentLevel.bgm);
					}
					break;
				case "pause":
					if (state.IsKeyDown(Keys.Enter) && !prevState.IsKeyDown(Keys.Enter))
					{
						mode = "game";
					}
					break;
				case "debug":
					if (state.IsKeyDown(Keys.Left) && !prevState.IsKeyDown(Keys.Left))
					{
						levelNo--;
					}
					else if (state.IsKeyDown(Keys.Right) && !prevState.IsKeyDown(Keys.Right))
					{
						levelNo++;
					}
					else if (state.IsKeyDown(Keys.Enter) && !prevState.IsKeyDown(Keys.Enter))
					{
						currentLevel = new Level(levels[levelNo - 1], this);
						MediaPlayer.Play(currentLevel.bgm);
						mode = "game";
					}
					if (levelNo > levels.Length)
					{
						levelNo = levels.Length;
					}
					else if (levelNo < 1)
					{
						levelNo = 1;
					}
					break;
			}
			prevState = state;

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			spriteBatch.Begin();
			switch (mode)
			{
				case "title":
					graphics.GraphicsDevice.Clear(Color.Black);
					spriteBatch.DrawString(titleFont, "Cosmic Asylum", titleCenter("Cosmic Asylum"), Color.White);
					break;
				case "game":
					graphics.GraphicsDevice.Clear(Color.Black);
					spriteBatch.Draw(background, destinationRectangle: new Rectangle(0, 0 , SCREEN_WIDTH, SCREEN_HEIGHT)); //Needs a better background image
					for (int i = currentLevel.powerUpList.Count - 1; i >= 0; i--)
					{
						currentLevel.powerUpList[i].draw(spriteBatch);
					}
					for (int i = currentLevel.tileList.Count - 1; i >= 0; i--)
					{
						currentLevel.tileList[i].draw(spriteBatch);
					}
					currentLevel.player.draw(spriteBatch);
					for (int i = currentLevel.enemyList.Count - 1; i >= 0; i--)
					{
						currentLevel.enemyList[i].draw(spriteBatch);
					}
					for (int i = currentLevel.explosionList.Count - 1; i >= 0; i--)
					{
						currentLevel.explosionList[i].draw(spriteBatch);
					}
					for (int i = currentLevel.bulletList.Count - 1; i >= 0; i--)
					{
						int index = i;
						currentLevel.bulletList[index].draw(spriteBatch);
					}
					for (int i = currentLevel.enemyBulletList.Count - 1; i >= 0; i--)
					{
						currentLevel.enemyBulletList[i].draw(spriteBatch);
					}
					/*
					foreach (Enemy enemy in currentLevel.enemyList)
					{
						enemy.draw(spriteBatch);
					}

					foreach (Bullet bullet in currentLevel.bulletList)
					{
						bullet.draw(spriteBatch);
					}
					*/
					spriteBatch.DrawString(hudFont, "Lives: " + lives + " | Score: " + score + " | Level: " + levelNo, new Vector2(0, 0), Color.White);
					break;
				case "gameover":
					graphics.GraphicsDevice.Clear(Color.Black);
					spriteBatch.DrawString(titleFont, "Game Over", titleCenter("Game Over"), Color.White);
					break;
				case "levelclear":
					graphics.GraphicsDevice.Clear(Color.Black);
					spriteBatch.DrawString(titleFont, "Next Level: " + levelNo, titleCenter("Next Level: " + levelNo), Color.White);
					break;
				case "debug":
					graphics.GraphicsDevice.Clear(Color.Black);
					spriteBatch.DrawString(hudFont, "Get the fuck out of my debug mode, faggots", new Vector2(0, 0), Color.White);
					spriteBatch.DrawString(titleFont, "Level " + levelNo, titleCenter("Level " + levelNo), Color.White);
					break;
			}
			spriteBatch.End();
 
			base.Draw(gameTime);
		}

		void MediaPlayer_MediaStateChanged(object sender, System.
										   EventArgs e)
		{
			// 0.0f is silent, 1.0f is full volume
			MediaPlayer.Volume -= 0.1f;
			MediaPlayer.Play(currentLevel.bgm);
		}


		public void nextLevel()
		{
			
			levelNo += 1;
			try
			{
				currentLevel = new Level(levels[levelNo - 1], this);
				mode = "levelclear";
			}
			catch (IndexOutOfRangeException)
			{
				Initialize();
			}
		}

		// Get the center point for a title text.
		Vector2 titleCenter(string text)
		{
			int halfWidth = (int)titleFont.MeasureString(text).X / 2;
			int halfHeight = (int)titleFont.MeasureString(text).Y / 2;
			int x = SCREEN_WIDTH/2 - halfWidth;
			int y = SCREEN_HEIGHT/2 - halfHeight;
			return new Vector2(x, y);
		}
	}
}
