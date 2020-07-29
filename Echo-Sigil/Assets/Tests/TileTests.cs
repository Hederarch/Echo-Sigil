using NUnit.Framework;
using UnityEngine;
using mapEditor;

namespace Tile_Tests
{
    class tile
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
                height = 1
            };
            Assert.AreEqual(1, tile.height);
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
            Tile[,] tiles = MapReader.GeneratePhysicalMap(SaveSystem.LoadPallate(Application.dataPath + "/Quests/Tests"), new Map(1, 1));
            Assert.AreEqual(new Vector2Int(0, 0), tiles[0, 0].PosInGrid);
        }
        [Test]

        public void pos_in_grid_is_acurate_3x5()

        {
            Tile[,] tiles = MapReader.GeneratePhysicalMap(SaveSystem.LoadPallate(Application.dataPath + "/Quests/Tests"), new Map(3, 5));
            Assert.AreEqual(new Vector2Int(2, 4), tiles[2, 4].PosInGrid);
        }
    }
}
