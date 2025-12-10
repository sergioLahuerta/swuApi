namespace swuApi.Enums
{
    public enum ReviewValueType
    {
        one = 10,
        oneHalf = 15,
        two = 20,
        twoHalf = 25,
        three = 30,
        threeHalf = 35,
        four = 40,
        fourHalf = 45,
        five = 50
    }

    public static class StarsToDouble
    {
        public static double ValueToDouble(this ReviewValueType value)
        {
            return (int)value / 10.0;
        }
    }
}