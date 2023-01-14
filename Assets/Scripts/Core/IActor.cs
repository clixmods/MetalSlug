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
    /// <summary>
    /// Method executed when the actor is dead
    /// </summary>
    public void OnDown();
    
}
