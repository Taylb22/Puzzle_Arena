using System;
using PuzzleArena;
using PuzzleArena.Controllers;

var arena = new Arena(
    Level.Get(8),
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

    private void BFS(Level level, (int X, int Y) player, int START_UNIVERSE) {
        int WIDTH = level.Width;
        int START = player.Y * WIDTH + player.X;

        //Data Structures for BFS implementation
        var Visited = new Dictionary<State, ParentInfo>();
        Queue<State> queue = new Queue<State>();

        //Starting the queue for the BFS
        State Start_State = new State(START, START_UNIVERSE)
        queue.Enqueue(Start_State);
        Visited.Add(
            Start_State,
            new ParentInfo();
        );

        //Reaching for Neighbours
        State Goal = new State();
        while (queue.Count != 0) {
            State Current = queue.Dequeue();
            
            //Finishing after finding the GOAL
            if (level.Tile[Current.Index].IsGoal) {
                Goal = Current;
                break;
            }

            //Verifying the RETURN option

            //Going through each direction
            int CURRENT_UNIVERSE = Current.Universe;
            foreach () {

            }
        }

        //Validate if GOAL was Found

        //Reconstructing Path
    }
}

public struct State {
    public int? Index;
    public int? Universe;

    public State () {}
    public State (int index, int universe) {
        this.Index = index;
        this.Universe = universe;
    }
}

public struct ParentInfo {
    public State? Parent;
    public MoveType? Move;

    public ParentInfo () {}
}