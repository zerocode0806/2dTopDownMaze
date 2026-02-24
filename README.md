# Unity Maze Generator (DFS Backtracking)

A procedural maze generator built in Unity using C# and the Depth-First Search (DFS) Recursive Backtracking algorithm.

Press Spacebar to generate a new random maze.

---

## Overview

This project generates a perfect maze using a stack-based DFS backtracking algorithm. The maze contains no loops and ensures there is exactly one path between any two cells.

Maze generation is animated using Unity coroutines.

---

## How It Works (Short Explanation)

1. Start from an initial cell and mark it as visited.
2. Randomly select an unvisited neighbor.
3. Remove the wall between the two cells.
4. Push the current cell to a stack and move to the neighbor.
5. If no neighbors are available, backtrack using the stack.
6. Continue until all cells are visited.

---

## Features

- Procedural maze generation
- DFS with backtracking
- Coroutine-based animation
- Adjustable grid size
- Automatically centered orthographic camera
- Compatible with Unity New Input System

---

## Project Structure
Assets/
├── Scripts/
│ ├── GenerateMaze.cs
│ └── Room.cs
├── Prefabs/
│ └── Room.prefab
---

## Requirements

- Unity 2021 or newer
- New Input System enabled (or Both)
- Orthographic Camera

---

## Usage

1. Attach `GenerateMaze.cs` to an empty GameObject.
2. Assign the `Room` prefab.
3. Set `numX` and `numY`.
4. Press Play.
5. Press Spacebar to generate the maze.

---

## Credits

This project was inspired by educational resources from:

YouTube Channel: https://www.youtube.com/@faramirasg  
GitHub Repository: https://github.com/shamim-akhtar  

Special thanks for sharing valuable programming knowledge.

---

## Author

Created by Ubed Dahlan  
Unity Developer
