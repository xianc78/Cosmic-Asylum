using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace shmup
{
	public class Explosion
	{
		Texture2D texture;
		Rectangle rect;
		Level level;
		Game1 game;
		int life;
		float lifeTimer;
		float lifeInterval = 500f;
		SoundEffect explosionSound;

		public Explosion(int x, int y, Level level)
		{
			this.level = level;
			game = this.level.game;
			texture = game.Content.Load<Texture2D>("explosion");
			rect = new Rectangle(x, y, texture.Width, texture.Height);
			explosionSound = this.game.Content.Load<SoundEffect>("explosionSound");
			life = 2;
			explosionSound.Play();
		}

		public void update(GameTime gameTime)
		{
			lifeTimer += (float) gameTime.ElapsedGameTime.TotalMilliseconds;
			if (lifeTimer >= lifeInterval)
			{
				life -= 1;
				lifeTimer = 0;
			}
			if (life <= 0)
			{
				level.explosionList.Remove(this);
			}
		}

		public void draw(SpriteBatch spriteBatch)
		{
			Vector2 pos = new Vector2(rect.X - level.camera.rect.X, rect.Y - level.camera.rect.Y);
			spriteBatch.Draw(texture, position: pos);
			//spriteBatch.Draw(texture, destinationRectangle: rect);
		}
	}
}