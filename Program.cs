using System;
using System.Text.RegularExpressions;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

namespace Yatzy {
    class Program {

        static string[] letters = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o" };

        static void Main(string[] args) {
            Player[] players = SetupPlayers();
            int playerTurn = 0;
            bool gameOver = false;

            int[] lockedDice = new int[5];

            while (gameOver == false) {
                Console.Clear();

                //Starts a new player turn and goes through the rolling process
                Player activePlayer = players[playerTurn];
                lockedDice = lockAndRoll(activePlayer);

                Array.Sort(lockedDice);
                Console.WriteLine("Your final dice are:");
                Console.WriteLine("");
                foreach (int val in lockedDice)
                    Console.WriteLine("X : " + val);

                Console.WriteLine("");
                PrintScoreboard(activePlayer);

                //Choosing which category to score
                int categoryScore = 0;
                Regex rgx = new Regex(@"[a-o]");
                bool isMatch = false;
                bool isValid = true;
                string pressedKey;
                int indexOfKey;
                do {
                    Console.WriteLine("Which do you want to score (A-O)?");
                    pressedKey = Console.ReadKey().Key.ToString().ToLower();
                    indexOfKey = Array.IndexOf(letters, pressedKey);
                    isMatch = rgx.IsMatch(pressedKey) && pressedKey.Length == 1;
                    if (!isMatch) {
                        Console.WriteLine(" is not a valid choice.");
                        isValid = false;
                    }
                    if (activePlayer.scoreboard[indexOfKey].isScored) {
                        Console.WriteLine(" is already scored.");
                        isValid = false;
                    }
                } while (!isValid);

                //Calculate how many times every value occurs
                Console.WriteLine("");
                int[] occurences = new int[6];
                foreach (int diceVal in lockedDice)
                    occurences[diceVal - 1]++;

                //Calculate score of chosen category
                switch (pressedKey) {
                    case "a":
                    case "b":
                    case "c":
                    case "d":
                    case "e":
                    case "f": // 1-6
                        categoryScore = lockedDice.Aggregate(0, (a, b) => b == indexOfKey + 1 ? a + b : a);
                        break;
                    case "g": //One Pair
                        for (int i = lockedDice.Length - 1; i > 0; i--) {
                            if (lockedDice[i - 1] == lockedDice[i]) {
                                categoryScore = lockedDice[i] * 2;
                                break;
                            }
                        }
                        break;
                    case "h": //Two Pair
                        int lastPair = 0;
                        for (int i = lockedDice.Length - 1; i > 0; i--) {
                            if (lockedDice[i - 1] == lockedDice[i]) {
                                if (lastPair == 0)
                                    lastPair = lockedDice[i];
                                else if (lastPair != lockedDice[i]) {
                                    categoryScore = lockedDice[i] * 2 + lastPair * 2;
                                    break;
                                }
                            }
                        }
                        break;
                    case "i": //Three of a kind
                    case "j": //Four of a kind
                        int countNeed = Array.IndexOf(letters, pressedKey) - 5;

                        for (int i = occurences.Length - 1; i >= 0; i--) {
                            if (occurences[i] >= countNeed) {
                                categoryScore = (i + 1) * countNeed;
                                break;
                            }

                        }
                        break;
                    case "k": //Small Straight
                    case "l": //Large Straight
                        bool isLarge = Array.IndexOf(letters, pressedKey) == 11;
                        categoryScore = 15 + Convert.ToInt32(isLarge) * 5;
                        for (int i = 0; i < lockedDice.Length; i++) {
                            if (lockedDice[i] != i + 1 + Convert.ToInt32(isLarge)) {
                                categoryScore = 0;
                                break;

                            }
                        }
                        break;
                    case "m": //Full House

                        Array.Sort(occurences);
                        if (occurences[4] == 2 && occurences[5] == 3) {
                            categoryScore = lockedDice[4] * 3 + lockedDice[0] * 2;
                        }
                        break;
                    case "n": //Chance
                        categoryScore = lockedDice.Aggregate((a, b) => a + b);
                        break;
                    case "o": //Yatzy
                        categoryScore = 50;
                        for (int i = 1; i < lockedDice.Length; i++) {
                            if (lockedDice[i] != lockedDice[0]) {
                                categoryScore = 0;
                                break;
                            }
                        }
                        break;
                    default:
                        break;
                }
                Score theScore = activePlayer.scoreboard[indexOfKey];

                //Set score variables
                theScore.val = categoryScore;
                theScore.isScored = true;
                Console.WriteLine(activePlayer.name + "s score for category " + theScore.name + " is  " + theScore.val);
                Console.ReadKey();

                //playerTurn = (playerTurn == players.Length - 1) ? playerTurn + 1 : playerTurn = 0; //Change playerturn
                if (playerTurn == 0 && activePlayer.isDone()) {
                    gameOver = true;
                }

            }



        }

