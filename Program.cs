using System;
using PuzzleArena;
using PuzzleArena.Controllers;

var arena = new Arena(
    Level.Get(1),
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
            BFS(level, player, universe);
            return Directions.Dequeue();
        } else {
            return Directions.Dequeue();
        }
    }

    private void BFS(Level level, (int X, int Y) player, int universe) {
        int Start = player.Y * level.Width + player.X;

        bool[,] Visited = new bool[level.Tiles.Length, 2];
        var queue = new Queue<state>();
        int[,] Parents = new int[level.Tiles.Length, 2];
        Array.Fill(Parents, -1);

        queue.Enqueue(new state(Start, universe));
        Visited[Start, universe] = true;

        bool AlreadyInGoal = true;
        state Goal = new state(-1, 1);
        while (queue.Count != 0) {
            state current = queue.Dequeue();

            if (level.Tiles[current.index].IsGoal) {
                Goal.index = current.index;
                Goal.universe = current.universe;
                break;
            }
            AlreadyInGoal = false;

            int gravity = current.universe == 1 ? level.Width : -level.Width;
            List<int> Offsets = [-1, 1, gravity];
            foreach (int dir in Offsets) {
                state next = new state(current.index + dir, current.universe);

                if (level.Tiles[next.index].IsWall
                    || Visited[next]) {
                        continue;
                }

                if (level.Tiles[next.index].IsPortal) {
                    next.universe = level.Tiles[next.index].PortalUniverse;
                }

                Visited[next.index, next.universe] = true;
                queue.Enqueue(next);
                Parents[next.index, next.universe] = current;
            }
        }

        if (Goal.index == -1 || AlreadyInGoal) {
            this.Directions.Enqueue(MoveType.None);
            return;
        }

        var Res = new List<MoveType>();

        state Prev = Goal;
        state Curr = Goal;
        while(Parents[Curr.index, Curr.universe] != -1) {
            Curr = Parents[Prev.index, Prev.universe];
            if (Curr.index < Prev.index) {
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

struct state {
    public int index;
    public int universe;

    public state(int index, int universe) {
        this.index = index;
        this.universe = universe;
    }
}