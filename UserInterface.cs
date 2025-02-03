namespace editor;

using System;
using System.Numerics;
using Raylib_cs;

class UserInterface
{
    public static bool DrawButton(string name, int x, int y, int width, int height, Color defaultColor, Color hoverColor)
    {
        var btn = new UIButton(x, y, width, height, defaultColor, hoverColor);
        var pressed = btn.Draw();
        Mfont.DrawText(name, x + 5, y + 5, Color.White, false);
        return pressed;
    }
}

public class UIButton : UIElement
{
    public Color DefaultColor, HoverColor, PressedColor;

    public UIButton(int x, int y, int width, int height, Color defaultColor, Color hoverColor)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        DefaultColor = defaultColor;
        HoverColor = hoverColor;
        PressedColor = Color.DarkBlue;
    }

    public override bool Draw()
    {
        Update();
        Color drawColor = IsPressed ? PressedColor : (IsHovered ? HoverColor : DefaultColor);
        Raylib.DrawRectangle(X, Y, Width, Height, drawColor);
        return IsPressed;
    }

    public override void Update()
    {
        Vector2 mousePos = Raylib.GetMousePosition();
        IsHovered = Raylib.CheckCollisionPointRec(mousePos, new Rectangle(X, Y, Width, Height));
        IsPressed = IsHovered && Raylib.IsMouseButtonPressed(MouseButton.Left);
    }
}


public abstract class UIElement
{
    public int X, Y, Width, Height;
    public bool IsHovered, IsPressed;

    public abstract void Update();
    public abstract bool Draw();
}
