using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace MergeBeast {
    public class DataConfig  {
        

        public static JSONArray ListSoulReward;
        public static JSONArray ListGemReward;
        public static JSONArray ListMedalReward;
        public static JSONArray ListChestReward;

        static DataConfig() {
            ListSoulReward = JSON.Parse("[]").AsArray;
            ListGemReward = JSON.Parse("[]").AsArray;
            ListMedalReward = JSON.Parse("[]").AsArray;
            ListChestReward = JSON.Parse("[]").AsArray;
        }

        public static JSONNode ToJson() {
            var data = new JSONObject();
            data["listSoulReward"] = ListSoulReward;
            data["listGemReward"] = ListGemReward;
            data["listMedalReward"] = ListMedalReward;
            data["listChestReward"] = ListChestReward;
            return data;
        }

        public static string FromJsonToString() {
            JSONNode data = ToJson();
            //Debug.LogError(data.ToString());
            return data.ToString();
        }

        public static void ParseJson(JSONNode data) {
            ListSoulReward = data.HasKey("listSoulReward") ? data["listSoulReward"].AsArray : ListSoulReward;
            ListGemReward = data.HasKey("listGemReward") ? data["listGemReward"].AsArray : ListGemReward;
            ListMedalReward = data.HasKey("listMedalReward") ? data["listMedalReward"].AsArray : ListMedalReward;
            ListChestReward = data.HasKey("listChestReward") ? data["listChestReward"].AsArray : ListChestReward;
        }

        public static void SaveData() {
            SaveSystem.Save(FromJsonToString());
        }


    } //end class
}