using UnityEngine;

public enum ThemeType
{
    blue,
    forest,
    pink,
    snow,
}
[System.Serializable]
public class GameTheme
{
    public Sprite cloud;
    public Sprite bg;
    public Sprite platform;
    public Sprite platformCracked;
    public ThemeType themeType;
}