        private static int[] lockAndRoll(Player activePlayer) {
            int[] lockedDice = new int[5];
            int lockedCount = 0;
            int rerolls = 2;
            while (lockedCount < 5) {

                //Rolls the the remaining dice
                int[] liveDice = RollDice(5 - lockedCount);

                //If there are no rerolls left - exit the while loop.
                if (rerolls == 0) {
                    foreach (int num in liveDice) {
                        lockedDice[lockedCount] = num;
                        lockedCount++;
                    }
                    break;
                }

                //Write out the locked and newly rolled dice
                Console.WriteLine(activePlayer.name + " has " + rerolls + " rerolls remaining");
                Console.WriteLine("");
                if (lockedCount > 0) {
                    for (int i = 0; i < lockedCount; i++)
                        Console.WriteLine("X : " + lockedDice[i]);
                    Console.WriteLine("");
                }
                for (int i = 0; i < liveDice.Length; i++)
                    Console.WriteLine(letters[i] + " : " + liveDice[i]);

                //Choose which dice to lock
                Console.WriteLine("");
                Console.WriteLine("Which dice do you want to lock? Write the corresponding letters");
                string lockInput = Console.ReadLine();
                for (int i = 0; i < liveDice.Length; i++) {
                    if (lockInput.Contains(letters[i])) {
                        lockedDice[lockedCount] = liveDice[i];
                        lockedCount++;
                    }
                }

                Console.Clear();
                //Console.WriteLine("");
                rerolls--;
            }
            return lockedDice;
        }

        static void PrintScoreboard(Player player) {
            Console.WriteLine(player.name + "s current scores are:");
            for (int i = 0; i < player.scoreboard.Length; i++) {
                Score score = player.scoreboard[i];
                string line = letters[i] + " [";
                if (!score.isScored) line += ("  ");
                else {
                    if (score.val < 10)
                        line += " ";
                    line += score.val.ToString();
                }
                line += "] " + score.name;
                Console.WriteLine(line);
                if (i == 5) Console.WriteLine("");
            }
        }

        static int[] RollDice(int maxVal) {
            Random rnd = new Random();
            int[] values = new int[maxVal];
            for (int i = 0; i < maxVal; i++) {
                values[i] = rnd.Next(1, 7);
            }
            return values;
        }


        static Player[] SetupPlayers() {
            //Real
            //Console.WriteLine("How many players?");
            //int playerCount = int.Parse(Console.ReadLine());
            //Player[] players = new Player[playerCount];
            //Console.WriteLine(playerCount);
            //Console.ReadKey();
            //for (int i = 0; i < playerCount; i++) {
            //    Console.WriteLine("What is the name of player " + (i + 1) + "?");
            //    string name = Console.ReadLine();
            //    players[i] = new Player(name);
            //}
            //return players;

            //Debugtests
            return new Player[] { new Player("Struten"), new Player("Braxen") };
        }

    }
}