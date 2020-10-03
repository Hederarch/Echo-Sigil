using MapEditor;
using NUnit.Framework;
using UnityEngine;

namespace Map_Tests
{
    class map_reader
    {
       [Test]
       public void genarates_map_with_tile_with_hieght()
        {
            Tile[,] tiles = new Tile[1, 1];
            Tile tile = new Tile(0, 0)
            {
                height = 2
            };
            tiles[0, 0] = tile;
            Tile[,] outTiles = MapReader.GeneratePhysicalMap(new Map(tiles));
            Assert.AreEqual(2, outTiles[0,0].height);
        }
        [Test]
        public void genarates_map_with_tile()
        {
            Tile[,] tiles = new Tile[1, 1];
            Tile tile = new Tile(0, 0)
            {
                height = 2
            };
            tiles[0, 0] = tile;
            Tile[,] outTiles = MapReader.GeneratePhysicalMap(new Map(tiles));
            Assert.IsNotNull(outTiles[0, 0]);
        }
        [Test]
        public void genarates_map_with_unit()
        {
            Map map = new Map(1, 1);
            map.units = new System.Collections.Generic.List<MapImplement>();
            map.units.Add(new MapImplement());
            MapReader.GeneratePhysicalMap(map);
            Assert.IsNotNull(MapReader.implements[0]);
        }
        [Test]
        public void genarates_map_with_nonNull_unit_array()
        {
            MapReader.GeneratePhysicalMap(new Map(1, 1));
            Assert.IsNotNull(MapReader.implements);
        }
        [Test]
        public void genarates_map_with_nonNull_tile_array()
        {
            Tile[,] outTiles = MapReader.GeneratePhysicalMap();
            Assert.IsNotNull(outTiles);
        }
        [Test]
        public void tile_parent_has_child()
        {
            MapReader.GeneratePhysicalMap();
            Assert.Greater(MapReader.tileParent.childCount,0);
        }
        [Test]
        public void generates_tile_array_of_map_size()
        {
            Vector2Int size = new Vector2Int(3, 5);
            Tile[,] tiles = MapReader.GeneratePhysicalMap(new Map(size));
            Assert.AreEqual(size, new Vector2Int(tiles.GetLength(0), tiles.GetLength(1)));
        }
        [Test]
        public void returns_mapReader_tiles()
        {
            Tile[,] tiles = MapReader.GeneratePhysicalMap();
            Assert.AreEqual(MapReader.tiles, tiles);
        }
        [Test]
        public void tile_sprites_1x1()
        {
            MapReader.GeneratePhysicalMap();
            Assert.AreEqual(Vector3.one, MapReader.tileParent.GetChild(0).lossyScale);
        }

    }
    class mapeditor
    {
        [Test]
        public void editor_tile_convertions_correct_size_negitivey()
        {
            MapReader.GeneratePhysicalMap();
            EditorTile.CorrectGridSize(Vector2Int.down);
            Assert.AreEqual(2, MapReader.tiles.GetLength(1));
        }
        [Test]
        public void editor_tile_convertions_correct_size_negitivex()
        {
            MapReader.GeneratePhysicalMap();
            EditorTile.CorrectGridSize(Vector2Int.left);
            Assert.AreEqual(2, MapReader.tiles.GetLength(0));
        }
        [Test]
        public void editor_tile_convertions_correct_size_positivey()
        {
            MapReader.GeneratePhysicalMap();
            EditorTile.CorrectGridSize(Vector2Int.up);
            Assert.AreEqual(2, MapReader.tiles.GetLength(1));
        }
        [Test]
        public void editor_tile_convertions_correct_size_positivex()
        {
            MapReader.GeneratePhysicalMap();
            EditorTile.CorrectGridSize(Vector2Int.right);
            Assert.AreEqual(2, MapReader.tiles.GetLength(0));
        }
        [Test]
        public void editor_tile_convertions_contains_starting_tile_negitivey()
        {
            MapReader.GeneratePhysicalMap();
            EditorTile.CorrectGridSize(Vector2Int.down);
            Assert.IsNotNull(MapReader.GetTile(0,1));
        }
        [Test]
        public void editor_tile_convertions_contains_starting_tile_negitivex()
        {
            MapReader.GeneratePhysicalMap();
            EditorTile.CorrectGridSize(Vector2Int.left);
            Assert.IsNotNull(MapReader.GetTile(1,0));
        }
        [Test]
        public void editor_tile_convertions_contains_starting_tile_positivey()
        {
            MapReader.GeneratePhysicalMap();
            EditorTile.CorrectGridSize(Vector2Int.up);
            Assert.IsNotNull(MapReader.GetTile(0,0));
        }
        [Test]
        public void editor_tile_convertions_contains_starting_tile_positivex()
        {
            MapReader.GeneratePhysicalMap();
            EditorTile.CorrectGridSize(Vector2Int.right);
            Assert.IsNotNull(MapReader.GetTile(0,0));
        }
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
            Assert.IsNotNull(map.heightMap[0,0]);
        }
        [Test]
        public void new_map_sets_size()
        {
            Map map = new Map(3, 5);
            Assert.AreEqual(new Vector2Int(3, 5), new Vector2Int(map.sizeX,map.sizeY));
        }
        [Test]
        public void new_map_overloads_equivalant()
        {
            Map intmap = new Map(3, 5);
            Map vecmap = new Map(new Vector2Int(3, 5));
            Assert.AreEqual(new Vector2Int(vecmap.sizeX,vecmap.sizeY), new Vector2Int(intmap.sizeX, intmap.sizeY));
        }
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
        public void HeightMapIsSize(int x, int y, bool isXAxis)
        {
            Map map = new Map(x, y);
            if (isXAxis)
            {
                Assert.AreEqual(map.sizeX, map.heightMap.GetLength(0));
            }
            else
            {
                Assert.AreEqual(map.sizeY, map.heightMap.GetLength(1));
            }
        }
    }
    class conversions
    {
        [Test]
        public void center_tile_is_at_0x0_1x1()
        {
            MapReader.GeneratePhysicalMap();
            Transform tileTransform = MapReader.tileParent.GetChild(0);
            Assert.AreEqual(Vector3.zero, tileTransform.position);
        }
        [Test]
        public void center_tile_is_at_0x0_3x3()
        {
            MapReader.GeneratePhysicalMap(new Map(3, 3));
            Transform tileTransform = MapReader.tileParent.Find("1,1 tile");
            Assert.AreEqual(Vector3.zero, tileTransform.position);
        }
        [Test]
        public void grid_to_world_space_3x3()
        {
            MapReader.GeneratePhysicalMap(new Map(3, 3));
            Assert.AreEqual(Vector2.one, MapReader.GridToWorldSpace(Vector2Int.zero));
        }
        [Test]
        public void world_to_grid_space_3x3_center()
        {
            MapReader.GeneratePhysicalMap(new Map(3, 3));
            Assert.AreEqual(Vector2Int.zero, MapReader.WorldToGridSpace(Vector2.one));
        }
        [Test]
        public void world_to_grid_space_3x3_1x1()
        {
            MapReader.GeneratePhysicalMap(new Map(3, 3));
            Assert.AreEqual(Vector2Int.zero, MapReader.WorldToGridSpace(Vector2.one + (Vector2.one * .4f)));
        }
        [Test]
        public void world_to_grid_space_3x3_0x0()
        {
            MapReader.GeneratePhysicalMap(new Map(3, 3));
            Assert.AreEqual(Vector2Int.zero, MapReader.WorldToGridSpace(Vector2.one - (Vector2.one * .4f)));
        }
        [Test]
        public void grid_to_world_space_with_tile_parent_move()
        {

            MapReader.GeneratePhysicalMap(new Map(3, 3));
            MapReader.tileParent.position += Vector3.one;
            Assert.AreEqual(Vector2.one * 2, MapReader.GridToWorldSpace(Vector2Int.zero));
        }
        [Test]
        public void world_to_grid_space_to_tile_0x0()
        {
            Tile[,] tiles = MapReader.GeneratePhysicalMap();
            Assert.AreEqual(tiles[0, 0], MapReader.GetTile(MapReader.WorldToGridSpace(0, 0)));
        }
    }
}
