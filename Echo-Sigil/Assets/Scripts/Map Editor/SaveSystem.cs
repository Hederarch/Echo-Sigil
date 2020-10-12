using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Windows.Forms;
using MapEditor.Animations;
using System;

namespace MapEditor
{
    public static class SaveSystem
    {

        public static bool developerMode = true;
        public const string mapFileName = "Quests";
        public const string implementFileName = "Implements";
        public const string pallateFileName = "Pallate";
        public const string baseSpriteFileName = "Base";
        public const string defaultQuest = "Test";

        static ModPath[] modPaths;

        public static ModPath[] GetModPaths(bool reloadModPaths = false)
        {
            if (modPaths == null || reloadModPaths)
            {
                ModPath[] newModPaths = new ModPath[1];
                newModPaths[0] = GetDefualtModPath();
                modPaths = newModPaths;
            }
            return modPaths;
        }

        private static ModPath GetDefualtModPath()
        {
            return UnityEngine.Application.dataPath + "/Offical Story Pack";

        }

        public static void SaveMap(Map map)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(map.modPath + "/" + mapFileName + "/" + map.quest + "/" + map.name, FileMode.Create);
            formatter.Serialize(stream, map);
            stream.Close();
        }

        public static string GetMapPath(int modPathIndex, string quest, string name) => GetQuestPath(modPathIndex, quest) + "/" + name;

