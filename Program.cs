using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Player {
    public string name;
    public int id;
    public Scores scores = new Scores();

    public Player(string _name) {
        name = _name;
    }
}

public class Scores {
    public int ones;
    public int twos;
    public int threes;
    public int fours;
    public int fives;
    public int sixes;
    public int onePair;
    public int twoPair;
    public int threeOfAKind;
    public int fourOfAKind;
    public int smallStraight;
    public int largeStraight;
    public int fullHouse;
    public int chance;
    public int yatzy;
}



namespace Yatzy {
    class Program {

        static void Main(string[] args) {
            Player[] players = SetupPlayers();
            int playerTurn = 0;
            Console.WriteLine(players.Length);
            Console.ReadKey();
            bool gameOver = false;
            while (gameOver == false) {
                int lockedCount = 0;
                int reRolls = 2;
                int[] lockedDice = new int[5];
                int[] liveDice = RollDice(5);
                while (lockedCount < 5) {
                    Console.WriteLine("");

                    Console.WriteLine("");

                }


            }



        }

        static int[] RollDice(int count) {
            Random rnd = new Random();
            int[] values = new int[count];
            for (int i = 0; i < count; i++) {
                values[i] = rnd.Next(1, 6);
            }
            return values;
        }

        static Player[] SetupPlayers() {
            Console.WriteLine("How many players?");
            int playerCount = int.Parse(Console.ReadLine());
            Console.WriteLine(playerCount);
            Console.ReadKey();
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
