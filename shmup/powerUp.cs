using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace shmup
{
	public class PowerUp
	{
		protected Texture2D texture;
		public Rectangle rect;
		protected Level level;
		protected Game1 game;

		public void draw(SpriteBatch spriteBatch)
		{
			Vector2 pos = new Vector2(rect.X - level.camera.rect.X, rect.Y - level.camera.rect.Y);
			spriteBatch.Draw(texture, position: pos);
			//spriteBatch.Draw(texture, destinationRectangle: rect);
		}
	}

	public class OneUp : PowerUp
	{
		public OneUp(int x, int y, Level level)
		{
			this.level = level;
			this.game = this.level.game;
			texture = game.Content.Load<Texture2D>("heart");
			rect = new Rectangle(x, y, texture.Width, texture.Height);
		}
	}

	public class ExtraHit : PowerUp
	{
		public ExtraHit(int x, int y, Level level)
		{
			this.level = level;
			this.game = this.level.game;
		}
	}

	/*
	public class Invinsibility : PowerUp
	{

	}
	*/

	// With this power up, you can shot three bullets at once going in different directions
	public class Spray : PowerUp
	{
		public Spray(int x, int y, Level level)
		{
			this.level = level;
			this.game = this.level.game;
			texture = game.Content.Load<Texture2D>("spray");
			rect = new Rectangle(x, y, 32, 32);
		}
	}

	public class LaserGun : PowerUp
	{
		public LaserGun(int x, int y, Level level)
		{
			this.level = level;
			this.game = this.level.game;
			texture = game.Content.Load<Texture2D>("laserPU");
			rect = new Rectangle(x, y, 32, 32);
		}
	}
}