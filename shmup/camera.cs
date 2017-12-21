using System;
using Microsoft.Xna.Framework;

namespace shmup
{
	public class Camera
	{
		public Rectangle rect;
		public int changex;
		Level level;
		Game1 game;
		public Boolean active = true;

		public Camera(Level level)
		{
			this.level = level;
			this.game = this.level.game;
			rect = new Rectangle(0, 0, 640, 480);
			//changex = 1;
		}

		public void update()
		{
			rect.X += changex;
			if (active)
			{
				changex = 1;
			}
			else
			{
				changex = 0;
			}
		}
	}
}