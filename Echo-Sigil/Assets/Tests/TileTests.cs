using NUnit.Framework;
using UnityEngine;
using MapEditor;

namespace Tile_Tests
{
    class initialization
    {
        [Test]

        public void new_tile_can_not_return_null()

        {
            Tile tile = new Tile(0, 0);
            Assert.IsNotNull(tile);
        }
        [Test]

        public void new_tile_can_hold_height_value()

        {
            Tile tile = new Tile(0, 0)
            {
                topHeight = 1
            };
            Assert.AreEqual(1, tile.topHeight);
        }
        [Test]

        public void new_tile_can_hold_walkable()

        {
            Tile tile = new Tile(0, 0)
            {
                walkable = true
            };
            Assert.IsTrue(tile.walkable);
        }
        [Test]

        public void pos_in_grid_is_acurate_0x0()

        {
            MapReader.GeneratePhysicalMap(new Map(1, 1));
            Assert.AreEqual(new Vector2Int(0, 0), MapReader.GetTile(0, 0, 0));
        }
        [Test]

        public void pos_in_grid_is_acurate_3x5()

        {
            MapReader.GeneratePhysicalMap(new Map(3, 5));
            Assert.AreEqual(new Vector2Int(2, 4), MapReader.GetTile(2,4,0));
        }
    }

    class texture
    {
        [Test]
        public void original_returns_original_texture()
        {
            Texture2D texture2D = new Texture2D(64, 64);
            Assert.AreEqual(texture2D, TileTextureManager.GetTileTexture(texture2D, TileTextureType.Original));
        }

        [Test]
        public void top_returns_square()
        {
            int random = Random.Range(2, 256);
            Texture2D texture2D = new Texture2D(random, random);
            texture2D = TileTextureManager.GetTileTextureSection(texture2D, TileTextureSection.Top);
            Assert.AreEqual(texture2D.width, texture2D.height);
        }

        [Test]
        public void get_tile_border_returns_10_percent()
        {
            int random = Random.Range(2, 256);
            Texture2D texture2D = new Texture2D(random, random + random / 10);
            int width = texture2D.width;
            texture2D = TileTextureManager.GetTileTextureSection(texture2D, TileTextureSection.Border);
            Assert.AreEqual(width / 10, texture2D.height);
        }

        [Test]
        public void dubug_returns_correct_placement()
        {
            int random = Random.Range(2, 256);
            Texture2D texture2D = new Texture2D(random, random * 3);
            int width = texture2D.width;
            texture2D = TileTextureManager.GetTileTexture(texture2D, TileTextureType.Debug);
            Assert.AreNotEqual(texture2D.GetPixel(0, width - 1), texture2D.GetPixel(0, width + 1));
            Assert.AreNotEqual(texture2D.GetPixel(0, width + (width / 10) - 1), texture2D.GetPixel(0, width + (width / 10) + 1));
            Assert.AreNotEqual(texture2D.GetPixel(0, texture2D.height), texture2D.GetPixel(0, (width * 2) - 1));
        }
    }
}
