using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Map_Tests
{
    class map_reader
    {
        [Test]
        public void height_map_is_map_sizex_1x1()
        {
            HeightMapIsSize(1, 1, true);
        }
        [Test]
        public void height_map_is_map_sizey_1x1()
        {
            HeightMapIsSize(1, 1, false);
        }
        [Test]
        public void height_map_is_map_sizex_3x5()
        {
            HeightMapIsSize(3, 5, true);
        }
        [Test]
        public void height_map_is_map_sizey_3x5()
        {
            HeightMapIsSize(3, 5, false);
        }
        public void HeightMapIsSize(int x,int y,bool isXAxis)
        {
            Map map = new Map(x,y);
            if (isXAxis)
            {
                Assert.AreEqual(map.size.x, map.heightmap.GetLength(0));
            } else
            {
                Assert.AreEqual(map.size.y, map.heightmap.GetLength(1));
            }
        }
    }
    class mapeditor
    {

    }
    class map
    {
        [Test]
        public void map_can_not_return_null_1x1()
        {
            Map map = new Map(1, 1);
            Assert.IsNotNull(map);
        }
        [Test]
        public void map_can_not_return_null_3x5()
        {
            Map map = new Map(3, 5);
            Assert.IsNotNull(map);
        }
        [Test]
        public void new_maps_is_walkable()
        {
            Map map = new Map(1, 1);
            Tile tile = map.SetTileProperties(0,0);
            Assert.IsTrue(tile.walkable);
        }
        [Test]
        public void set_tile_properties_does_not_return_null_at_0x0()
        {
            Map map = new Map(1, 1);
            Assert.IsNotNull(map.SetTileProperties(0, 0));
        }
        [Test]
        public void set_tile_properties_does_not_return_null_at_1x1()
        {
            Map map = new Map(2, 2);
            Assert.IsNotNull(map.SetTileProperties(1, 1));
        }
        [Test]
        public void new_map_has_heigtmap_data()
        {
            Map map = new Map(1, 1);
            Assert.IsNotNull(map.heightmap[0,0]);
        }
        [Test]
        public void new_map_sets_size()
        {
            Map map = new Map(3, 5);
            Assert.AreEqual(new Vector2Int(3, 5), map.size);
        }
        [Test]
        public void new_map_overloads_equivalant()
        {
            Map intmap = new Map(3, 5);
            Map vecmap = new Map(new Vector2Int(3, 5));
            Assert.AreEqual(vecmap.size, intmap.size);
        }
    }
}

