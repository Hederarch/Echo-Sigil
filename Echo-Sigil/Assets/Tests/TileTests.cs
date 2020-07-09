using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tile_Tests
{
#pragma warning disable IDE1006 // Naming Styles
    class tile
#pragma warning restore IDE1006 // Naming Styles
    {
        [Test]
#pragma warning disable IDE1006 // Naming Styles
        public void new_tile_can_not_return_null()
#pragma warning restore IDE1006 // Naming Styles
        {
            Tile tile = new Tile(0,0);
            Assert.IsNotNull(tile);
        }
        [Test]
#pragma warning disable IDE1006 // Naming Styles
        public void new_tile_can_hold_height_value()
#pragma warning restore IDE1006 // Naming Styles
        {
            Tile tile = new Tile(0,0)
            {
                height = 1
            };
            Assert.AreEqual(1,tile.height);
        }
        [Test]
#pragma warning disable IDE1006 // Naming Styles
        public void new_tile_can_hold_walkable()
#pragma warning restore IDE1006 // Naming Styles
        {
            Tile tile = new Tile(0,0)
            {
                walkable = true
            };
            Assert.IsTrue(tile.walkable);
        }
        [Test]
#pragma warning disable IDE1006 // Naming Styles
        public void pos_in_grid_is_acurate_0x0()
#pragma warning restore IDE1006 // Naming Styles
        {
            Tile[,] tiles = MapReader.GeneratePhysicalMap(SaveSystem.LoadPallate(Application.dataPath + "/Quests/Tests"), new Map(1, 1));
            Assert.AreEqual(new Vector2Int(0, 0),tiles[0,0].PosInGrid);
        }
        [Test]
#pragma warning disable IDE1006 // Naming Styles
        public void pos_in_grid_is_acurate_3x5()
#pragma warning restore IDE1006 // Naming Styles
        {
            Tile[,] tiles = MapReader.GeneratePhysicalMap(SaveSystem.LoadPallate(Application.dataPath + "/Quests/Tests"), new Map(3, 5));
            Assert.AreEqual(new Vector2Int(2, 4), tiles[2, 4].PosInGrid);
        }
    }
}
