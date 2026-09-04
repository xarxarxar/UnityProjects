using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace MergeBeast {
    public static class SaveSystem {
        public static readonly string SAVE_FOLDER = Application.persistentDataPath + "/_SAVE/";
        public static readonly string DATA_PATH = SAVE_FOLDER + "data.json";
        public static readonly string TILE_CONTINUE = SAVE_FOLDER + "TileContinue/";
        public static readonly string TILE_ORIGIN = SAVE_FOLDER + "TileOrigin/";

        public static void Init() {
            if (!Directory.Exists(SAVE_FOLDER)) {
                Directory.CreateDirectory(SAVE_FOLDER);
            }
            if(!Directory.Exists(TILE_CONTINUE)) {
                Directory.CreateDirectory(TILE_CONTINUE);
            }
            if(!Directory.Exists(TILE_ORIGIN)) {
                Directory.CreateDirectory(TILE_ORIGIN);
            }

         
         
        }


        public static void Save(string saveString) {           
            File.WriteAllText(DATA_PATH, saveString);
        }

        public static void Save(string path,string data) {
            File.WriteAllText(path, data);
        }

        public static string Load() {
            if(File.Exists(DATA_PATH)) {
                return File.ReadAllText(DATA_PATH);
            }
            return null;
        }

        public static string Load(string path) {
            if(File.Exists(path)) {
                return File.ReadAllText(path);
            }
            return null;
        }
    }
}