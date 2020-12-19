using NUnit.Framework;
using UnityEngine;
using TileMap;

namespace Tile_Tests
{
    class initialization
    {
        [Test]

        public void new_tile_can_not_return_null()

        {
            Tile tile = new Tile(0, 0, 1, 0);
            Assert.IsNotNull(tile);
        }
        [Test]

        public void new_tile_can_hold_height_value()

        {
            Tile tile = new Tile(0, 0, 1, 0)
            {
                topHeight = 1
            };
            Assert.AreEqual(1, tile.topHeight);
        }
        [Test]

        public void new_tile_can_hold_walkable()

        {
            Tile tile = new Tile(0, 0, 1, 0)
            {
                walkable = true
            };
            Assert.IsTrue(tile.walkable);
        }
        [Test]

        public void pos_in_grid_is_acurate_0x0()

        {
            MapReader.GenerateVirtualMap(new Map(1, 1));
            Assert.AreEqual(new Vector2Int(0, 0), MapReader.GetTile(0, 0, 0, 2).posInGrid);
        }
        [Test]

        public void pos_in_grid_is_acurate_3x5()

        {
            MapReader.GenerateVirtualMap(new Map(3, 5));
            Assert.AreEqual(new Vector2Int(2, 4), MapReader.GetTile(2, 4, 0, 2).posInGrid);
        }
    }

    class get_neighbor
    {
        [Test]
        public void tiles_find_neighbor()
        {
            MapReader.GenerateVirtualMap(new Map(3, 3));
            ITile[] tiles = MapReader.GetTile(1, 1, 0, 2).FindNeighbors();
            Assert.IsNotNull(tiles);
            Assert.Greater(tiles.Length, 0);
        }
    }
}
