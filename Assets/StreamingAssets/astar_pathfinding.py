import json
import heapq
import sys

def astar(start, goal, grid):
    try:
        open_list = []
        heapq.heappush(open_list, (0, start))
        came_from = {}
        g_score = {start: 0}
        f_score = {start: heuristic(start, goal)}
        path = []

        while open_list:
            current = heapq.heappop(open_list)[1]

            if current == goal:
                while current in came_from:
                    path.append(current)
                    current = came_from[current]
                path.append(start)
                path.reverse() 
                return path

            for neighbor in get_neighbors(current, grid):
                tentative_g_score = g_score[current] + 1  # Bisa diperbarui untuk diagonal
                
                if neighbor not in g_score or tentative_g_score < g_score[neighbor]:
                    came_from[neighbor] = current
                    g_score[neighbor] = tentative_g_score
                    f_score[neighbor] = tentative_g_score + heuristic(neighbor, goal)
                    heapq.heappush(open_list, (f_score[neighbor], neighbor))

        return []  # Jika tidak ditemukan path

    except Exception as e:
        print(f"Error dalam A* algorithm: {e}")
        sys.exit(1)  # Hentikan program jika terjadi error

def heuristic(a, b):
    return abs(a[0] - b[0]) + abs(a[1] - b[1])  # Bisa diganti dengan heuristik lain

def get_neighbors(node, grid):
    try:
        x, y = node
        neighbors = []

        directions = [
            (-1, 0), (1, 0), (0, -1), (0, 1), 
            (-1, -1), (-1, 1), (1, -1), (1, 1)  # Diagonal bisa disesuaikan
        ]

        for dx, dy in directions:
            nx, ny = x + dx, y + dy
            if 0 <= nx < len(grid) and 0 <= ny < len(grid[0]) and grid[nx][ny] == 0:
                neighbors.append((nx, ny))

        return neighbors
    except Exception as e:
        print(f"Error dalam mendapatkan tetangga: {e}")
        sys.exit(1)  # Hentikan program jika terjadi error

def find_path(start, goal, grid):
    try:
        path = astar(start, goal, grid)
        path_dicts = [{"x": point[0], "y": point[1]} for point in path]
        return json.dumps(path_dicts)
    except Exception as e:
        print(f"Error dalam menemukan path: {e}")
        sys.exit(1)  # Hentikan program jika terjadi error

def main():
    try:
        if len(sys.argv) != 4:
            print("Penggunaan: python find_path.py <start> <goal> <grid_json>")
            sys.exit(1)

        start = tuple(map(int, sys.argv[1].strip("()").split(",")))  # Parsing start
        goal = tuple(map(int, sys.argv[2].strip("()").split(",")))    # Parsing goal
        grid_json = sys.argv[3]

        try:
            grid = json.loads(grid_json)  # Parsing grid JSON
        except json.JSONDecodeError as e:
            print(f"Error decoding grid JSON: {e}")
            sys.exit(1)

        path_json = find_path(start, goal, grid)
        print(path_json)
    
    except ValueError as e:
        print(f"Error parsing input: {e}")
        sys.exit(1)  # Hentikan program jika ada error dalam parsing input
    except Exception as e:
        print(f"Unexpected error: {e}")
        sys.exit(1)  # Hentikan program jika ada error tak terduga

if __name__ == "__main__":
    main()
