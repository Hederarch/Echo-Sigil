using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Windows.Forms;

namespace mapEditor
{
    public static class SaveSystem
    {

        public static bool developerMode = true;
        public static string curModPath = null;

        public static void SaveMap(string path, Map map)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, map);
            stream.Close();
        }

        public static Map LoadMap(string path, bool logError = false)
        {
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                Map map = formatter.Deserialize(stream) as Map;
                stream.Close();
                map.path = path;
                return map;
            }
            else if (logError)
            {
                Debug.LogError("Map File not found in " + path);
            }
            return null;
        }

        public static void DeleteMap(string path, bool logWarning = false)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (logWarning)
            {
                Debug.LogWarning("Map File not found in " + path + ". So... um, I guess its been sucsessfuly gotten rid of then.");
            }
        }

        public static Sprite[] LoadPallate(string questPath)
        {
            List<Sprite> spritePallate = new List<Sprite>();
            if (!Directory.Exists(questPath + "/Pallate"))
            {
                Debug.LogError("Pallate folder does not exist in " + questPath + " directory. Are you sure this is a quest folder?");
                return null;
            }
            for (int i = 0; File.Exists(questPath + "/Pallate/" + i + ".png"); i++)
            {
                spritePallate.Add(LoadPNG(questPath + "/Pallate/" + i + ".png", Vector2.one / 2f));
            }
            return spritePallate.ToArray();
        }

        internal static void SavePallate(string fullName, Sprite[] pallate)
        {
            if (!Directory.Exists(fullName + "/Pallate"))
            {
                Directory.CreateDirectory(fullName + "/Pallate");
            }
            for (int i = 0; i < pallate.Length; i++)
            {
                if (!File.Exists(fullName + "/Pallate/" + i + ".png"))
                {
                    SavePNG(fullName + "/Pallate/" + i + ".png", pallate[i].texture);
                }
            }
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
            using(OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image Files(*.PNG; *.JPG;)| *.PNG; *.JPG; | All files(*.*) | *.*";
                openFileDialog.FilterIndex = 0;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Sprite[] sprites = new Sprite[openFileDialog.FileNames.Length];

                    for(int i = 0; i< openFileDialog.FileNames.Length; i++)
                    {
                        sprites[i] = LoadPNG(openFileDialog.FileNames[i], pivot, numTileWidth);
                    }

                    return sprites;
                }

                return new Sprite[0];
            }
        }

        public static void SavePNG(string filePath, Texture2D texture)
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

        public static ImplementList SaveImplmentList(ImplementList implements)
        {
            using (StreamWriter stream = new StreamWriter(implements.modPath + "/" + Path.GetFileName(implements.modPath) + ".json"))
            {
                stream.Write(JsonUtility.ToJson(implements,true));
            }
            return implements;
        }
        internal static ImplementList LoadImplementList(string modPath = null)
        {
            modPath = SetDefualtModPath(modPath);

            using (StreamReader stream = new StreamReader(modPath + "/" + Path.GetFileName(modPath) + ".json"))
            {
                string json = stream.ReadToEnd();
                return JsonUtility.FromJson<ImplementList>(json);
            }
        }

        public static string SetDefualtModPath(string modPath)
        {
            if (developerMode && modPath == null)
            {
                return UnityEngine.Application.dataPath + "/Implements";
            }
            if (modPath == null)
            {
                Debug.LogError("No modpath for new implement. Putting changes in temp file");
                return UnityEngine.Application.temporaryCachePath + "/Implements";
            }
            return modPath;
        }
    }
}