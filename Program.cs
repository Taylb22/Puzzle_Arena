using System;
using PuzzleArena;
using PuzzleArena.Controllers;

var arena = new Arena(
    Level.Get(8),
    new AIController()
);
arena.Start();


enum UNIVERSE {
    NORMAL = 1,
    GRAVITY = 2,
    EVEN = 3,
    SPACE = 4
}

public class AIController : Controller {
    private Queue<MoveType> Directions;
    private const int COUNT_UNIVERSES = 4;

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
        int SIZE = level.Tiles.Length;
        int Start = player.Y * level.Width + player.X;
        int Curr_Universe = universe;

        bool[] Visited = new bool[SIZE * COUNT_UNIVERSES];
        var queue = new Queue<state>();
        int[] Parents = new int[level.Tiles.Length * COUNT_UNIVERSES];
        Array.Fill(Parents, -1);

        queue.Enqueue(new state(Start, Curr_Universe));
        Visited[Start + (SIZE * (Curr_Universe - 1))] = true;

        bool AlreadyInGoal = true;
        int GoalId = -1;
        while (queue.Count != 0) {
            state current = queue.Dequeue();

            if (level.Tiles[current.index].IsGoal) {
                GoalId = StateId(current, SIZE);
                break;
            }
            AlreadyInGoal = false;

            List<int> Offsets = new();
            int n = current.index;
            int RETURN = Start;
                         //Start + (SIZE * (current.universe - 1))
            switch (current.universe) {
                case (int) UNIVERSE.NORMAL:
                    Offsets.AddRange([RETURN, n-1, n+1, n+level.Width]);
                    break;
                case (int) UNIVERSE.GRAVITY:
                    Offsets.AddRange([RETURN, n-1, n+1, n-level.Width]);
                    break;
                case (int) UNIVERSE.EVEN:
                    Offsets.AddRange([RETURN, n-2, n+2, n+level.Width]);
                    break;
                case (int) UNIVERSE.SPACE:
                    Offsets.AddRange([RETURN, n-1, n+1]);
                    break;
            }

            foreach (int dir in Offsets) {
                state next = new state(dir, current.universe);

                if (level.Tiles[next.index].IsPortal) {
                    next.universe = level.Tiles[next.index].PortalUniverse;
                }

                if (level.Tiles[next.index].IsWall
                    || Visited[StateId(next, SIZE)]) {
                        continue;
                }

                Visited[StateId(next, SIZE)] = true;
                queue.Enqueue(next);
                Parents[StateId(next, SIZE)] = StateId(current, SIZE);
            }
        }

        if (GoalId == -1 || AlreadyInGoal) {
            this.Directions.Enqueue(MoveType.None);
            return;
        }

        var Res = new List<MoveType>();

        int Prev = GoalId;
        int Curr = GoalId;
        while(Parents[Curr] != -1) {
            Curr = Parents[Prev];

            int dir = (Prev % SIZE) - (Curr % SIZE);
            if ((Prev % SIZE) == Start) {
                Res.Add(MoveType.Return);
            } else if (dir == 1
                || dir == 2) {
                Res.Add(MoveType.Right);
            } else if (dir == -1
                        || dir == -2){
                Res.Add(MoveType.Left);
            } else if (Math.Abs(dir) == level.Width) {
                Res.Add(MoveType.None);
            }
            Prev = Curr;
        }

        for (int i = Res.Count-1; i >= 0 ; i--) {
            this.Directions.Enqueue(Res[i]);
        }
    }

    private int StateId(state stat, int SIZE) {
        return stat.index + (SIZE * (stat.universe - 1));
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

struct ParentInfo {
    public MoveType Move;
}