﻿using Raylib_cs;
using System;
using System.IO;
using System.Numerics;
using System.Threading;
namespace editor;

class Program
{

    static void Main(string[] args)
    {
        Raylib.InitWindow(Editor.width, Editor.height, "editor");
        var fontTexture = Mfont.CreateFontTextureAtlas();
        string? display = null;
        int startX = 0;
        int startY = 0;
        var editor = new Editor();


        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            char pChar = GetTypedCharacter();

            if (pChar == '\n') // Enter Key
            {
                editor.AddAt(startX, startY, '\n');

                startY++;
                startX = 0;
                System.Console.WriteLine(editor.GetDisplay());
                System.Console.WriteLine("Enter");

                // cursorY += Mfont.CHAR_IMAGE_HEIGHT;
                // cursorX = padding;
                editor.MoveCursor(CursorPos.Down);
                editor.ResetCursorX();
                // editor.cursorX = editor.padding;

            }
            else if (pChar != '\0')
            {
                System.Console.WriteLine(pChar);
                System.Console.WriteLine($"{startX}, {startY}");
                editor.AddAt(startX, startY, pChar);
                startX++;

                // cursorX += Mfont.CHAR_IMAGE_WIDTH;
                editor.MoveCursor(CursorPos.Right);
            }

            if (Raylib.IsKeyPressedRepeat(KeyboardKey.Backspace) ||
                Raylib.IsKeyPressed(KeyboardKey.Backspace))
            {
                // If backspace
                System.Console.WriteLine("Backspace");
                if (editor.cursorX == editor.padding && editor.cursorY == editor.padding)
                {
                    display = editor.GetDisplay();
                    tempUI(display, editor);
                    Raylib.EndDrawing();
                    // Thread.Sleep(33);
                    continue;
                }
                if (editor.cursorX == editor.padding)
                {
                    if (editor.cursorY != editor.padding)
                    {
                        // if at start of line go to end of the line above
                        // cursorY -= Mfont.CHAR_IMAGE_HEIGHT;
                        editor.MoveCursor(CursorPos.Up);
                        editor.cursorX = (editor.GetLineWidth(startY - 1)) * Mfont.CHAR_IMAGE_WIDTH + editor.padding;

                        startX = editor.GetLineWidth(startY - 1);
                        startY--;


                        Raylib.EndDrawing();
                        continue;
                    }

                }

                startX--;
                editor.RemoveAt(startX, startY);

                // cursorX -= Mfont.CHAR_IMAGE_WIDTH;
                editor.MoveCursor(CursorPos.Left);

                System.Console.WriteLine(editor.GetDisplay());
            }


            // Handle Arrow Key Movements
            if (Raylib.IsKeyDown(KeyboardKey.Right))
            {
                // Move cursor right if we're not at the end of the line
                // Check if we can move right (not at end of line)
                if (startX < editor.GetLineWidth(startY))
                {
                    startX++;
                    // cursorX += Mfont.CHAR_IMAGE_WIDTH;
                    editor.MoveCursor(CursorPos.Right);
                }
            }
            else if (Raylib.IsKeyDown(KeyboardKey.Left))
            {
                // Move cursor left if we're not at the start of the line
                // Check if we can move left (not at start of line)
                if (startX > 0)
                {
                    startX--;
                    // cursorX -= Mfont.CHAR_IMAGE_WIDTH;
                    editor.MoveCursor(CursorPos.Left);
                }
            }
            else if (Raylib.IsKeyDown(KeyboardKey.Down))
            {
                // Move cursor down to the next row if not at the bottom
                // Check if there's a line below and it has content
                int nextLineWidth = editor.GetLineWidth(startY + 1);
                if (nextLineWidth > 0)  // Line exists and has content
                {
                    startY++;
                    // cursorY += Mfont.CHAR_IMAGE_HEIGHT;
                    editor.MoveCursor(CursorPos.Down);
                    // Adjust X position if current X is beyond the next line's width
                    if (startX > nextLineWidth - 1)
                    {
                        startX = nextLineWidth - 1;
                        editor.cursorX = startX * Mfont.CHAR_IMAGE_WIDTH + editor.padding;
                    }
                }
            }
            else if (Raylib.IsKeyDown(KeyboardKey.Up))
            {
                // Move cursor up to the previous row if not at the top
                // Check if we can move up (not at first line)
                if (startY > 0)
                {
                    startY--;
                    // cursorY -= Mfont.CHAR_IMAGE_HEIGHT;
                    editor.MoveCursor(CursorPos.Up);
                    // Adjust X position if current X is beyond the previous line's width
                    int prevLineWidth = editor.GetLineWidth(startY);
                    if (startX > prevLineWidth - 1)
                    {
                        startX = prevLineWidth - 1;
                        editor.cursorX = startX * Mfont.CHAR_IMAGE_WIDTH + editor.padding;
                    }
                }
            }

            if (Raylib.IsKeyDown(KeyboardKey.LeftControl) && Raylib.IsKeyPressed(KeyboardKey.S))
            {
                try
                {
                    string content = editor.GetDisplay();
                    string baseFileName = "editor_save";
                    string fileName = GetIncrementalFileName(baseFileName, "txt");

                    File.WriteAllText(fileName, content);
                    System.Console.WriteLine($"File saved successfully to: {fileName}");
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Error saving file: {ex.Message}");
                }
            }

            if (Raylib.IsFileDropped())
            {
                string[] droppedFiles = Raylib.GetDroppedFiles();
                if (droppedFiles.Length > 0)
                {
                    try
                    {
                        // Get the first dropped file (in case multiple files were dropped)
                        string filePath = droppedFiles[0];
                        string content = File.ReadAllText(filePath).Replace(System.Environment.NewLine, "\n");
                        foreach (var innerList in editor._chars)
                        {
                            for (int i = 0; i < innerList.Count; i++)
                            {
                                innerList[i] = '\0';  // Set each element to the null character
                            }
                        }
                        startX = 0;
                        startY = 0;

                        editor.ResetCursor();
                        // cursorX = padding;
                        // cursorY = padding;
                        // Add the file content character by character
                        for (int i = 0; i < content.Length; i++)
                        {
                            char c = content[i];
                            if (c == '\n')
                            {
                                editor.AddAt(startX, startY, '\n');
                                startY++;
                                startX = 0;
                                // cursorY += Mfont.CHAR_IMAGE_HEIGHT;
                                // cursorX = padding;
                                editor.MoveCursor(CursorPos.Down);
                                editor.ResetCursorX();

                            }
                            else
                            {
                                editor.AddAt(startX, startY, c);
                                startX++;
                                // cursorX += Mfont.CHAR_IMAGE_WIDTH;
                                editor.MoveCursor(CursorPos.Right);
                            }
                        }

                        System.Console.WriteLine($"Loaded file: {filePath}");
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine($"Error loading file: {ex.Message}");
                    }
                }
            }

            display = editor.GetDisplay();
            System.Console.WriteLine(display.Replace("\n", "\\n"));


            // Pseudo UI
            tempUI(display, editor);
            // Raylib.DrawRectangle(0, Editor.height - 20, Editor.width, Editor.height, Color.DarkBlue);
            // Mfont.DrawText(DateTime.Now.ToString("HH:mm:ss tt"), 10, Editor.height - 15);
            // Mfont.DrawText(display, editor.padding, editor.padding, Mfont.CHAR_IMAGE_WIDTH);
            // Mfont.DrawCharacter(Convert.ToChar(3), editor.cursorX, editor.cursorY);
            Raylib.EndDrawing();
            Thread.Sleep(33);
        }

