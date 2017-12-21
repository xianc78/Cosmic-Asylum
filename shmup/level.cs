using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace shmup
{
	public class Level
	{
		public Game1 game;
		public Player player;
		public List<Enemy> enemyList;
		public Boss boss;
		public List<Bullet> bulletList;
		public List<Bullet> enemyBulletList;
		public List<PowerUp> powerUpList;
		public List<Tile> tileList;
		public List<Explosion> explosionList;
		public Song bgm;
		public Camera camera;
		public int levelLimit;
		public string fileName;

		public Level(string fileName, Game1 game)
		{
			this.game = game;
			this.fileName = fileName;
			init();
		}

		public void init()
		{
			// Create empty lists for game objects
			player = new Player(0, 0, this);
			camera = new Camera(this);
			enemyList = new List<Enemy>();
			bulletList = new List<Bullet>();
			enemyBulletList = new List<Bullet>();
			tileList = new List<Tile>();
			powerUpList = new List<PowerUp>();
			explosionList = new List<Explosion>();
			bgm = this.game.Content.Load<Song>("track" + game.levelNo);
			// Build the level
			buildLevel();
		}

		public void buildLevel()
		{
			using (StreamReader sr = new StreamReader(fileName))
			{
				// Local variables
				string line;
				int x = 0;
				int y = 0;

				// Take the first line of the file and use it to get the length of the level
				levelLimit = sr.ReadLine().Length * 32;
				sr.DiscardBufferedData();
				sr.BaseStream.Seek(0, SeekOrigin.Begin);

				// Start adding game objects
				while ((line = sr.ReadLine()) != null) {
					foreach (Char character in line)
					{
						switch (character)
						{
							case '.':
								break;
							case '#':
								tileList.Add(new Tile(x, y, false, this));
								break;
							case '|':
								tileList.Add(new Tile(x, y, true, this));
								break;
							case '+':
								powerUpList.Add(new OneUp(x, y, this));
								break;
							case 's':
								powerUpList.Add(new Spray(x, y, this));
								break;
							case '1':
								enemyList.Add(new Enemy1(x, y, this));
								break;
							case '2':
								enemyList.Add(new Enemy2(x, y, this));
								break;
							case '3':
								enemyList.Add(new Enemy3(x, y, this));
								break;
							case '4':
								enemyList.Add(new Enemy4(x, y, this));
								break;
							case '5':
								enemyList.Add(new Enemy5(x, y, this));
								break;
							case 'b':
								enemyList.Add(new Boss(x, y, this));
								break;
							case 't':
								enemyList.Add(new Turret(x, y, this));
								break;
							default:
								game.Exit();
								break;
								
						}
						x += 32;
					}
					x = 0;
					y += 32;
				}
			}

		}
	}
}