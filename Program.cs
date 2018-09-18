using System;
using System.Text.RegularExpressions;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Yatzy {
    class Program {

        static readonly string[] letters = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o" };
        public static Player[] players = SetupPlayers();
        static int playerTurn = 0;
        static Player activePlayer;


        public static void Main(string[] args) {
            bool gameOver = false;

            while (gameOver == false) {
                Console.Clear();

                //Starts a new player turn and goes through the rolling process
                activePlayer = players[playerTurn];
                Console.WriteLine($"It is {activePlayer.Name}s turn.");

                int[] lockedDice = LockAndRoll(activePlayer);

                PrintScoreboard(activePlayer);

                //Choosing which category to score
                int categoryScore = 0;
                Regex regexAtoO = new Regex(@"[a-o]");
                bool isValid;
                string pressedKey;
                int indexOfKey;
                do {
                    isValid = true;
                    Console.WriteLine("Which do you want to category (A-O)?");
                    pressedKey = Console.ReadKey().Key.ToString().ToLower();
                    indexOfKey = Array.IndexOf(letters, pressedKey);

                    bool isMatch = regexAtoO.IsMatch(pressedKey) && pressedKey.Length == 1;
                    if (!isMatch) {
                        Console.WriteLine(" is not a valid choice.");
                        isValid = false;
                    }
                    else if (activePlayer.Scoreboard[indexOfKey].IsScored) {
                        Console.WriteLine(" is already scored.");
                        isValid = false;
                    }
                } while (!isValid);

                //Calculate how many times every value occurs
                Console.WriteLine("");
                int[] occurrences = new int[6];
                foreach (int diceVal in lockedDice)
                    occurrences[diceVal - 1]++;

                //Calculate category of chosen category
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
                            if (lockedDice[i - 1] != lockedDice[i]) continue;
                            categoryScore = lockedDice[i] * 2;
                            break;
                        }
                        break;
                    case "h": //Two Pair
                        int lastPair = 0;
                        for (int i = lockedDice.Length - 1; i > 0; i--) {
                            if (lockedDice[i - 1] != lockedDice[i]) continue;
                            if (lastPair == 0)
                                lastPair = lockedDice[i];
                            else if (lastPair != lockedDice[i]) {
                                categoryScore = lockedDice[i] * 2 + lastPair * 2;
                                break;
                            }
                        }
                        break;
                    case "i": //Three of a kind
                    case "j": //Four of a kind
                        int countNeed = Array.IndexOf(letters, pressedKey) - 5;

                        for (int i = occurrences.Length - 1; i >= 0; i--) {
                            if (occurrences[i] < countNeed) continue;
                            categoryScore = (i + 1) * countNeed;
                            break;

                        }
                        break;
                    case "k": //Small Straight
                    case "l": //Large Straight
                        bool isLarge = Array.IndexOf(letters, pressedKey) == 11;
                        categoryScore = 15 + Convert.ToInt32(isLarge) * 5;
                        for (int i = 0; i < lockedDice.Length; i++) {
                            if (lockedDice[i] == i + 1 + Convert.ToInt32(isLarge)) continue;
                            categoryScore = 0;
                            break;
                        }
                        break;
                    case "m": //Full House

                        Array.Sort(occurrences);
                        if (occurrences[4] == 2 && occurrences[5] == 3) {
                            categoryScore = lockedDice[4] * 3 + lockedDice[0] * 2;
                        }
                        break;
                    case "n": //Chance
                        categoryScore = lockedDice.Aggregate((a, b) => a + b);
                        break;
                    case "o": //Yatzy
                        categoryScore = 50;
                        for (int i = 1; i < lockedDice.Length; i++) {
                            if (lockedDice[i] == lockedDice[0]) continue;
                            categoryScore = 0;
                            break;
                        }
                        break;
                    default:
                        break;
                }
                Category categoryToCategory = activePlayer.Scoreboard[indexOfKey];

                //Set category variables
                categoryToCategory.Val = categoryScore;
                categoryToCategory.IsScored = true;

                Console.Clear();
                Console.WriteLine(activePlayer.Name + "s current scores are:");
                PrintScoreboard(activePlayer);
                Console.ReadKey();



                playerTurn = (playerTurn == players.Length - 1) ? 0 : playerTurn + 1; //Change player turn
                if (playerTurn == 0 && activePlayer.HasFullScoreboard()) {
                    gameOver = true;
                }

            }


            GameOver();
            Console.ReadKey();
        }

        private static void GameOver() {
            //Handle Game over
            foreach (Player player in players) {
                Console.WriteLine(player.Name + "s scores:");
                PrintScoreboard(player);
                Console.WriteLine("");
            }

            Console.WriteLine("The final scores are:");

            Player[] winners = new Player[players.Length];
            int winnerCount = 0;
            winners[0] = players[0];
            int maxScore = 0;
            foreach (Player player in players) {
                int playerScore = player.GetTotalScore();
                if (playerScore > winners[0].GetTotalScore()) {
                    winners[0] = player;
                    winnerCount = 1;
                    maxScore = playerScore;
                }
                else if (playerScore == winners[0].GetTotalScore()) {
                    winners[winnerCount] = player;
                    winnerCount++;
                }

                Console.WriteLine(player.Name + ": " + player.GetTotalScore());
            }

            if (winnerCount == 1) {
                Console.WriteLine("The winner is: " + winners[0].Name);
            }
            else {
                Console.WriteLine("It's a tie between: ");
                for (int i = 0; i < winnerCount; i++) {
                    Console.WriteLine(winners[i].Name);
                }
            }

            Console.WriteLine("Congratulations!");
        }

        private static int[] LockAndRoll(Player activePlayer) {
            int[] lockedDice = new int[5];
            int lockedCount = 0;
            int reRolls = 2;
            while (lockedCount < 5) {

                //Rolls the the remaining dice
                int[] liveDice = RollDice(5 - lockedCount);

                //If there are no rerolls left - exit the while loop.
                if (reRolls == 0) {
                    foreach (int num in liveDice) {
                        lockedDice[lockedCount] = num;
                        lockedCount++;
                    }
                    break;
                }

                //Write out the locked and newly rolled dice
                Console.WriteLine(activePlayer.Name + " has " + reRolls + " rerolls remaining");
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
                    if (!lockInput.Contains(letters[i])) continue;
                    lockedDice[lockedCount] = liveDice[i];
                    lockedCount++;
                }

                Console.Clear();
                reRolls--;
            }
            Array.Sort(lockedDice);
            Console.WriteLine("Your final dice are:");
            foreach (int val in lockedDice) {
                Console.WriteLine("X : " + val);
            }

            Console.WriteLine("");

            return lockedDice;
        }

        static void PrintScoreboard(Player player) {
            Console.WriteLine($"{player.Name}s current scores are:");
            for (int i = 0; i < player.Scoreboard.Length; i++) {
                Category category = player.Scoreboard[i];
                string line = letters[i] + " [";
                if (!category.IsScored) line += ("  ");
                else {
                    if (category.Val < 10)
                        line += " ";
                    line += category.Val.ToString();
                }
                line += "] " + category.Name;
                Console.WriteLine(line);
                if (i == 5) Console.WriteLine("");
            }
        }

        private static int[] RollDice(int maxVal) {
            Random rnd = new Random();
            int[] values = new int[maxVal];
            for (int i = 0; i < maxVal; i++) {
                values[i] = rnd.Next(1, 7);
            }
            return values;
        }


        private static Player[] SetupPlayers() {
            //Real
            Console.WriteLine("How many players?");
            int playerCount = int.Parse(Console.ReadLine());
            Player[] players = new Player[playerCount];
            for (int i = 0; i < playerCount; i++) {
                Console.WriteLine("What is the name of player " + (i + 1) + "?");
                string name = Console.ReadLine();
                players[i] = new Player(name);
            }
            return players;
        }

    }
}