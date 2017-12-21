using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace shmup
{
	public class Tile
	{
	    Texture2D texture;
		public Rectangle rect;
		Level level;
		Game1 game;
		Boolean breakable;

		public Tile(int x, int y, Boolean breakable, Level level)
		{
			this.level = level;
			this.game = this.level.game;
			texture = this.game.Content.Load<Texture2D>("tileSet");
			rect = new Rectangle(x, y, 32, 32);
			this.breakable = breakable;
		}

		public void draw(SpriteBatch spriteBatch)
		{
			//Vector2 pos = new Vector2(rect.X - level.camera.rect.X, rect.Y - level.camera.rect.Y);
			Rectangle pos = new Rectangle(rect.X - level.camera.rect.X, rect.Y - level.camera.rect.Y, 32, 32);
			Rectangle imgRect;
			if (breakable)
			{
				imgRect = new Rectangle(0, 0, 32, 32);
			}
			else
			{
				imgRect = new Rectangle(32, 0, 32, 32);
			}
			spriteBatch.Draw(texture, destinationRectangle: pos, sourceRectangle: imgRect);
			//spriteBatch.Draw(texture, destinationRectangle: rect, sourceRectangle: new Rectangle(1792, 256, 32, 32));
		}

		public void update()
		{
			if (rect.Intersects(level.camera.rect)) {
				for (int i = level.bulletList.Count - 1; i >= 0; i--)
				{
					if (rect.Intersects(level.bulletList[i].rect))
					{
						if (breakable)
						{
							level.tileList.Remove(this);
						}
						level.bulletList.Remove(level.bulletList[i]);
					}
				}
			}
		}
	}
}