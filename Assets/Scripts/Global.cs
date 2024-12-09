static class Global {
    public static int width = 16;
    public static int height = 10;
    public static int snakeSize = 2;
    public static float[] widthMinMax;
    public static float[] heightMinMax;
    public static float loopTolerance = 0.02f;
}

public enum ObjectTypes {
    Empty,
    Egg,
    SnakeHead,
    SnakeBody
}