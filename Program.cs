using System;
using PuzzleArena;
using PuzzleArena.Controllers;

var arena = new Arena(
    Level.Get(2),
    new AIController()
);
arena.Start();

public class AIController : Controller {
    private Queue<MoveType> Directions;

    public AIController () {
        this.Directions = new Queue<MoveType>();
    }

    public override MoveType Move (
        Level level,
        int playerX,
        int playerY,
        int universe
    ) {
        if (this.Directions.Count == 0) {
            (int X, int Y) player = (playerX, playerY);
            BFS(level, player);
            return Directions.Dequeue();
        } else {
            return Directions.Dequeue();
        }
    }

    private void BFS(Level level, (int X, int Y) player) {
        int Start = player.Y * level.Width + player.X;

        bool[] Visited = new bool[level.Tiles.Length];
        var queue = new Queue<int>();
        int[] Parents = new int[level.Tiles.Length];
        Array.Fill(Parents, -1);

        queue.Enqueue(Start);
        Visited[Start] = true;

        bool AlreadyInGoal = true;
        int Goal = -1;
        while (queue.Count != 0) {
            int current = queue.Dequeue();

            if (level.Tiles[current].IsGoal) {
                Goal = current;
                break;
            }
            AlreadyInGoal = false;

            List<int> Offsets = [-1, 1, level.Width];
            foreach (int dir in Offsets) {
                int next = current + dir;

                if (level.Tiles[next].IsWall
                    || Visited[next]) {
                        continue;
                }

                Visited[next] = true;
                queue.Enqueue(next);
                Parents[next] = current;
            }
        }

        if (Goal == -1 || AlreadyInGoal) {
            this.Directions.Enqueue(MoveType.None);
            return;
        }

        var Res = new List<MoveType>();

        int Prev = Goal;
        int Curr = Goal;
        while(Parents[Curr] != -1) {
            Curr = Parents[Prev];
            if (Curr < Prev) {
                Res.Add(MoveType.Right);
            } else {
                Res.Add(MoveType.Left);
            }
            Prev = Curr;
        }

        for (int i = Res.Count-1; i >= 0 ; i--) {
            this.Directions.Enqueue(Res[i]);
        }
    }
}