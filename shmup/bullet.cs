using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace shmup
{
	public class Bullet
	{
		protected Texture2D texture;
		public Rectangle rect;
		public int changex;
		public int changey;
		protected Level level;
		protected Game1 game;
		protected SoundEffect laserSound;
		public int hp;

		public Bullet(int x, int y, int changex, int changey, Level level)
		{
			this.level = level;
			this.game = this.level.game;
			texture = this.game.Content.Load<Texture2D>("bullet");
			laserSound = this.game.Content.Load<SoundEffect>("laserSound"); // Laser sound effect by dklon
			rect = new Rectangle(x, y, texture.Width, texture.Height);
			this.changex = changex;
			this.changey = changey;
			laserSound.Play();
		}

		public virtual void update()
		{
			rect.X += changex;
			rect.Y += changey;
		}

		public void draw(SpriteBatch spriteBatch)
		{
			Vector2 pos = new Vector2(rect.X - level.camera.rect.X, rect.Y - level.camera.rect.Y);
			spriteBatch.Draw(texture, position: pos);
			//spriteBatch.Draw(texture, destinationRectangle: rect);
		}
	}
	// This bullet can kill multiple enemies
	public class Laser : Bullet
	{
		public Laser(int x, int y, int changex, int changey, Level level) : base (x, y, changex, changey, level)
		{
			this.level = level;
			this.game = this.level.game;
			texture = this.game.Content.Load<Texture2D>("laser");
			laserSound = this.game.Content.Load<SoundEffect>("laserSound"); // Laser sound effect by dklon
			rect = new Rectangle(x, y, texture.Width, texture.Height);
			this.changex = changex;
			this.changey = changey;
			hp = 2;
			laserSound.Play();
		}

		public override void update()
		{
			base.update();
			if (hp <= 0)
			{
				level.bulletList.Remove(this);
			}
		}
	}
}