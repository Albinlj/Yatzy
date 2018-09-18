public class Player {
    public string Name { get; }
    public int Id { get; }

    public Category[] Scoreboard { get; } = {
        new Category("Ones"),
        new Category("Twos"),
        new Category("Threes"),
        new Category("Fours"),
        new Category("Fives"),
        new Category("Sixes"),
        new Category("OnePair"),
        new Category("TwoPair"),
        new Category("Three of a kind"),
        new Category("Four o a kind"),
        new Category("Small straight"),
        new Category("Large straight"),
        new Category("Full house"),
        new Category("Chance"),
        new Category("Yatzy")
    };

    public Player(string name) {
        Name = name;
    }

    public bool HasFullScoreboard() {
        foreach (Category score in Scoreboard) {
            if (!score.IsScored) return false;
        }
        return true;
    }

    public int GetTotalScore() {
        int total = 0;
        foreach (Category score in Scoreboard) {
            total += score.Val;
        }
        return total;
    }

}