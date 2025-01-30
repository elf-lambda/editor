using System.Collections.Generic;
using System;
using Raylib_cs;
using System.Numerics;
namespace editor;

public enum CursorPos
{
    Left,
    Right,
    Up,
    Down,
}


class Editor
{
    public List<List<char>> _chars = new List<List<char>>();
    private int window_rows = (10 + width + 10) / Mfont.CHAR_IMAGE_WIDTH;
    private int window_cols = (10 + height + 10) / Mfont.CHAR_IMAGE_HEIGHT;
    private int rows = (10 + width + 10) / Mfont.CHAR_IMAGE_WIDTH;
    private int cols = (10 + height + 10) / Mfont.CHAR_IMAGE_HEIGHT;
    public static int width = 600;
    public static int height = 480;
    public int padding { get; set; } = 10;
    public int cursorX { get; set; }
    public int cursorY { get; set; }

    public Editor()
    {
        System.Console.WriteLine($"intializing 2d array [${rows}, ${cols}]");
        for (int i = 0; i < rows; i++)
        {
            List<char> row = new List<char>(new char[cols]);
            _chars.Add(row);
        }
        cursorX = padding;
        cursorY = padding;
    }
    public void ResetCursorX() => cursorX = padding;
    public void ResetCursorY() => cursorY = padding;

    public void ResetCursor()
    {
        cursorX = padding;
        cursorY = padding;
    }

    public void MoveCursor(CursorPos pos)
    {
        switch (pos)
        {
            case CursorPos.Left:
                {
                    cursorX -= Mfont.CHAR_IMAGE_WIDTH;
                    break;
                }
            case CursorPos.Right:
                {
                    cursorX += Mfont.CHAR_IMAGE_WIDTH;
                    break;
                }
            case CursorPos.Up:
                {
                    cursorY -= Mfont.CHAR_IMAGE_HEIGHT;
                    break;
                }
            case CursorPos.Down:
                {
                    cursorY += Mfont.CHAR_IMAGE_HEIGHT;
                    break;
                }
        }
    }

    public string GetDisplay()
    {
        string full = "";
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (_chars[i][j] != '\0')
                {
                    full += _chars[i][j];
                }
            }
        }
        return full;
    }


    private void CheckSize(int x, int y)
    {
        bool resized = false;

        // Double rows if necessary
        while (x >= rows)
        {
            int newRows = rows * 2;
            for (int i = rows; i < newRows; i++)
            {
                _chars.Add(new List<char>(new char[cols])); // Initialize new rows
            }
            rows = newRows;
            resized = true;
        }

        // Double columns in each row if necessary
        while (y >= cols)
        {
            int newCols = cols * 2;
            foreach (var row in _chars)
            {
                row.AddRange(new char[newCols - cols]); // Extend each row
            }
            cols = newCols;
            resized = true;
        }

        if (resized)
        {
            Console.WriteLine($"Resized to {rows}x{cols}");
        }
    }
    public char GetAt(int x, int y)
    {
        return _chars[x][y];
    }
    public void AddAt(int y, int x, char c)
    {
        // Reversed x,y
        CheckSize(x, y);
        _chars[x][y] = c;
    }
    public void RemoveAt(int y, int x)
    {
        _chars[x][y] = '\0';
    }
    public int GetLineWidth(int row)
    {
        if (row >= 0 && row < rows) // Ensure the row is within bounds
        {
            int width = 0;

            // Count non-default characters in the row
            foreach (var ch in _chars[row])
            {
                if (ch != '\0')
                {
                    width++;
                }
            }

            return width;
        }
        return 0;
    }
}