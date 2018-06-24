using System;
using System.Linq;


public class Player {
    public string name;
    public int id;
    public Score[] scoreboard = new Score[] {
        new Score ("Ones"),
        new Score ("Twos"),
        new Score ("Threes"),
        new Score ("Fours"),
        new Score ("Fives"),
        new Score ("Sixes"),
        new Score ("OnePair"),
        new Score ("TwoPair"),
        new Score ("Three of a kind"),
        new Score ("Four o a kind"),
        new Score ("Small straight"),
        new Score ("Large straight"),
        new Score ("Full house"),
        new Score ("Chance"),
        new Score ("Yatzy")
    };

    public Player(string _name) {
        name = _name;
    }

    public bool isDone() {
        foreach (Score score in scoreboard) {
            if (!score.isScored) return false;
        }
        return true;
    }

    public int getTotalScore() {
        int total = 0;
        foreach (Score score in scoreboard) {
            total += score.val;
        }
        return total;
    }

}

public class Score {
    public string name;
    public int val;
    public bool isScored;

    public Score(string _name) {
        name = _name;
        val = 0;
        isScored = false;
    }
}
