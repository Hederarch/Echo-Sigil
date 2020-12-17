using TileMap;
using NUnit.Framework;
using UnityEngine;

namespace Map_Tests
{
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
            Tile tile = MapTile.ConvertTile(map.mapTiles[0], 0, 0);
            Assert.IsTrue(tile.walkable);
        }
        [Test]
        public void new_map_sets_size()
        {
            Map map = new Map(3, 5);
            Assert.AreEqual(new Vector2Int(3, 5), new Vector2Int(map.sizeX, map.sizeY));
        }
        [Test]
        public void new_map_overloads_equivalant()
        {
            Map intmap = new Map(3, 5);
            Map vecmap = new Map(new Vector2Int(3, 5));
            Assert.AreEqual(new Vector2Int(vecmap.sizeX, vecmap.sizeY), new Vector2Int(intmap.sizeX, intmap.sizeY));
        }
    }
    namespace Convertions
    {
        class grid_to_world_space
        {
            [Test]
            public void grid_to_world_space_3x3()
            {
                MapReader.GenerateVirtualMap(new Map(3, 3));
                Assert.AreEqual(Vector3.one, MapReader.GridToWorldSpace(0, 0, 1));
            }
            [Test]
            public void grid_to_world_space_with_tile_parent_move()
            {
                MapReader.GenerateVirtualMap(new Map(3, 3));
                MapReader.tileParent.position += Vector3.one;
                Assert.AreEqual(Vector3.one * 2 - Vector3.forward, MapReader.GridToWorldSpace(0, 0, 1));
            }
        }
        class world_to_grid_space
        {
            [Test]
            public void world_to_grid_space_3x3_center()
            {
                MapReader.GenerateVirtualMap(new Map(3, 3));
                Assert.AreEqual(Vector2Int.zero, MapReader.WorldToGridSpace(Vector3.one));
            }
            [Test]
            public void world_to_grid_space_3x3_1x1()
            {
                MapReader.GenerateVirtualMap(new Map(3, 3));
                Vector3 posInWorld = MapReader.GetTile(1, 1, 0, 2).PosInWorld;
                TilePos actual = MapReader.WorldToGridSpace(posInWorld);
                Assert.AreEqual(Vector2Int.one, actual);
            }
            [Test]
            public void world_to_grid_space_3x3_0x0()
            {
                MapReader.GenerateVirtualMap(new Map(3, 3));
                Vector3 posInWorld = MapReader.GetTile(0, 0, 0, 2).PosInWorld;
                TilePos actual = MapReader.WorldToGridSpace(posInWorld);
                Assert.AreEqual(Vector2Int.zero, actual);
            }
        }
        class out_of_bounds
        {
            class world_to_grid_space
            {
                [Test]
                public void returns_null_positive_x()
                {
                    MapReader.GenerateVirtualMap(new Map(5, 5));
                    Assert.Null(MapReader.GetTile(MapReader.WorldToGridSpace(new Vector3(3, 0, 1))));
                }
                [Test]
                public void returns_null_positive_y()
                {
                    MapReader.GenerateVirtualMap(new Map(5, 5));
                    Assert.Null(MapReader.GetTile(MapReader.WorldToGridSpace(new Vector3(0, 3, 1))));
                }
                [Test]
                public void returns_null_negitive_x()
                {
                    MapReader.GenerateVirtualMap(new Map(5, 5));
                    Assert.Null(MapReader.GetTile(MapReader.WorldToGridSpace(new Vector3(-3, 0, 1))));
                }
                [Test]
                public void returns_null_negitive_y()
                {
                    MapReader.GenerateVirtualMap(new Map(5, 5));
                    Assert.Null(MapReader.GetTile(MapReader.WorldToGridSpace(new Vector3(0,-3,1))));
                }
            }

            [Test]
            public void returns_null_positive_x()
            {
                MapReader.GenerateVirtualMap(new Map(5, 5));
                Assert.Zero(MapReader.GetTiles(5, 0).Length);
                Assert.Null(MapReader.GetTile(5, 0, 0, 1));
            }
            [Test]
            public void returns_null_positive_y()
            {
                MapReader.GenerateVirtualMap(new Map(5, 5));
                Assert.Zero(MapReader.GetTiles(0, 5).Length);
                Assert.Null(MapReader.GetTile(0, 5, 0, 1));
            }
            [Test]
            public void returns_null_negitive_x()
            {
                MapReader.GenerateVirtualMap(new Map(5, 5));
                Assert.Zero(MapReader.GetTiles(-1, 0).Length);
                Assert.Null(MapReader.GetTile(-1, 0, 0, 1));
            }
            [Test]
            public void returns_null_negitive_y()
            {
                MapReader.GenerateVirtualMap(new Map(5, 5));
                Assert.Zero(MapReader.GetTiles(0, -1).Length);
                Assert.Null(MapReader.GetTile(0, -1, 0, 1));
            }
        }
    }
}
