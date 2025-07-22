using System;
using System.Collections.Generic;

public class Player
{
    public string Name { get; set; }
    public int Score { get; set; } = 0;

    public List<PowerUp> AvailablePowerUps { get; set; }

    public Player(string name)
    {
        Name = name;
        AvailablePowerUps = new List<PowerUp>
        {
            PowerUp.FiftyFifty,
            PowerUp.SkipQuestion,
            PowerUp.DoublePoints
        };
    }

    public void UsePowerUp(PowerUp powerUp)
    {
        if (AvailablePowerUps.Contains(powerUp))
        {
            AvailablePowerUps.Remove(powerUp);
            Console.WriteLine($"{Name} used {powerUp}!");
        }
        else
        {
            Console.WriteLine($"{Name} has no {powerUp} left.");
        }
    }
}