        public static Map LoadMap(int modPathindex, string quest, string name, bool logError = false)
        {
            string path = GetMapPath(modPathindex, quest, name);
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                Map map = formatter.Deserialize(stream) as Map;
                stream.Close();
                map.modPathIndex = modPathindex;
                return map;
            }
            else if (logError)
            {
                Debug.LogError(name + " not found at " + path);
            }
            return null;
        }

        public static void DeleteMap(int modPathindex, string quest, string name, bool logError = false)
        {
            string path = GetMapPath(modPathindex, quest, name);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (logError)
            {
                Debug.LogWarning(name + " not found in " + path + ". So... I guess its been sucsessfuly gotten rid of then.");
            }
        }

        public static void SavePallate(int modPathIndex, string quest, Sprite[] pallate)
        {
            string path = GetPallatePath(modPathIndex, quest);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            for (int i = 0; i < pallate.Length; i++)
            {
                string imagePath = path + i + ".png";
                if (!File.Exists(imagePath))
                {
                    SavePNG(imagePath, pallate[i].texture);
                }
            }
        }

        public static string GetQuestPath(int modPathIndex, string quest) => GetModPaths()[modPathIndex] + "/" + mapFileName + "/" + quest;

        public static string GetPallatePath(int modPathIndex, string quest) => GetQuestPath(modPathIndex, quest) + "/" + pallateFileName;

        public static Sprite[] LoadPallate(int modPathIndex, string quest = defaultQuest)
        {
            string path = GetPallatePath(modPathIndex, quest);
            List<Sprite> spritePallate = new List<Sprite>();
            if (!Directory.Exists(path))
            {
                Debug.LogError("Pallate folder does not exist in " + quest + " directory. Are you sure this is a quest folder?");
                return null;
            }
            for (int i = 0; File.Exists(path + i + ".png"); i++)
            {
                spritePallate.Add(LoadPNG(path + i + ".png", Vector2.one / 2f));
            }
            return spritePallate.ToArray();
        }

        public static Sprite LoadPNG(string filePath, Vector2 pivot, int numTileWidth = 1)
        {
            if (File.Exists(filePath))
            {
                byte[] fileData;
                fileData = File.ReadAllBytes(filePath);
                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, true);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.

                if (tex.width != tex.height)
                {
                    Debug.LogWarning("Texture " + filePath + " is not square. Texture hieght may be adversly affected");
                }

                Sprite sprite = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(tex.width, tex.height)), pivot, tex.width / numTileWidth);
                return sprite;
            }
            return null;
        }

        public static Sprite[] LoadPNG(Vector2 pivot, int numTileWidth = 1)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image Files(*.PNG; *.JPG;)| *.PNG; *.JPG; | All files(*.*) | *.*";
                openFileDialog.FilterIndex = 0;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Sprite[] sprites = new Sprite[openFileDialog.FileNames.Length];

                    for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                    {
                        sprites[i] = LoadPNG(openFileDialog.FileNames[i], pivot, numTileWidth);
                    }

                    return sprites;
                }

                return new Sprite[0];
            }
        }

        private static void SavePNG(string filePath, Texture2D texture)
        {
            string fullName = Directory.GetParent(filePath).FullName;
            if (!Directory.Exists(fullName))
            {
                Directory.CreateDirectory(fullName);
            }
            if (!texture.isReadable)
            {
                Debug.LogError(texture + " is unreadable");
                return;
            }
            byte[] fileData;
            fileData = texture.EncodeToPNG();
            File.WriteAllBytes(filePath, fileData);
        }

        public static string GetImplementPath(int modPathIndex) => GetModPaths()[modPathIndex] + "/" + implementFileName;

        public static void SaveImplments(int modPathIndex, Implement[] implements)
        {
            for (int i = 0; i < implements.Length; i++)
            {
                SaveImplement(modPathIndex, implements[i]);
            }
        }

        public static void SaveImplement(int modPathIndex, Implement implement)
        {
            string name = implement.splashInfo.name;
            string implementPath = GetImplementPath(modPathIndex) + "/" + name;
            if (!Directory.Exists(implementPath))
            {
                Directory.CreateDirectory(implementPath);
            }
            else
            {
                if (implement != null && implement.NullCheck())
                {
                    Directory.Delete(implementPath, true);
                    Directory.CreateDirectory(implementPath);
                }
                else
                {
                    Debug.LogError("Tried to save, but found null value");
                    return;
                }
            }
            using (StreamWriter stream = new StreamWriter(implementPath + "/" + name + ".json"))
            {
                stream.Write(JsonUtility.ToJson(implement, true));
            }
            if (implement.baseSprite != null)
            {
                SavePNG(implementPath + "/" + baseSpriteFileName + ".png", implement.baseSprite.texture);
            }

            SaveAnimations(modPathIndex, name, implement.animations);
        }

        public static Implement[] LoadImplements(int modPathIndex)
        {
            string path = GetImplementPath(modPathIndex);

            List<Implement> implementList = new List<Implement>();
            if (Directory.Exists(path))
            {
                string[] directories = Directory.GetDirectories(path);
                foreach (string directory in directories)
                {
                    implementList.Add(LoadImplement(modPathIndex, Path.GetFileName(directory)));
                }
            }
            return implementList.ToArray();
        }

        public static Implement LoadImplement(int modPathIndex, string name)
        {
            string path = GetImplementPath(modPathIndex) + "/" + name;

            Implement implement;
            using (StreamReader stream = new StreamReader(path + "/" + name + ".json"))
            {
                string json = stream.ReadToEnd();
                implement = JsonUtility.FromJson<Implement>(json);
            }
            implement.modPathIndex = modPathIndex;
            implement.baseSprite = LoadPNG(path + "/" + baseSpriteFileName + ".png", Vector2.one / 2f);
            implement.animations = LoadAnimations(modPathIndex, name);
            return implement;

        }

        private static IAnimation[] LoadAnimations(int modPathIndex, string ImplementName)
        {
            List<IAnimation> animations = new List<IAnimation>();
            string[] directories = Directory.GetDirectories(GetImplementPath(modPathIndex) + "/" + ImplementName);
            foreach (string directory in directories)
            {
                animations.Add(LoadAnimation(modPathIndex, ImplementName, Path.GetFileName(directory)));
            }
            return animations.ToArray();
        }

        private static IAnimation LoadAnimation(int modPathIndex, string implementName, string animationName)
        {
            string path = GetImplementPath(modPathIndex) + "/" + implementName + "/" + animationName;
            string json;
            using (StreamReader stream = new StreamReader(path + "/" + Path.GetFileName(path) + ".json"))
            {
                json = stream.ReadToEnd();
            }
            int endOfJson = json.LastIndexOf('}') + 1;
            string jsonEnd = json.Substring(endOfJson);
            json = json.Substring(0, endOfJson);

            switch (jsonEnd)
            {
                case "MapEditor.Animations.VaraintAnimation":
                    VaraintAnimation varaintAnimation = JsonUtility.FromJson<VaraintAnimation>(json);
                    varaintAnimation.animations = LoadAnimations(modPathIndex, implementName + "/" + animationName);
                    return varaintAnimation;
                case "MapEditor.Animations.DirectionalAnimation":
                    DirectionalAnimation directionalAnimation = JsonUtility.FromJson<DirectionalAnimation>(json);
                    directionalAnimation.animations = LoadAnimations(modPathIndex, implementName + "/" + animationName);
                    directionalAnimation.animationIndexes = new DirectionalAnimation.AnimationIndexes();
                    return directionalAnimation;
                case "MapEditor.Animations.MultiTileAnimation":
                    List<Sprite> MultiTilesprites = new List<Sprite>();
                    string[] MultiTilefiles = GetOrderedSpriteFiles(path);
                    MultiTileAnimation multiTileAnimation = JsonUtility.FromJson<MultiTileAnimation>(json);
                    foreach (string file in MultiTilefiles)
                    {
                        MultiTilesprites.Add(LoadPNG(file, Vector2.one / 2f, multiTileAnimation.tileWidth));
                    }
                    multiTileAnimation.sprites = MultiTilesprites.ToArray();
                    return multiTileAnimation;
                case "MapEditor.Animations.Animation":
                    List<Sprite> sprites = new List<Sprite>();
                    string[] files = GetOrderedSpriteFiles(path);
                    foreach (string file in files)
                    {
                        sprites.Add(LoadPNG(file, Vector2.one / 2f));
                    }
                    Animations.Animation animation1 = JsonUtility.FromJson<Animations.Animation>(json);
                    animation1.sprites = sprites.ToArray();
                    return animation1;
                default:
                    Debug.LogError("Tried to load type " + jsonEnd);
                    return null;
            }
        }

        private static string[] GetOrderedSpriteFiles(string path)
        {
            string[] files = Directory.GetFiles(path, "*.png");

            //Order files
            List<string> fileList = new List<string>();
            foreach (string file in files)
            {
                fileList.Add(file);
            }
            fileList.Sort();
            return fileList.ToArray();
        }

        private static void SaveAnimations(int modPathIndex, string ImplementName, IAnimation[] animations, string parentName = null)
        {
            foreach (IAnimation animation in animations)
            {
                SaveAnimation(modPathIndex, ImplementName, animation, parentName);
            }
        }

        private static void SaveAnimation(int modPathIndex, string implementName, IAnimation animation, string parentName = null)
        {
            if (animation != null)
            {
                string implementPath = GetImplementPath(modPathIndex) + "/" + implementName;
                //format parernt name
                parentName = parentName == null ? "" : parentName + "/";
                string path = GetUniqueName(implementPath + "/" + parentName + animation.Name);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                using (StreamWriter stream = new StreamWriter(path + "/" + Path.GetFileName(path) + ".json"))
                {
                    stream.Write(JsonUtility.ToJson(animation, true) + animation.Type.ToString());
                }

                if (animation.Type == typeof(VaraintAnimation) || animation.Type == typeof(DirectionalAnimation))
                {
                    if (animation.Type == typeof(VaraintAnimation))
                    {
                        VaraintAnimation varaintAnimation = (VaraintAnimation)animation;
                        SaveAnimations(modPathIndex, implementName, varaintAnimation.animations, parentName + animation.Name);

                    }
                    else if (animation.Type == typeof(DirectionalAnimation))
                    {
                        DirectionalAnimation directionalAnimation = (DirectionalAnimation)animation;
                        SaveAnimations(modPathIndex, implementName, directionalAnimation.animations, parentName + animation.Name);

                    }
                }
                else if (animation.Type == typeof(Animations.Animation) || animation.Type == typeof(MultiTileAnimation))
                {
                    Sprite[] sprites = new Sprite[0];
                    if (animation.Type == typeof(Animations.Animation))
                    {
                        Animations.Animation animation1 = (Animations.Animation)animation;
                        sprites = animation1.sprites;
                    }
                    else if (animation.Type == typeof(MultiTileAnimation))
                    {
                        MultiTileAnimation multiTileAnimation = (MultiTileAnimation)animation;
                        sprites = multiTileAnimation.sprites;

                    }

                    for (int i = 0; i < sprites.Length; i++)
                    {
                        SavePNG(path + "/" + animation.Name + "_" + i.ToString() + ".png", sprites[i].texture);
                    }

                }

            }
            else
            {
                Debug.LogError("You just tried to save a Null animation, wow!");
            }
        }

        private static string GetUniqueName(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            bool nameTaken = false;
            foreach (string otherName in Directory.EnumerateFiles(Path.GetDirectoryName(path)))
            {
                nameTaken &= name != otherName;
            }
            if (nameTaken)
            {
                name += "0";
                return GetUniqueName(Path.GetDirectoryName(path) + "/" + name + Path.GetExtension(path));
            }
            return path;
        }
    }
}

public struct ModPath
{
    public string modPath;
    public string modName;

    public ModPath(string _modPath)
    {
        modPath = _modPath;
        modName = Path.GetFileName(_modPath);
    }

    public static implicit operator ModPath(string s) => new ModPath(s);
    public static implicit operator string(ModPath m) => m.modPath;

}