        Raylib.CloseWindow();
    }


    static void tempUI(string display, Editor editor)
    {
        Raylib.DrawRectangle(0, Editor.height - 20, Editor.width, Editor.height, Color.DarkBlue);
        Mfont.DrawText(DateTime.Now.ToString("HH:mm:ss tt"), 10, Editor.height - 15);
        Mfont.DrawText(display, editor.padding, editor.padding, Mfont.CHAR_IMAGE_WIDTH);
        Mfont.DrawCharacter(Convert.ToChar(3), editor.cursorX, editor.cursorY);
    }

    static string GetIncrementalFileName(string baseFileName, string extension)
    {
        int counter = 1;
        string fileName = $"{baseFileName}.{extension}";

        while (File.Exists(fileName))
        {
            fileName = $"{baseFileName}_{counter:D4}.{extension}";
            counter++;
        }

        return fileName;
    }

    static char GetTypedCharacter()
    {
        bool shiftPressed = Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift);

        // Handle alphanumeric input via GetCharPressed()
        if (Raylib.IsKeyPressed(KeyboardKey.Enter)) return '\n';
        // if (Raylib.IsKeyPressed(KeyboardKey.Backspace)) return Convert.ToChar(1);

        int key = Raylib.GetCharPressed();
        if (key != 0)
        {
            // System.Console.WriteLine($"key={enter}");
            char character = (char)key;
            if (shiftPressed) return char.ToUpper(character);
            // if (shiftPressed && char.IsLetter(character))
            // {
            //     return char.ToUpper(character); // Convert lowercase letters to uppercase
            // }

            return character; // Return normal input
        }

        return '\0'; // No valid key pressed
    }
}