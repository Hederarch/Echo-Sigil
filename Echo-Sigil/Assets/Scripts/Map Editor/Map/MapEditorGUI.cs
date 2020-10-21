using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor
{
    public class MapEditorGUI : MonoBehaviour
    {
        public Image SelectedImage;
        public Button SelectedButton;
        public Sprite plus;
        public GameObject padding;

        //tile
        public GameObject tileGUI;
        public Text tileName;
        public Button leftTileButton;
        public Button rightTileButton;
        private bool tileTrueIndex = true;
        public Toggle walkable;
        public InputField height;
        List<Tile> prevTile;

        //unit
        public Button addUnit;
        public GameObject unitGUI;
        public Text unitName;
        public Button leftUnitButton;
        public Button rightUnitButton;
        public Toggle player;
        public InputField unitPosX;
        public InputField unitPosY;
        public InputField moveDistance;
        public InputField moveSpeed;
        public InputField jumpHeight;
        public InputField maxHealth;
        public Slider health;
        public InputField maxWill;
        public Slider will;
        public InputField reach;

        // Start is called before the first frame update
        void Start()
        {
            InitSubscribe();
        }

        private void InitSubscribe()
        {
            MapEditor.SelectedEvent += UpdatePanel;
            MapEditor.MultiSelectedEvent += UpdatePanel;
            MapEditor.SelectedEvent += ResetPrevTile;
            MapEditor.MultiSelectedEvent += ResetPrevTile;
            MapReader.MapGeneratedEvent += delegate { addUnit.gameObject.SetActive(true); };

        }

        private void OnDestroy()
        {
            MapEditor.SelectedEvent -= UpdatePanel;
            MapEditor.MultiSelectedEvent -= UpdatePanel;
            MapEditor.SelectedEvent -= ResetPrevTile;
            MapEditor.MultiSelectedEvent -= ResetPrevTile;
            MapReader.MapGeneratedEvent -= delegate { addUnit.gameObject.SetActive(true); };

            UnSubscribeTile();
            UnSubscribeUnit();
        }

        private void ResetPrevTile(Transform selected)
        {
            Transform[] selectedArray = new Transform[1] { selected };
            ResetPrevTile(selectedArray);
        }

        private void ResetPrevTile(Transform[] selected)
        {
            if (prevTile != null && prevTile.Count > 0)
            {
                foreach (Tile t in prevTile)
                {
                    t.current = false;
                }
            }
            if (selected != null && selected.Length > 0)
            {
                prevTile = new List<Tile>();
                foreach (Transform t in selected)
                {
                    if (t != null && t.GetComponent<TileBehaviour>() != null)
                    {
                        Tile selTile = t.GetComponent<TileBehaviour>();
                        selTile.current = true;
                        prevTile.Add(selTile);
                    }
                }
            }
            else
            {
                prevTile = null;
            }
        }

        void UpdatePanel(Transform selected)
        {
            UnSubscribeTile();
            UnSubscribeUnit();
            if (selected != null)
            {
                Transform transform = selected;
                SelectedImage.color = Color.white;
                SelectedImage.sprite = transform.GetComponent<SpriteRenderer>().sprite;
                padding.SetActive(false);
                if (transform.TryGetComponent(out TileBehaviour tile))
                {
                    SetTileProperties(selected, tile);
                    SubscribeTile(selected, tile);
                }
                else if (transform.parent.TryGetComponent(out Unit implement))
                {
                    SetUnitPanelProperties(implement);
                    SubscribeUnit(implement);
                }
            }
            else
            {
                SelectedImage.color = Color.clear;
                tileGUI.SetActive(false);
                unitGUI.SetActive(false);
                padding.SetActive(true);
            }
        }

        private void UpdatePanel(Transform[] selected)
        {
            UnSubscribeTile();
            UnSubscribeUnit();
            if (selected != null && selected.Length > 1)
            {
                SelectedImage.color = Color.clear;
                List<Tile> tiles = new List<Tile>();
                List<Transform> transforms = new List<Transform>();
                foreach (Transform transform in selected)
                {
                    Tile tile = transform.GetComponent<TileBehaviour>();
                    if (tile != null)
                    {
                        tiles.Add(tile);
                        transforms.Add(transform);
                    }
                }
                if (selected != null && tiles.Count > 1)
                {
                    SelectedImage.color = Color.clear;
                    padding.SetActive(false);
                    SetTileProperties(transforms.ToArray(), tiles.ToArray());
                    SubscribeTile(transforms.ToArray(), tiles.ToArray());
                }
                else
                {
                    tileGUI.SetActive(false);
                    unitGUI.SetActive(false);
                    padding.SetActive(true);
                }
            }
            else if (selected != null && selected.Length == 1)
            {
                UpdatePanel(selected[0]);
            }
            else
            {
                SelectedImage.color = Color.clear;
                tileGUI.SetActive(false);
                unitGUI.SetActive(false);
                padding.SetActive(true);
            }
        }

        private void SubscribeTile(Transform[] selecteds, Tile[] selectedTiles)
        {
            SpriteRenderer[] spriteRenderers = new SpriteRenderer[selecteds.Length];
            for (int i = 0; i < selecteds.Length; i++)
            {
                spriteRenderers[i] = selecteds[i].GetComponent<SpriteRenderer>();
            }
            SelectedButton.onClick.AddListener(delegate { LoadSprite(selectedTiles, spriteRenderers); });
            for (int i = 0; i < selecteds.Length && i < selectedTiles.Length; i++)
            {
                Transform selected = selecteds[i];
                Tile selectedTile = selectedTiles[i];
                leftTileButton.onClick.AddListener(delegate { ChangeTileTexture(selectedTile.spriteIndex - 1, selectedTile, selected.GetComponent<SpriteRenderer>()); });
                rightTileButton.onClick.AddListener(delegate { ChangeTileTexture(selectedTile.spriteIndex - 1, selectedTile, selected.GetComponent<SpriteRenderer>()); });
                walkable.onValueChanged.AddListener(delegate { MapEditor.ChangeTileWalkable(walkable.isOn, selectedTile); });
                height.onEndEdit.AddListener(delegate { MapEditor.ChangeTileHeight(float.Parse(height.text), selected); });
            }
        }

        private void SubscribeTile(Transform selected, Tile selectedTile)
        {
            SelectedButton.onClick.AddListener(delegate { LoadSprite(selectedTile, selected.GetComponent<SpriteRenderer>()); });
            leftTileButton.onClick.AddListener(delegate { ChangeTileTexture(selectedTile.spriteIndex - 1, selectedTile, selected.GetComponent<SpriteRenderer>()); });
            rightTileButton.onClick.AddListener(delegate { ChangeTileTexture(selectedTile.spriteIndex + 1, selectedTile, selected.GetComponent<SpriteRenderer>()); });
            walkable.onValueChanged.AddListener(delegate { MapEditor.ChangeTileWalkable(walkable.isOn, selectedTile); });
            height.onEndEdit.AddListener(delegate { MapEditor.ChangeTileHeight(float.Parse(height.text), selected); });
        }

        private void ChangeTileTexture(int index, Tile selectedTile, SpriteRenderer spriteRenderer)
        {
            if ((index == MapEditor.pallate.Length || index == -1) && tileTrueIndex)
            {
                SelectedImage.sprite = plus;
                SelectedButton.enabled = true;
                tileTrueIndex = false;
            }
            else
            {
                if (!tileTrueIndex && index < MapEditor.pallate.Length && index >= 0)
                {
                    index = selectedTile.spriteIndex;
                }
                SelectedImage.sprite = MapEditor.ChangeTileTexture(index, selectedTile, spriteRenderer);
                SelectedButton.enabled = false;
                tileTrueIndex = true;
            }

        }

        private void SetTileProperties(Transform[] selected, Tile[] tile)
        {
            unitGUI.SetActive(false);
            if (selected.Length == 1 && tile.Length == 1)
            {
                SetTileProperties(selected[0], tile[0]);
            }
            else
            {
                int walkableYes = 0;
                int walkableNo = 0;
                Vector2Int upperTilePos = new Vector2Int(int.MinValue, int.MinValue);
                Vector2Int lowerTilePos = new Vector2Int(int.MaxValue, int.MaxValue);
                float heightAverage = tile[0].height;

                for (int i = 0; i < selected.Length && i < tile.Length; i++)
                {
                    Vector2Int pos = MapReader.WorldToGridSpace(selected[i].position);
                    if (upperTilePos.x < pos.x)
                    {
                        upperTilePos.x = pos.x;
                    }
                    if (upperTilePos.y < pos.y)
                    {
                        upperTilePos.y = pos.y;
                    }
                    if (lowerTilePos.x > pos.x)
                    {
                        lowerTilePos.x = pos.x;
                    }
                    if (lowerTilePos.y > pos.y)
                    {
                        lowerTilePos.y = pos.y;
                    }

                    if (tile[i].walkable)
                    {
                        walkableYes++;
                    }
                    else
                    {
                        walkableNo++;
                    }

                    heightAverage = (heightAverage + tile[i].height) / 2f;
                }

                tileName.text = upperTilePos.ToString() + " / " + lowerTilePos.ToString();
                if (walkableYes >= walkableNo)
                {
                    walkable.isOn = true;
                }
                else
                {
                    walkable.isOn = false;
                }
                height.text = heightAverage.ToString();

            }
            tileGUI.SetActive(true);
        }

        private void SetTileProperties(Transform selected, Tile tile)
        {
            unitGUI.SetActive(false);
            tileName.text = selected.name;
            walkable.isOn = tile.walkable;
            height.text = tile.height.ToString();
            tileGUI.SetActive(true);
        }

        private void UnSubscribeTile()
        {
            SelectedButton.enabled = false;
            SelectedButton.onClick.RemoveAllListeners();
            leftTileButton.onClick.RemoveAllListeners();
            rightTileButton.onClick.RemoveAllListeners();
            tileTrueIndex = true;
            walkable.onValueChanged.RemoveAllListeners();
            height.onEndEdit.RemoveAllListeners();
        }

        private void SubscribeUnit(Unit implement)
        {
            player.onValueChanged.AddListener(delegate { MapEditor.ChangeIsPlayer(player.isOn, implement); });
            unitPosX.onEndEdit.AddListener(delegate { MapEditor.ChangeUnitPos(int.Parse(unitPosX.text), int.Parse(unitPosY.text), implement); });
            unitPosY.onEndEdit.AddListener(delegate { MapEditor.ChangeUnitPos(int.Parse(unitPosX.text), int.Parse(unitPosY.text), implement); });
            moveDistance.onEndEdit.AddListener(delegate { MapEditor.ChangeNumVariable(moveDistance.text, "Move Distance", implement); });
            moveSpeed.onEndEdit.AddListener(delegate { MapEditor.ChangeNumVariable(moveSpeed.text, "Move Speed", implement); });
            jumpHeight.onEndEdit.AddListener(delegate { MapEditor.ChangeNumVariable(jumpHeight.text, "Jump Height", implement); });
            maxHealth.onEndEdit.AddListener(delegate { MapEditor.ChangeNumVariable(maxHealth.text, "Max Health", implement); });
            health.onValueChanged.AddListener(delegate { MapEditor.ChangeHealthWill(true, (int)health.value, implement); });
            maxWill.onEndEdit.AddListener(delegate { MapEditor.ChangeNumVariable(maxWill.text, "Max Will", implement); });
            will.onValueChanged.AddListener(delegate { MapEditor.ChangeHealthWill(false, (int)will.value, implement); });
            reach.onEndEdit.AddListener(delegate { MapEditor.ChangeNumVariable(reach.text, "Reach", implement); });
        }

        private void SetUnitPanelProperties(Unit implement)
        {
            tileGUI.SetActive(false);

            TacticsMove tacticsMove = (TacticsMove)implement.move;
            JRPGBattle jRPGBattle = (JRPGBattle)implement.battle;

            unitName.text = implement.name;

            if (implement is PlayerUnit)
            {
                player.isOn = true;
            }
            else
            {
                player.isOn = false;
            }
            Vector2Int unitPos = MapReader.WorldToGridSpace(implement.transform.position);
            if (unitPos != null)
            {
                unitPosX.text = unitPos.x.ToString();
                unitPosY.text = unitPos.y.ToString();
            }
            else
            {
                unitPosX.text = "";
                unitPosY.text = "";
            }

            moveDistance.text = tacticsMove.moveDistance.ToString();
            moveSpeed.text = tacticsMove.moveSpeed.ToString();
            jumpHeight.text = tacticsMove.jumpHeight.ToString();

            maxHealth.text = jRPGBattle.maxHealth.ToString();
            health.maxValue = jRPGBattle.maxHealth;
            health.value = jRPGBattle.health;

            maxWill.text = jRPGBattle.maxWill.ToString();
            will.maxValue = jRPGBattle.maxWill;
            will.value = jRPGBattle.will;

            reach.text = jRPGBattle.reach.ToString();

            unitGUI.SetActive(true);
        }

        private void UnSubscribeUnit()
        {
            player.onValueChanged.RemoveAllListeners();
            unitPosX.onEndEdit.RemoveAllListeners();
            unitPosY.onEndEdit.RemoveAllListeners();
            moveDistance.onEndEdit.RemoveAllListeners();
            moveSpeed.onEndEdit.RemoveAllListeners();
            jumpHeight.onEndEdit.RemoveAllListeners();
            maxHealth.onEndEdit.RemoveAllListeners();
            health.onValueChanged.RemoveAllListeners();
            maxWill.onEndEdit.RemoveAllListeners();
            will.onValueChanged.RemoveAllListeners();
            reach.onEndEdit.RemoveAllListeners();
        }

        public void LoadSprite(Tile[] selectedTile, SpriteRenderer[] spriteRenderer)
        {
            int initLength = AddSpriteToEditorPallate();
            for (int i = 0; i < selectedTile.Length; i++)
            {
                ChangeTileTexture(initLength, selectedTile[i], spriteRenderer[i]);
            }
        }

        public void LoadSprite(Tile selectedTile, SpriteRenderer spriteRenderer)
        {
            int initLength = AddSpriteToEditorPallate();
            ChangeTileTexture(initLength, selectedTile, spriteRenderer);
        }

        private int AddSpriteToEditorPallate()
        {
            Sprite[] addable = SaveSystem.LoadPNG(Vector2.one / 2f);
            int initLength = MapEditor.pallate.Length;
            Sprite[] pallate = new Sprite[initLength + addable.Length];
            MapEditor.pallate.CopyTo(pallate, 0);
            addable.CopyTo(pallate, initLength);
            MapEditor.pallate = pallate;
            return initLength;
        }

        public void AddUnit() => MapEditor.AddUnit();

        public void NewMap() => MapReader.GeneratePhysicalMap(new Map(1, 1));
    }

}