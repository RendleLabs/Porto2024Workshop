namespace TrafficCamera.Generator;

public static class RandomExtensions
{
    public static T Take<T>(this Random random, IList<T> list)
    {
        return list[random.Next(list.Count)];
    }
}