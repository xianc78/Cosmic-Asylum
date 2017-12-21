using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace shmup
{
	public class Enemy
	{
		protected Texture2D texture;
		public Rectangle rect;
		public int hp;
		public int score; // Score to give to player if killed
		// Vectors
		public int changex;
		public int changey;
		protected Level level;
		protected Game1 game;
		protected int shootTime;
		protected float shootTimer;
		protected float shootInterval = 500f;

		public virtual void update(GameTime gameTime)
		{
			if (rect.Intersects(level.camera.rect))
			{
				rect.X += changex;
				rect.Y += changey;
				for (int i = level.bulletList.Count - 1; i >= 0; i--)
				{
					if (rect.Intersects(level.bulletList[i].rect))
					{
						if (level.bulletList[i].GetType() == typeof(Laser))
						{
							level.bulletList[i].hp -= 1;
						}
						else
						{
							level.bulletList.RemoveAt(i);
						}
						hp -= 1;
					}
				}
				if (hp <= 0)
				{
					die(true);
				}

			}
		}

		public virtual void die(Boolean giveScore)
		{
			level.explosionList.Add(new Explosion(rect.X, rect.Y, this.level));
			if (giveScore)
			{
				level.game.score += score;
			}
			level.enemyList.Remove(this);
		}

		public virtual Boolean isLineOfSight()
		{
			if (rect.Center.Y < level.player.rect.Bottom && rect.Center.Y > level.player.rect.Top)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public void draw(SpriteBatch spriteBatch)
		{
			//Vector2 pos = new Vector2(rect.X - level.camera.rect.X, rect.Y - level.camera.rect.Y);
			Rectangle pos = new Rectangle(rect.X - level.camera.rect.X, rect.Y - level.camera.rect.Y, rect.Width, rect.Height);
			spriteBatch.Draw(texture, destinationRectangle: pos);
			//spriteBatch.Draw(texture, destinationRectangle: rect);
		}
	}

	// This enemy moves in a straight line
	public class Enemy1 : Enemy
	{
		public Enemy1(int x, int y, Level level)
		{
			this.level = level;
			this.game = this.level.game;
			texture = this.game.Content.Load<Texture2D>("enemyShip1");
			rect = new Rectangle(x, y, texture.Width, texture.Height);
			hp = 1;
			score = 100;
			changex = -4;
		}

	}
	// This enemy is just like enemy 1, but has more hp and shoots
	public class Enemy2 : Enemy
	{
		
		public Enemy2(int x, int y, Level level)
		{
			this.level = level;
			this.game = this.level.game;
			texture = this.game.Content.Load<Texture2D>("enemyShip2");
			rect = new Rectangle(x, y, texture.Width, texture.Height);
			hp = 2;
			score = 200;
			changex = -4;
		}

		public override void update(GameTime gameTime)
		{
			base.update(gameTime);
			if (isLineOfSight() && rect.Intersects(level.camera.rect))
			{
				shootTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
				if (shootTimer > shootInterval)
				{
					shoot();
					shootTimer = 0f;
				}
			}

			
		}

		public void shoot()
		{
			level.enemyBulletList.Add(new Bullet(rect.Left - 9, rect.Center.Y, -8, 0, this.level));
		}


	}

	// This enemy moves up and down
	public class Enemy3 : Enemy {
		int maxY;
		int minY;
		Random rand = new Random();

		public Enemy3(int x, int y, Level level)
		{
			this.level = level;
			this.game = this.level.game;
			this.texture = this.game.Content.Load<Texture2D>("enemyShip3");
			this.rect = new Rectangle(x, y, 32, 32);
			changey = 2;
			maxY = rect.Y + 10;
			minY = rect.Y - 10;
			hp = 1;
			score = 50;
		}

		public override void update(GameTime gameTime)
		{
			base.update(gameTime);
			if (rect.Y >= maxY || rect.Y <= minY)
			{
				changey *= -1;
			}
		}

		public override void die(Boolean giveScore)
		{
			switch (rand.Next(3))
			{
				case 0:
					level.powerUpList.Add(new OneUp(rect.X, rect.Y, this.level));
					break;
				case 1:
					level.powerUpList.Add(new Spray(rect.X, rect.Y, this.level));
					break;
				case 2:
					level.powerUpList.Add(new LaserGun(rect.X, rect.Y, this.level));
					break;
			}

			base.die(giveScore);
		}
	}

	// This enemy is invinsible and only serves as an obstacle
	public class Enemy4 : Enemy
	{
		SoundEffect deflectSound;
		public Enemy4(int x, int y, Level level)
		{
			this.level = level;
			this.game = level.game;
			texture = game.Content.Load<Texture2D>("enemyShip4");
			rect = new Rectangle(x, y, 32, 32);
			changex = -3;
			score = 500;
			hp = 1;
			deflectSound = game.Content.Load<SoundEffect>("deflect");

		}
		public override void update(GameTime gameTime)
		{
			if (rect.Intersects(level.camera.rect))
			{
				rect.X += changex;
				for (int i = level.bulletList.Count - 1; i >= 0; i--)
				{
					if (rect.Intersects(level.bulletList[i].rect))
					{
						if (level.bulletList[i].GetType() == typeof(Laser))
						{
							hp -= 1;
						}
						else
						{
							deflectSound.Play();
							level.bulletList.RemoveAt(i);
						}
					}
				}
				if (hp <= 0)
				{
					die(true);
				}
			}
		}

	}

	// This enemy moves in a sine curve
	public class Enemy5 : Enemy
	{
		int maxY;
		int minY;
		public Enemy5(int x, int y, Level level)
		{
			this.level = level;
			game = this.level.game;
			texture = this.game.Content.Load<Texture2D>("enemyShip3");
			rect = new Rectangle(x, y, 32, 32);
			changex = -3;
			changey = 2;
			maxY = rect.Y + 20;
			minY = rect.Y - 20;
			hp = 1;

		}

		public override void update(GameTime gameTime)
		{
			{
				base.update(gameTime);
				if (rect.Y >= maxY || rect.Y <= minY)
				{
					changey *= -1;
				}
			}

		}
	}

	// This enemy is stationary on the ground and can shoot diagonallly
	public class Turret : Enemy
	{
		public Turret(int x, int y, Level level)
		{
			this.level = level;
			game = this.level.game;
			texture = game.Content.Load<Texture2D>("turret");
			rect = new Rectangle(x, y, rect.Width, rect.Height);

		}

		public override void update(GameTime gameTime)
		{
			if (isLineOfSight())
			{
				shoot();
			}
		}

		public override Boolean isLineOfSight()
		{
			if (level.player.rect.Center.X > rect.Left || level.player.rect.Center.X < rect.Right)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public void shoot()
		{
			level.enemyBulletList.Add(new Bullet(rect.Center.X - 9, rect.Top - 9, 0, -8, level));
		}

	}

	// This enemy doesn't move and the ship and camera won't move until it is killed.
	public class Boss : Enemy
	{
		
		public Boss(int x, int y, Level level)
		{
			this.level = level;
			game = this.level.game;
			texture = game.Content.Load<Texture2D>("bossShip");
			rect = new Rectangle(x, y, 202, 244);
			changey = 3;
			hp = 30 * game.levelNo;
			score = 500;
		}

		public override void update(GameTime gameTime)
		{
			// Check to see if the entire ship is in the frame of view and pause the camera
			if (rect.Right <= level.camera.rect.Right)
			{
				level.camera.active = false;
			}
			base.update(gameTime);
			// 
			if (rect.Bottom >= 480 || rect.Top <= 0)
			{
				changey *= -1;
			}
			// Shoot if the ship is in line of sight with the player
			if (isLineOfSight() && rect.Intersects(level.camera.rect))
				{
					shootTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
					if (shootTimer > shootInterval)
					{
						shoot();
						shootTimer = 0f;
					}
				}
			}

		public void shoot()
		{
			level.enemyBulletList.Add(new Bullet(rect.Left - 9, rect.Top + 18, -8, 0, this.level));
			level.enemyBulletList.Add(new Bullet(rect.Left - 9, rect.Center.Y, -8, 0, this.level));
			level.enemyBulletList.Add(new Bullet(rect.Left - 9, rect.Bottom - 18, -8, 0, this.level));
		}

		public override void die(Boolean giveScore)
		{
			level.camera.active = true;
			base.die(giveScore);
			//game.nextLevel();
		}

	}
}