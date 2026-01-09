
namespace snakegame
{
    public class GameState
    {
        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }
        private readonly LinkedList<Direction> dirChange = new LinkedList<Direction>();
        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        private readonly Random random = new Random();
        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols];
            Dir  = Direction.Right;
            AddSnake();
            AddFood();
        }
        private void AddSnake()
        {
            int r = Rows / 2;
            for(int c = 1; c <= 3; c++)     
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }
        private IEnumerable<Position> EmptyPositions()
        {
            for(int r = 0; r < Rows; r++)
            {
                for(int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }
        private void AddFood()
        {
            List<Position> empty= new List<Position>(EmptyPositions());
            if(empty.Count == 0) return;    
            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GridValue.Food;
        }
        public Position HeadPosition() {
            return snakePositions.First.Value;
        }
        public Position TailPosition()
        {
            return snakePositions.Last.Value;
        }
        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }
        private void AddHead(Position pos)
        {
            
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }
        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePositions.RemoveLast();
        }

        private Direction getLastDirection()
        {
            if(dirChange.Count == 0)
            {
                return Dir;
            }
            else
            {
                return dirChange.Last.Value;
            }
        }   
        private bool canChangeDirection(Direction newDir)
        {
            if(dirChange.Count == 2)
            {
                return false;
            }
            Direction lastDir = getLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();
        }

        public void ChangeDirection(Direction dir)
        {
            if(canChangeDirection(dir))
            {
                dirChange.AddLast(dir);
            }
            
        }

        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;
        }
        private GridValue WillHit(Position newHeadPos)
        {
            if (OutsideGrid(newHeadPos))
            {
                return GridValue.Outside;
            }
            if (newHeadPos == TailPosition())   
            {
                return GridValue.Empty;
            }
            return Grid[newHeadPos.Row, newHeadPos.Col];
        }
        public void Move()
        {
            if(dirChange.Count > 0)
            {
                Dir = dirChange.First.Value;
                dirChange.RemoveFirst();
            }
            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);
            if(hit == GridValue.Outside || hit == GridValue.Snake)
            {
                GameOver = true;
            }else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if(hit == GridValue.Food)
            {
                AddHead(newHeadPos);
                Score += 1;
                AddFood();
            }
        }
    }
}
