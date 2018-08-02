pragma solidity ^0.4.23;


contract LuidasBar {
    address owner;

    constructor() public {
        owner = msg.sender;
    }

    uint maxTopScores = 100;

    struct Adventure{
        address addr;
        int score;
        string name;
        int charaId;
    }

    Adventure[] public topAdventures;
    mapping (address=>Adventure) public userAdventures;

    function setAdventure(address addr, int score, string name, int charaId) public returns(uint) {
        Adventure storage currentTopScore = userAdventures[addr];
        Adventure memory topScore = Adventure(addr, score, name, charaId);
        if(currentTopScore.score < score){
            userAdventures[addr] = topScore;
        }

        if(topAdventures.length < maxTopScores){
            topAdventures.push(topScore);
        }else{
            int lowestScore = 0;
            uint lowestScoreIndex = 0;
            for (uint i = 0; i < topAdventures.length; i++)
            {
                Adventure storage currentScore = topAdventures[i];
                if(i == 0){
                    lowestScore = currentScore.score;
                    lowestScoreIndex = i;
                }else{
                    if(lowestScore > currentScore.score){
                        lowestScore = currentScore.score;
                        lowestScoreIndex = i;
                    }
                }
            }
            if(score > lowestScore){
                topAdventures[lowestScoreIndex] = topScore;
            }
        }
        return getCountTopAdventures();
    }

    function getCountTopAdventures() public view returns(uint) {
        return topAdventures.length;
    }
}
