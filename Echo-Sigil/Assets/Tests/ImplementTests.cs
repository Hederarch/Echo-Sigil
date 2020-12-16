using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Pathfinding;

namespace Implement_Tests
{
    namespace Unit
    {
        class turn_manager : ITurn
        {

            public turn_manager()
            {

            }

            public turn_manager(string tag)
            {
                Tag = tag;
            }

            public string Tag { get; set; }

            public void BeginTurn()
            {
                
            }

            public void EndTurn()
            {
                
            }

            public virtual Color GetTeamColor()
            {
                return Color.black;
            }

            public virtual Texture2D GetTeamTexture()
            {
                return null;
            }

            [Test]
            public void check_for_win_returns_win_on_player_win()
            {
                TurnManager.Reset();
                TurnManager.playerWinEvent += check_for_win;
                check_for_win_bool = false;
                turn_manager unit = new turn_manager(PlayerUnit.playerTag);
                TurnManager.AddUnit(unit);
                TurnManager.RemoveUnit(unit);
                Assert.IsTrue(check_for_win_bool);
                
            }

            private static bool check_for_win_bool = false;
            private void check_for_win(bool obj)
            {
                TurnManager.playerWinEvent -= check_for_win;
                check_for_win_bool = obj;
            }

            [Test]
            public void check_for_win_returns_lose_on_player_win()
            {
                TurnManager.Reset();
                TurnManager.playerWinEvent += check_for_win;
                check_for_win_bool = true;
                turn_manager unit = new turn_manager("Diffrent Tag");
                TurnManager.AddUnit(unit);
                TurnManager.RemoveUnit(unit);
                Assert.IsFalse(check_for_win_bool);

            }

        }
        class inistalization
        {
            [Test]
            public void mapreader_sets_spriterender_hieght_defualt()
            {
                TileMap.MapReader.GeneratePhysicalMap(new TileMap.Map(1, 1));
                Assert.AreEqual(0,TileMap.MapReader.implements[0].unitSprite.transform.position.z);
            }
        }
    }
    namespace Pathfinding
    {
        class a_star : IAStarItem<a_star>
        {
            static a_star[,] field;

            public Vector2Int posInGrid;

            public a_star(Vector2Int vector2Int)
            {
                posInGrid = vector2Int;
            }

            public a_star()
            {

            }

            public int F => G + H;

            public int G { get; set; }
            public int H { get; set; }
            public int HeapIndex { get; set; }
            public a_star parent { get; set; }

            public int weight { get; set; } = 1;

            public bool walkable { get; set; } = true;
            public int distance { get; set; } = 0;

            public int CompareTo(a_star other)
            {
                int compare = F.CompareTo(other.F);
                if (compare == 0)
                {
                    compare = H.CompareTo(other.H);
                }
                return -compare;
            }

            public a_star[] FindNeighbors()
            {
                List<a_star> a_Stars = new List<a_star>();

                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        if (Mathf.Abs(x) != Mathf.Abs(y))
                        {
                            a_star astar = FindNeighbor(new Vector2Int(x, y));
                            if (astar != null && astar.walkable)
                            {
                                a_Stars.Add(astar);
                            }
                        }
                    }
                }

                return a_Stars.ToArray();
            }

            public a_star FindNeighbor(Vector2Int direction)
            {
                int x = posInGrid.x + direction.x;
                int y = posInGrid.y + direction.y;
                if (x >= 0 && y >= 0 && x < field.GetLength(0) && y < field.GetLength(1))
                {
                    return field[x, y];
                }
                return null;
            }

            public int GetDistance(a_star other) => Mathf.Abs(posInGrid.x - other.posInGrid.x) + Mathf.Abs(posInGrid.y - other.posInGrid.y);

            public int GetMaxSize() => field.Length;

            public override string ToString()
            {
                return "a_star " + posInGrid;
            }

            public void MakeField(int x, int y)
            {
                a_star[,] field = new a_star[x, y];
                for (int xi = 0; xi < x; xi++)
                {
                    for (int yi = 0; yi < y; yi++)
                    {
                        field[xi, yi] = new a_star(new Vector2Int(xi, yi));
                    }
                }
                a_star.field = field;
            }

            [Test]
            public void short_path_sucsessful()
            {
                MakeField(3, 3);
                AStar<a_star> path = new AStar<a_star>(field[0, 0], field[0, 2], GetMaxSize());
                Assert.IsTrue(path.PathFound);
            }

            [Test]
            public void short_path_contains_middle_item()
            {
                MakeField(3, 3);
                AStar<a_star> path = new AStar<a_star>(field[0, 0], field[0, 2], GetMaxSize());
                IEnumerator<a_star> enumerator = path.GetPathEnum();
                List<a_star> a_Stars = new List<a_star>();
                while (enumerator.MoveNext())
                {
                    a_Stars.Add(enumerator.Current);
                }
                Assert.IsTrue(a_Stars.Contains(field[0, 1]));
            }

            [Test]
            public void short_path_does_not_contains_unwalkable_middle_item()
            {
                MakeField(3, 3);
                field[0, 1].walkable = false;
                AStar<a_star> path = new AStar<a_star>(field[0, 0], field[0, 2], GetMaxSize());
                IEnumerator<a_star> enumerator = path.GetPathEnum();
                List<a_star> a_Stars = new List<a_star>();
                while (enumerator.MoveNext())
                {
                    a_Stars.Add(enumerator.Current);
                }
                Assert.IsFalse(a_Stars.Contains(field[0, 1]));
            }

