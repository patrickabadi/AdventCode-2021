using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class adv021
    {
        public void Run()
        {
            var players = Parse(input2);

            //var result = RunDeterministicDice(players);
            var result = RunDiracDice(players);
            Debug.WriteLine($"Result {result}");
        }

        ulong RunDiracDice(int[] players)
        {
            Quantum(players[0], 0, players[1], 0, 1, 1);

            return Math.Max(win1, win2);
        }

        ulong win1 = 0, win2 = 0;

        void Quantum(int pos1, int score1, int pos2, int score2, int player, ulong multiplier)
        {

            int new_pos;

            // Throw dice 3 times for each player
            if (player == 1)
            {

                // Case 111 = 3
                new_pos = pos1 + 3;
                while (new_pos > 10) { new_pos -= 10; }
                if (score1 + new_pos >= 21) { win1 += multiplier; }
                else { Quantum(new_pos, score1 + new_pos, pos2, score2, 2, multiplier); }

                // Case 112 121 211 = 4
                new_pos = pos1 + 4;
                while (new_pos > 10) { new_pos -= 10; }
                if (score1 + new_pos >= 21) { win1 += multiplier * 3; }
                else { Quantum(new_pos, score1 + new_pos, pos2, score2, 2, multiplier * 3); }

                // Case 113 122 131 212 221 311 = 5
                new_pos = pos1 + 5;
                while (new_pos > 10) { new_pos -= 10; }
                if (score1 + new_pos >= 21) { win1 += multiplier * 6; }
                else { Quantum(new_pos, score1 + new_pos, pos2, score2, 2, multiplier * 6); }

                // Case 123 132 213 222 231 312 321 = 6
                new_pos = pos1 + 6;
                while (new_pos > 10) { new_pos -= 10; }
                if (score1 + new_pos >= 21) { win1 += multiplier * 7; }
                else { Quantum(new_pos, score1 + new_pos, pos2, score2, 2, multiplier * 7); }

                // Case 133 223 232 313 322 331 = 7
                new_pos = pos1 + 7;
                while (new_pos > 10) { new_pos -= 10; }
                if (score1 + new_pos >= 21) { win1 += multiplier * 6; }
                else { Quantum(new_pos, score1 + new_pos, pos2, score2, 2, multiplier * 6); }

                // Case 233 323 332 = 8
                new_pos = pos1 + 8;
                while (new_pos > 10) { new_pos -= 10; }
                if (score1 + new_pos >= 21) { win1 += multiplier * 3; }
                else { Quantum(new_pos, score1 + new_pos, pos2, score2, 2, multiplier * 3); }

                // Case 333 = 9
                new_pos = pos1 + 9;
                while (new_pos > 10) { new_pos -= 10; }
                if (score1 + new_pos >= 21) { win1 += multiplier; }
                else { Quantum(new_pos, score1 + new_pos, pos2, score2, 2, multiplier); }

            }
            else
            {

                // Case 111 = 3
                new_pos = pos2 + 3;
                while (new_pos > 10) { new_pos -= 10; }
                if (score2 + new_pos >= 21) { win2 += multiplier; }
                else { Quantum(pos1, score1, new_pos, score2 + new_pos, 1, multiplier); }

                // Case 112 121 211 = 4
                new_pos = pos2 + 4;
                while (new_pos > 10) { new_pos -= 10; }
                if (score2 + new_pos >= 21) { win2 += multiplier * 3; }
                else { Quantum(pos1, score1, new_pos, score2 + new_pos, 1, multiplier * 3); }

                // Case 113 122 131 212 221 311 = 5
                new_pos = pos2 + 5;
                while (new_pos > 10) { new_pos -= 10; }
                if (score2 + new_pos >= 21) { win2 += multiplier * 6; }
                else { Quantum(pos1, score1, new_pos, score2 + new_pos, 1, multiplier * 6); }

                // Case 123 132 213 222 231 312 321 = 6
                new_pos = pos2 + 6;
                while (new_pos > 10) { new_pos -= 10; }
                if (score2 + new_pos >= 21) { win2 += multiplier * 7; }
                else { Quantum(pos1, score1, new_pos, score2 + new_pos, 1, multiplier * 7); }

                // Case 133 223 232 313 322 331 = 7
                new_pos = pos2 + 7;
                while (new_pos > 10) { new_pos -= 10; }
                if (score2 + new_pos >= 21) { win2 += multiplier * 6; }
                else { Quantum(pos1, score1, new_pos, score2 + new_pos, 1, multiplier * 6); }

                // Case 233 323 332 = 8
                new_pos = pos2 + 8;
                while (new_pos > 10) { new_pos -= 10; }
                if (score2 + new_pos >= 21) { win2 += multiplier * 3; }
                else { Quantum(pos1, score1, new_pos, score2 + new_pos, 1, multiplier * 3); }

                // Case 333 = 9
                new_pos = pos2 + 9;
                while (new_pos > 10) { new_pos -= 10; }
                if (score2 + new_pos >= 21) { win2 += multiplier; }
                else { Quantum(pos1, score1, new_pos, score2 + new_pos, 1, multiplier); }
            }

            return;
        }

      
        int RunDeterministicDice(int[] players)
        {
            int dice = 1;
            int current_player = 0;
            var player_scores = new int[players.Length];
            int rolls = 0;

            while(true)
            {
                int total = 0;
                for (int i = 0; i < 3; i++)
                    total += (dice + i);

                rolls += 3;
                
                players[current_player] += total;
                if (players[current_player] % 10 != 0)
                    players[current_player] = players[current_player] % 10;
                else
                    players[current_player] = 10;
                player_scores[current_player] += players[current_player];

                Debug.WriteLine($"Player {current_player + 1} rolls {dice},{dice+1},{dice+2} to space {players[current_player]} for a total score of {player_scores[current_player]}");

                if (player_scores[current_player] >= 1000)
                    break;

                dice += 3;
                current_player++;
                current_player = current_player % players.Length;
            }

            var loser = ++current_player % players.Length;

            Debug.WriteLine($"Loser score {player_scores[loser]} with {rolls} rolls");

            return player_scores[loser] * rolls;
        }

        int [] Parse(string inp)
        {
            var lines = inp.Split('\n');

            var players = new int[lines.Length];
            for(int i=0; i<lines.Length; i++)
            {
                var line = lines[i];
                players[i] = line.Trim().Last() - '0';
            }
            return players;
        }

        string input = @"Player 1 starting position: 4
Player 2 starting position: 8";

        string input2 = @"Player 1 starting position: 4
Player 2 starting position: 5";
    }
}
