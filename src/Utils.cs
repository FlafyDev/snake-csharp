static class Utils {
    // C#'s modulo doesn't behave like modulo with negative numbers.  
    public static double mod(double x, double m)
    {
        return (x % m + m) % m;
    }
}
