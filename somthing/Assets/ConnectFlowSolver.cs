using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectFlowSolver : MonoBehaviour
{
     public int gridSize = 30; // Kích thước của mỗi ô trong lưới
    public float speed = 1f; // Tốc độ của animation

    public List<Color> colors;
    public List<Color> dotsColors;
    // private readonly List<Color> colors = new List<Color>
    // {
    //     new Color(34 / 255f, 34 / 255f, 34 / 255f), Color.black, Color.red, Color.yellow,
    //     Color.blue, Color.green, Color.orange, Color.magenta,
    //     Color.purple, Color.red, Color.darkGreen
    // };
    //
    // private readonly List<Color> dotsColors = new List<Color>
    // {
    //     new Color(34 / 255f, 34 / 255f, 34 / 255f), Color.black, Color.darkRed, Color.orange,
    //     Color.darkBlue, Color.darkGreen, Color.red, Color.purple,
    //     Color.magenta, Color.chocolate, Color.green
    // };

    private readonly List<List<int>> grid = new List<List<int>>
    {
        new List<int> {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        new List<int> {1, 0, 0, 2, 0, 0, 0, 0, 0, 1},
        new List<int> {1, 0, 0, 3, 4, 5, 0, 0, 0, 1},
        new List<int> {1, 0, 0, 6, 0, 0, 0, 0, 0, 1},
        new List<int> {1, 0, 0, 0, 0, 0, 6, 0, 4, 1},
        new List<int> {1, 0, 0, 0, 2, 0, 3, 0, 0, 1},
        new List<int> {1, 5, 0, 7, 0, 0, 7, 8, 0, 1},
        new List<int> {1, 9, 0, 0, 0, 8, 0, 0, 0, 1},
        new List<int> {1, 0, 0, 9, 0, 0, 0, 0, 0, 1},
        new List<int> {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
    };

    private Dictionary<int, Vector2> startNodes = new Dictionary<int, Vector2>();
    private Dictionary<int, Vector2> endNodes = new Dictionary<int, Vector2>();
    private List<Vector2> allNodes = new List<Vector2>();
    private Dictionary<int, int> distances = new Dictionary<int, int>();

    private void Start()
    {
        InitializeGame();
    }
    List<int> sortedNodes = new List<int>();
    private void InitializeGame()
    {
        // Analyze the initial grid
        for (int row = 1; row < grid.Count - 1; row++)
        {
            for (int col = 1; col < grid[row].Count - 1; col++)
            {
                int color = grid[row][col];
                if (color > 0)
                {
                    Vector2 node = new Vector2(row, col);
                    if (startNodes.ContainsKey(color))
                    {
                        endNodes[color] = node;
                        distances[color] = (int)(Mathf.Abs(node.x - startNodes[color].x) + Mathf.Abs(node.y - startNodes[color].y));
                    }
                    else
                    {
                        startNodes[color] = node;
                    }
                    allNodes.Add(node);
                }
            }
        }

        // Sort nodes based on their "taxicab" distance
        sortedNodes.Clear();
        List<int> keys = new List<int>(distances.Keys);
        while (keys.Count > 0)
        {
            int minKey = keys[0];
            int min = distances[minKey];
            foreach (int key in keys)
            {
                if (distances[key] < min)
                {
                    min = distances[key];
                    minKey = key;
                }
            }
            sortedNodes.Add(minKey);
            keys.Remove(minKey);
        }

        // Draw the grid
        DrawGrid(gridSize);

        // Start the backtracking algorithm
        SolvePuzzle(grid);
    }

    private void DrawGrid(int width)
    {
        for (int row = 1; row < grid.Count - 1; row++)
        {
            for (int col = 1; col < grid[row].Count - 1; col++)
            {
                DrawSquare(width, new Vector2(row, col), grid[row][col]);
                transform.Translate(Vector3.right * width);
            }
            transform.Translate(Vector3.up * width);
            transform.Translate(Vector3.left * width * (grid[row].Count - 2));
            transform.Translate(Vector3.right * width);
        }

        // Add Starting and Ending Nodes to the grid
        foreach (Vector2 node in allNodes)
        {
            transform.position = new Vector3(transform.position.x + node.y * width - width / 2, transform.position.y - node.x * width + width / 4, transform.position.z);
            DrawCircle(width / 4, dotsColors[(int)grid[(int)node.x][(int)node.y]]);
        }
    }

    private void DrawSquare(int width, Vector2 position, int index)
    {
        GameObject square = GameObject.CreatePrimitive(PrimitiveType.Cube);
        square.transform.localScale = new Vector3(width, 1, width);
        square.transform.position = new Vector3(position.y * width - width / 2, -position.x * width + width / 2, 0);
        square.GetComponent<Renderer>().material.color = colors[index];
    }

    private void DrawCircle(float radius, Color color)
    {
        GameObject circle = new GameObject("Circle");
        circle.AddComponent<SpriteRenderer>().color = color;
        circle.AddComponent<CircleCollider2D>().radius = radius;
        circle.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private bool SolvePuzzle(List<List<int>> currentGrid)
    {
        DrawGrid(gridSize);
        // Animation delay
        System.Threading.Thread.Sleep((int)(1000 - speed * 1000));

        if (!CheckGrid(currentGrid))
        {
            return false;
        }

        if (Solved(currentGrid))
        {
            Debug.Log("Problem Solved!");
            return true;
        }

        foreach (int color in sortedNodes)
        {
            Vector2 startNode = startNodes[color];
            Vector2 endNode = endNodes[color];

            if (Mathf.Abs(endNode.x - startNode.x) + Mathf.Abs(endNode.y - startNode.y) > 1)
            {
                List<string> directions = new List<string>();

                if (currentGrid[(int)startNode.x][(int)startNode.y + 1] == 0)
                {
                    if (endNode.y > startNode.y)
                    {
                        directions.Insert(0, "right");
                    }
                    else
                    {
                        directions.Add("right");
                    }
                }

                if (currentGrid[(int)startNode.x][(int)startNode.y - 1] == 0)
                {
                    if (endNode.y < startNode.y)
                    {
                        directions.Insert(0, "left");
                    }
                    else
                    {
                        directions.Add("left");
                    }
                }

                if (currentGrid[(int)startNode.x + 1][(int)startNode.y] == 0)
                {
                    if (endNode.x > startNode.x)
                    {
                        directions.Insert(0, "down");
                    }
                    else
                    {
                        directions.Add("down");
                    }
                }

                if (currentGrid[(int)startNode.x - 1][(int)startNode.y] == 0)
                {
                    if (endNode.x < startNode.x)
                    {
                        directions.Insert(0, "up");
                    }
                    else
                    {
                        directions.Add("up");
                    }
                }

                if (directions.Count == 0)
                {
                    return false;
                }

                foreach (string direction in directions)
                {
                    if (direction == "right")
                    {
                        startNode.y += 1;
                        currentGrid[(int)startNode.x][(int)startNode.y] = color;
                        if (SolvePuzzle(currentGrid))
                        {
                            return true;
                        }
                        else
                        {
                            // Backtrack
                            currentGrid[(int)startNode.x][(int)startNode.y] = 0;
                            startNode.y -= 1;
                        }
                    }
                    else if (direction == "left")
                    {
                        startNode.y -= 1;
                        currentGrid[(int)startNode.x][(int)startNode.y] = color;
                        if (SolvePuzzle(currentGrid))
                        {
                            return true;
                        }
                        else
                        {
                            // Backtrack
                            currentGrid[(int)startNode.x][(int)startNode.y] = 0;
                            startNode.y += 1;
                        }
                    }
                    else if (direction == "up")
                    {
                        startNode.x -= 1;
                        currentGrid[(int)startNode.x][(int)startNode.y] = color;
                        if (SolvePuzzle(currentGrid))
                        {
                            return true;
                        }
                        else
                        {
                            // Backtrack
                            currentGrid[(int)startNode.x][(int)startNode.y] = 0;
                            startNode.x += 1;
                        }
                    }
                    else if (direction == "down")
                    {
                        startNode.x += 1;
                        currentGrid[(int)startNode.x][(int)startNode.y] = color;
                        if (SolvePuzzle(currentGrid))
                        {
                            return true;
                        }
                        else
                        {
                            // Backtrack
                            currentGrid[(int)startNode.x][(int)startNode.y] = 0;
                            startNode.x -= 1;
                        }
                    }
                }

                return false;
            }
        }

        return false;
    }

    private bool CheckGrid(List<List<int>> currentGrid)
    {
        for (int row = 1; row < currentGrid.Count - 1; row++)
        {
            for (int col = 1; col < currentGrid[row].Count - 1; col++)
            {
                int color = currentGrid[row][col];

                if (color > 0)
                {
                    // Check if the cell is surrounded by different colors
                    if (currentGrid[row + 1][col] > 0 && currentGrid[row + 1][col] != color
                        && currentGrid[row - 1][col] > 0 && currentGrid[row - 1][col] != color
                        && currentGrid[row][col + 1] > 0 && currentGrid[row][col + 1] != color
                        && currentGrid[row][col - 1] > 0 && currentGrid[row][col - 1] != color)
                    {
                        return false;
                    }

                    // Check if a connection line crosses itself
                    if (currentGrid[row + 1][col] == color && currentGrid[row - 1][col] == color && currentGrid[row][col + 1] == color)
                    {
                        return false;
                    }
                    else if (currentGrid[row + 1][col] == color && currentGrid[row - 1][col] == color && currentGrid[row][col - 1] == color)
                    {
                        return false;
                    }
                    else if (currentGrid[row + 1][col] == color && currentGrid[row][col + 1] == color && currentGrid[row][col - 1] == color)
                    {
                        return false;
                    }
                    else if (currentGrid[row - 1][col] == color && currentGrid[row][col + 1] == color && currentGrid[row][col - 1] == color)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private bool Solved(List<List<int>> currentGrid)
    {
        for (int row = 1; row < currentGrid.Count - 1; row++)
        {
            for (int col = 1; col < currentGrid[row].Count - 1; col++)
            {
                if (currentGrid[row][col] == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
