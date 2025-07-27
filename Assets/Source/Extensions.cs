public static class Extensions
{
    public static int Sign(this float caller) => caller > 0 ? 1 : caller < 0 ? -1 : 0;
}
