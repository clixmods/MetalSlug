public enum TeamEnum
{
    Neutral,
    Allies,
    Axis
}
public interface IActor
{
    public TeamEnum Team { get; }
    public int Health { get; }
    public void DoDamage(int amount);
    public void OnDown();
    
}
