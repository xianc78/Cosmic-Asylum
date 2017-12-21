using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace shmup
{
	public class Player
	{
		Texture2D texture;
		public Rectangle rect;
		public int changex;
		public int changey;
		Game1 game;
		Level level;
		Boolean invinsible;
		//int invinsibilityTime;
		float invinsibilityTimer = 5f;
		float invinsibilityInterval = 500f;
		float maxInvinsTime = 5f;

		public Player (int x, int y, Level level) {
			this.level = level;
			this.game = this.level.game;
			this.texture = this.game.Content.Load<Texture2D>("playerShip");
			this.rect = new Rectangle(x, y, 48, 48);
		}

		public void update(GameTime gameTime)
		{
			if (invinsibilityTimer < maxInvinsTime)
			{
				invinsibilityTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
			rect.X += changex + level.camera.changex;

			if (rect.X < level.camera.rect.Left)
			{
				rect.X = level.camera.rect.Left;
			}
			else if (rect.Right > level.camera.rect.Right)
			{
				rect.X = level.camera.rect.Right - rect.Width;
			}
			rect.Y += changey;
			if (rect.Y < 0)
			{
				rect.Y = 0;
			}
			else if (rect.Bottom > Game1.SCREEN_HEIGHT)
			{
				rect.Y = Game1.SCREEN_HEIGHT - rect.Height;
			}

			for (int i = level.enemyList.Count - 1; i >= 0; i--)
			{
				if (rect.Intersects(level.enemyList[i].rect) && invinsibilityTimer >= maxInvinsTime) {
					die();
					level.enemyList[i].die(false);
					invinsibilityTimer = 0f;
					break;
				}
			}
			for (int i = level.enemyBulletList.Count - 1; i >= 0; i--)
			{
				if (rect.Intersects(level.enemyBulletList[i].rect) && invinsibilityTimer >= maxInvinsTime)
				{
					die();
					invinsibilityTimer = 0f;
					break;
				}
			}
			for (int i = level.tileList.Count - 1; i >= 0; i--)
			{
				if (rect.Intersects(level.tileList[i].rect) && invinsibilityTimer >= maxInvinsTime)
				{
					die();
					invinsibilityTimer = 0f;
					break;
				}
			}
			for (int i = level.powerUpList.Count - 1; i >= 0; i--)
			{
				if (rect.Intersects(level.powerUpList[i].rect))
				{
					if (level.powerUpList[i].GetType() == typeof(OneUp))
					{
						game.lives += 1;
					}
					else if (level.powerUpList[i].GetType() == typeof(ExtraHit))
					{
						invinsible = true;
						rect = new Rectangle(rect.X, rect.Y, 64, 64);
					}
					else if (level.powerUpList[i].GetType() == typeof(Spray))
					{
						game.playerState = "spray";
					}
					else if (level.powerUpList[i].GetType() == typeof(LaserGun))
					{
						game.playerState = "laser";
					}
					level.powerUpList.RemoveAt(i);
				}
			}

		}

		public void shoot()
		{
			if (game.playerState == "normal")
			{
				level.bulletList.Add(new Bullet(rect.Right + 1, rect.Center.Y, 8, 0, this.level));
			}
			else if (game.playerState == "spray")
			{
				level.bulletList.Add(new Bullet(rect.Right + 1, rect.Top - 9, 8, -6, this.level));
				level.bulletList.Add(new Bullet(rect.Right + 1, rect.Center.Y, 8, 0, this.level));
				level.bulletList.Add(new Bullet(rect.Right + 1, rect.Bottom, 8, 6, this.level));
			}
			else if (game.playerState == "laser")
			{
				level.bulletList.Add(new Laser(rect.Right + 1, rect.Center.Y, 8, 0, this.level));
			}
		}

		public void draw(SpriteBatch spriteBatch)
		{
			//Vector2 pos = new Vector2(rect.X - level.camera.rect.X, rect.Y - level.camera.rect.Y);
			Rectangle pos = new Rectangle(rect.X - level.camera.rect.X, rect.Y - level.camera.rect.Y, rect.Width, rect.Height);
			spriteBatch.Draw(texture, destinationRectangle: pos);
			//spriteBatch.Draw(texture, destinationRectangle: rect);
		}

		public void die()
		{
			level.explosionList.Add(new Explosion(rect.X, rect.Y, level));
			game.lives -= 1;
			game.playerState = "normal";
			rect.X = 0;
			rect.Y = Game1.SCREEN_HEIGHT / 2;
		}
	}
}