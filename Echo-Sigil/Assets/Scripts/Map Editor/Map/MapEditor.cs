using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MapEditor
{
    public class MapEditor : MonoBehaviour
    {
        public static int modPathIndex;
        public static Sprite[] pallate;

        static Transform editorTileParent;

        static Transform selectedTransform;
        static List<Transform> selectedTransforms = new List<Transform>();

        private static bool locked;

        public static event Action<Transform> SelectedEvent;
        public static event Action<Transform[]> MultiSelectedEvent;

        public static TacticsMovementCamera selectionCamera;
        private void Start()
        {
            selectionCamera = Camera.main.GetComponent<TacticsMovementCamera>();
            Init();
        }

        private static void Init()
        {
            MapReader.MapGeneratedEvent += GenerateExpationTiles;
            MapReader.MapGeneratedEvent += SetPallate;
            MapReader.MapGeneratedEvent += ResetSelection;
        }

        private static void ResetSelection()
        {
            SelectedEvent?.Invoke(null);
        }

        private static void SetPallate()
        {
            pallate = MapReader.spritePallate;
        }

        public static void GenerateExpationTiles()
        {
            Sprite[] sprites = MapReader.spritePallate;
            if (editorTileParent == null)
            {
                editorTileParent = new GameObject("Editor Tiles").transform;
                editorTileParent.parent = MapReader.tileParent;
            }
            else
            {
                foreach (Transform transform in editorTileParent)
                {
                    Destroy(transform.gameObject);
                }
            }
            for (int x = -1; x <= MapReader.tiles.GetLength(0); x++)
            {
                for (int y = -1; y <= MapReader.tiles.GetLength(1); y++)
                {
                    if (MapReader.GetTile(x, y) == null)
                    {
                        CreateEditorTile(x, y, sprites[0]);
                    }
                }
            }
        }

        public static void SetModPathIndex()
        {

        }

        private static void CreateEditorTile(int x, int y, Sprite sprite) => CreateEditorTile(new Vector2Int(x, y), sprite);

        private static void CreateEditorTile(Vector2Int posInGrid, Sprite pallate)
        {
            GameObject editorTileObject = new GameObject(posInGrid.x + "," + posInGrid.y + " editor tile");
            editorTileObject.transform.parent = editorTileParent;
            editorTileObject.transform.position = MapReader.GridToWorldSpace(posInGrid);
            editorTileObject.tag = "Tile";

            SpriteRenderer sprite = editorTileObject.AddComponent<SpriteRenderer>();
            sprite.sprite = pallate;
            Color color = Color.cyan;
            color.a = .3f;
            sprite.color = color;

            editorTileObject.AddComponent<EditorTile>().posInGrid = posInGrid;

            editorTileObject.AddComponent<BoxCollider2D>();
        }

        void Update()
        {
            if (MapReader.tiles != null && MapReader.tiles.Length > 0 && !EventSystem.current.IsPointerOverGameObject())
            {
                Select();
                TileEdits();
                UnitEdits();
            }
        }

        public static void ChangeIsPlayer(bool player, Unit selectedImplement)
        {
            if (selectedImplement != null)
            {

                Transform transform = selectedImplement.transform;
                SpriteRenderer sprite = selectedImplement.unitSprite;

                TacticsMove t = selectedImplement.move as TacticsMove;
                int moveDistance = t.moveDistance;
                float moveSpeed = t.moveSpeed;
                float jumpHeight = t.jumpHeight;
                Destroy(t);

                JRPGBattle j = selectedImplement.battle as JRPGBattle;
                int maxHeath = j.maxHealth;
                int health = j.health;
                int maxWill = j.maxWill;
                int will = j.will;
                int reach = j.reach;
                Destroy(j);

                Destroy(selectedImplement);

                if (player)
                {
                    selectedImplement = transform.gameObject.AddComponent<PlayerUnit>();
                    selectedImplement.move = transform.gameObject.AddComponent<PlayerMove>();
                    selectedImplement.battle = transform.gameObject.AddComponent<PlayerBattle>();
                }
                else
                {
                    selectedImplement = transform.gameObject.AddComponent<NPCUnit>();
                    selectedImplement.move = transform.gameObject.AddComponent<NPCMove>();
                    selectedImplement.battle = transform.gameObject.AddComponent<NPCBattle>();
                }

                selectedImplement.unitSprite = sprite;

                t = selectedImplement.move as TacticsMove;
                t.moveDistance = moveDistance;
                t.moveSpeed = moveSpeed;
                t.jumpHeight = jumpHeight;

                j = selectedImplement.battle as JRPGBattle;
                j.maxHealth = maxHeath;
                j.health = health;
                j.maxWill = maxWill;
                j.will = will;
                j.reach = reach;

            }
        }

        public static bool ChangeTileHeight(float desierdHeight, Transform tileTransform)
        {
            if (tileTransform.TryGetComponent(out TileBehaviour tile))
            {
                Tile selectedTile = tile;
                if (desierdHeight >= 0)
                {
                    selectedTile.topHeight = desierdHeight;
                    tileTransform.position = new Vector3(tileTransform.position.x, tileTransform.position.y, desierdHeight);
                    //GenerateExpationTile(tileTransform.GetComponent<SpriteRenderer>().sprite,selectedTile);
                }
                else
                {
                    RemoveTile(tileTransform, selectedTile);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void ChangeTileHeight(float delta, Transform[] tileTransform)
        {
            Dictionary<Transform, Tile> tiles = new Dictionary<Transform, Tile>();
            List<Transform> tileTransforms = new List<Transform>();
            foreach (Transform t in tileTransform)
            {
                if (t.TryGetComponent(out TileBehaviour tile))
                {
                    tiles.Add(t, tile);
                    tileTransforms.Add(t);
                }
            }
            foreach (Transform t in tileTransforms)
            {
                tiles[t].topHeight += delta;
                t.position += new Vector3(0, 0, delta);
                if (tiles[t].topHeight < 0)
                {
                    RemoveTile(t, tiles[t]);
                }
            }
        }

        private static void RemoveTile(Transform tileTransform, Tile selectedTile)
        {
            MapReader.tiles[selectedTile.PosInGrid.x, selectedTile.PosInGrid.y] = null;
            Destroy(tileTransform.gameObject);
            CreateEditorTile(selectedTile.PosInGrid.x, selectedTile.PosInGrid.y, MapReader.spritePallate[0]);
        }

        public static void ChangeTileWalkable(bool walkable, Tile selectedTile) => selectedTile.walkable = walkable;

        public static void ChangeTileHeight(string delta, Tile selectedTile, Transform tileTransform) => ChangeTileHeight(float.Parse(delta) - selectedTile.topHeight, tileTransform);

        public static void ChangeUnitPos(Vector3 point, Unit selectedImplement) => ChangeUnitPos(MapReader.WorldToGridSpace(point), selectedImplement);

        public static void ChangeUnitPos(Vector2 point, Unit selectedImplement) => ChangeUnitPos(MapReader.WorldToGridSpace(point), selectedImplement);

        public static void ChangeUnitPos(int x, int y, Unit selectedImplement) => ChangeUnitPos(new Vector2Int(x, y), selectedImplement);

        public static void ChangeUnitPos(Vector2Int pointOnGrid, Unit selectedImplement)
        {
            Vector2 worldSpace = MapReader.GridToWorldSpace(pointOnGrid);
            Tile tile = MapReader.GetTile(pointOnGrid);
            if (tile != null)
            {
                selectedImplement.transform.position = new Vector3(worldSpace.x, worldSpace.y, tile.topHeight - .1f);
            }
            else
            {
                RemoveUnit(selectedImplement);
            }

            SelectedEvent?.Invoke(selectedTransform);
        }

        public static void ChangeHealthWill(bool health, int value, Unit implement)
        {
            JRPGBattle j = implement.battle as JRPGBattle;
            if (health)
            {
                j.health = value;
            }
            else
            {
                j.will = value;
            }
        }

        public static void ChangeNumVariable(string value, string variable, Unit selectedImplement)
        {
            TacticsMove t = selectedImplement.move as TacticsMove;
            JRPGBattle j = selectedImplement.battle as JRPGBattle;
            switch (variable)
            {
                case "Move Distance":
                    t.moveDistance = int.Parse(value);
                    break;
                case "Move Speed":
                    t.moveSpeed = float.Parse(value);
                    break;
                case "Jump Height":
                    t.jumpHeight = float.Parse(value);
                    break;
                case "Max Health":
                    j.maxHealth = int.Parse(value);
                    break;
                case "Max Will":
                    j.maxWill = int.Parse(value);
                    break;
                case "Reach":
                    j.reach = int.Parse(value);
                    break;
            }
        }

        public static Sprite ChangeTileTexture(int index, Tile selectedTile, SpriteRenderer spriteRenderer)
        {
            if (index >= pallate.Length)
            {
                index = 0;
            }
            else if (index < 0)
            {
                index = pallate.Length - 1;
            }
            selectedTile.spriteIndex = index;
            spriteRenderer.sprite = pallate[index];
            return pallate[index];
        }

        public static void AddUnit()
        {
            MapReader.MapImplementToUnit(new MapImplement(SaveSystem.LoadImplements(0)[0].splashInfo.name, MapReader.WorldToGridSpace(Vector2.zero)), 0);
        }

        public static void RemoveUnit(Unit selectedImplement)
        {
            if (MapReader.implements.Contains(selectedImplement))
            {
                MapReader.implements.Remove(selectedImplement);
            }
            Destroy(selectedImplement.gameObject);
            selectedTransform = null;
            SelectedEvent?.Invoke(null);
        }

        private static void Select()
        {
            Physics2D.queriesStartInColliders = true;
            RaycastHit2D hit = Physics2D.Raycast(selectionCamera.GetScreenPoint(Input.mousePosition), Vector2.one, .1f);
            if (hit.collider != null && ((!locked && hit.transform != selectedTransform) || Input.GetMouseButtonDown(0)))
            {
                if (hit.transform.TryGetComponent(out TileBehaviour _))
                {
                    //Add to/subtract from a multiselection
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        if (selectedTransforms.Contains(hit.transform))
                        {
                            selectedTransforms.Remove(hit.transform);
                        }
                        else
                        {
                            selectedTransforms.Add(hit.transform);
                        }

                        //Add currently selected tile to multiselection
                        if (selectedTransform != null)
                        {
                            selectedTransforms.Add(selectedTransform);
                            selectedTransform = null;
                        }
                        MultiSelectedEvent?.Invoke(selectedTransforms.ToArray());
                    } //select a single thing
                    else
                    {
                        selectedTransform = hit.collider.transform;
                        SelectedEvent?.Invoke(selectedTransform);
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        locked = true;
                    }
                }
                else if (hit.transform.TryGetComponent(out EditorTile et))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        et.AddTileToMapReader();
                    }
                    if (selectedTransforms.Count <= 0)
                    {
                        SelectedEvent(null);
                    }
                    if (!locked)
                    {
                        selectedTransform = null;
                        selectedTransforms.Clear();
                        SelectedEvent?.Invoke(selectedTransform);
                    }
                }
                else if (hit.transform.parent.TryGetComponent(out Unit implement))
                {
                    selectedTransform = hit.collider.transform;
                    SelectedEvent?.Invoke(selectedTransform);

                    if (Input.GetMouseButtonDown(0))
                    {
                        locked = true;
                    }
                }
            }
            else if (hit.collider == null)
            {
                if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
                {
                    locked = false;
                }
                if (!locked)
                {
                    selectedTransform = null;
                    selectedTransforms.Clear();
                    SelectedEvent?.Invoke(selectedTransform);
                }
            }
        }

        private static void UnitEdits()
        {
            if (selectedTransform != null && selectedTransform.parent.TryGetComponent(out Unit implement))
            {
                Vector3 point = selectionCamera.GetScreenPoint(Input.mousePosition);
                point.z += .1f;
                if (Input.GetMouseButtonUp(0))
                {
                    ChangeUnitPos(point, implement);
                    SelectedEvent(selectedTransform);
                }
                else if (Input.GetMouseButton(0))
                {
                    selectedTransform.parent.position = point;
                }
            }
        }

        private static void TileEdits()
        {
            if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
            {
                if (selectedTransforms.Count > 1)
                {
                    ChangeTileHeight(Input.GetAxisRaw("Mouse ScrollWheel"), selectedTransforms.ToArray());
                    MultiSelectedEvent?.Invoke(selectedTransforms.ToArray());
                }
                if (selectedTransform != null)
                {
                    ChangeTileHeight(selectedTransform.position.z + Input.GetAxisRaw("Mouse ScrollWheel"), selectedTransform);
                    SelectedEvent?.Invoke(selectedTransform);
                }
            }
        }
    } 
}
