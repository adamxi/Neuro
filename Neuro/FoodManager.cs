using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXFramework.Util;
using Neuro.Objects;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Neuro
{
	public static class FoodManager
	{
		private static GraphicsDevice graphicsDevice;
		public static List<GameObject> FoodObjects;
		public static List<GameObject> removed = new List<GameObject>();

		private static float timer;
		private static int foodSpawnInterval = 1000;

		private static Rectangle levelBoundry;

		public static void Init(GraphicsDevice graphicsDevice)
		{
			FoodManager.graphicsDevice = graphicsDevice;
			FoodObjects = new List<GameObject>();


			int w = graphicsDevice.BackBuffer.Width;
			int h = graphicsDevice.BackBuffer.Height;
			levelBoundry = new Rectangle(-w / 2, -h / 2, w, h);

			//for (int i = 0; i < 1; i++)
			//{
			//	SpawnFood();
			//}
		}

		public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			FoodObjects.ForEach(o => o.Draw(spriteBatch));
		}

		public static void Update(GameTime gameTime)
		{
			UpdateFoodSpawn(gameTime);

			removed.ForEach(f => FoodObjects.Remove(f));
			removed.Clear();
        }

		public static void UpdateFoodSpawn(GameTime gameTime)
		{
			float currentTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
			if (currentTime - timer >= foodSpawnInterval)
			{
				timer = currentTime;
				SpawnFood();
			}
		}

		public static void SpawnFood()
		{
			Food f = new Food();
			f.Position = Randomizer.Range(levelBoundry);
			FoodObjects.Add(f);
		}

		public static void Remove(GameObject food)
		{
			removed.Add(food);
        }
	}
}