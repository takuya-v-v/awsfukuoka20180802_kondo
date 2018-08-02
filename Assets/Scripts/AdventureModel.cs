using System;

[Serializable]
public class AdventureModel {
    public string address = "";
    public int score = 0;
    public string name = "";
    public int charaId = 0;

    public AdventureModel() {
        
    }

    public AdventureModel(ETHManager.Adventure adventure) {
        address = adventure.Address;
        score = adventure.Score;
        name = adventure.Name;
        charaId = adventure.CharaId;
    }
}