            [Test]
            public void long_path_sucsessful()
            {
                MakeField(10, 10);
                AStar<a_star> path = new AStar<a_star>(field[0, 0], field[9, 9], GetMaxSize());
                Assert.IsTrue(path.PathFound);
            }

            [Test]
            public void long_path_contains_middle_item()
            {
                MakeField(10, 10);
                AStar<a_star> path = new AStar<a_star>(field[0, 0], field[9, 9], GetMaxSize());
                IEnumerator<a_star> enumerator = path.GetPathEnum();
                List<a_star> a_Stars = new List<a_star>();
                while (enumerator.MoveNext())
                {
                    a_Stars.Add(enumerator.Current);
                }
                Assert.IsTrue(a_Stars.Contains(field[9, 8]));
            }

            [Test]
            public void long_path_does_not_contains_unwalkable_middle_item()
            {
                MakeField(10, 10);
                field[9, 8].walkable = false;
                AStar<a_star> path = new AStar<a_star>(field[0, 0], field[9, 9], GetMaxSize());
                IEnumerator<a_star> enumerator = path.GetPathEnum();
                List<a_star> a_Stars = new List<a_star>();
                while (enumerator.MoveNext())
                {
                    a_Stars.Add(enumerator.Current);
                }
                Assert.IsFalse(a_Stars.Contains(field[9, 8]));
            }

            [Test]
            public void long_path_fails_when_too_long()
            {
                MakeField(10, 10);
                AStar<a_star> path = new AStar<a_star>(field[0, 0], field[9, 9], 5);
                Assert.IsFalse(path.PathFound);
            }
        }
        class bfs : IPathItem<bfs>
        {
            static bfs[,] field;
            public Vector2Int posInGrid;

            public bfs(Vector2Int vector2Int)
            {
                posInGrid = vector2Int;
            }

            public bfs()
            {

            }

            public int distance { get; set; } = 0;
            public bfs parent { get; set; }

            public int weight { get; set; } = 1;

            public bool walkable { get; set; } = true;

            public bfs[] FindNeighbors()
            {
                List<bfs> bfss = new List<bfs>();

                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        if (Mathf.Abs(x) != Mathf.Abs(y))
                        {
                            bfs bfs = FindNeighbor(new Vector2Int(x, y));
                            if (bfs != null && bfs.walkable)
                            {
                                bfss.Add(bfs);
                            }
                        }
                    }
                }

                return bfss.ToArray();
            }

            public bfs FindNeighbor(Vector2Int direction)
            {
                int x = posInGrid.x + direction.x;
                int y = posInGrid.y + direction.y;
                if (x >= 0 && y >= 0 && x < field.GetLength(0) && y < field.GetLength(1))
                {
                    return field[x, y];
                }
                return null;
            }

            public void MakeField(int x, int y)
            {
                bfs[,] field = new bfs[x, y];
                for (int xi = 0; xi < x; xi++)
                {
                    for (int yi = 0; yi < y; yi++)
                    {
                        field[xi, yi] = new bfs(new Vector2Int(xi, yi));
                    }
                }
                bfs.field = field;
            }

            public override string ToString()
            {
                return "bfs " + posInGrid;
            }

            [Test]
            public void short_path_sucsessful()
            {
                MakeField(3, 3);
                BFS<bfs> path = new BFS<bfs>(field[0, 0], field[0, 2], 4);
                Assert.IsTrue(path.PathFound);
            }

            [Test]
            public void short_path_contains_middle_item()
            {
                MakeField(3, 3);
                BFS<bfs> path = new BFS<bfs>(field[0, 0], field[0, 2], 4);
                IEnumerator<bfs> enumerator = path.GetPathEnum();
                List<bfs> a_Stars = new List<bfs>();
                while (enumerator.MoveNext())
                {
                    a_Stars.Add(enumerator.Current);
                }
                Assert.IsTrue(a_Stars.Contains(field[0, 1]));
            }

            [Test]
            public void short_path_does_not_contains_unwalkable_middle_item()
            {
                MakeField(3, 3);
                field[0, 1].walkable = false;
                BFS<bfs> path = new BFS<bfs>(field[0, 0], field[0, 2], 4);
                IEnumerator<bfs> enumerator = path.GetPathEnum();
                List<bfs> a_Stars = new List<bfs>();
                while (enumerator.MoveNext())
                {
                    a_Stars.Add(enumerator.Current);
                }
                Assert.IsFalse(a_Stars.Contains(field[0, 1]));
            }

            [Test]
            public void long_path_sucsessful()
            {
                MakeField(10, 10);
                BFS<bfs> path = new BFS<bfs>(field[0, 0], field[9, 9], 100);
                Assert.IsTrue(path.PathFound);
            }

            [Test]
            public void long_path_does_not_contains_unwalkable_middle_item()
            {
                MakeField(10, 10);
                field[9, 8].walkable = false;
                BFS<bfs> path = new BFS<bfs>(field[0, 0], field[9, 9], 100);
                IEnumerator<bfs> enumerator = path.GetPathEnum();
                List<bfs> a_Stars = new List<bfs>();
                while (enumerator.MoveNext())
                {
                    a_Stars.Add(enumerator.Current);
                }
                Assert.IsFalse(a_Stars.Contains(field[9, 8]));
            }

            [Test]
            public void long_path_fails_when_too_long()
            {
                MakeField(10, 10);
                BFS<bfs> path = new BFS<bfs>(field[0, 0], field[9, 9], 10);
                Assert.IsFalse(path.PathFound);
            }
        }
    } 
}
