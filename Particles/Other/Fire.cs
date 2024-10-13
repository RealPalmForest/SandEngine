using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SandEngine.AbstractParticles;
using SandEngine.Particles.Gasses;
using SandEngine.Particles.Stable;

namespace SandEngine.Particles.Other;

public class Fire : MovingParticle
{
    private float minFireDamage = 0.01f;
    private float maxFireDamage = 0.03f;

    public Fire(GameMap parentMap) : base(parentMap)
    {
        Acceleration = 0;
        Gravity = -1;

        int clr = Globals.Random.Next(4);
        switch (clr)
        {
            case 0:
                Color = Color.Goldenrod;
                break;
            case 1:
                Color = Color.Orange;
                break;
            case 2:
                Color = Color.OrangeRed;
                break;
            case 3:
                Color = Color.Red;
                break;
        }
    }

    public override void Update()
    {
        if (Spread() && GetAbove() == null)
        {
            if ((GetBelow() == null || GetBelow() is Smoke || GetBelow() is Fire) &&
            (GetBelowLeft() == null || GetBelowLeft() is Smoke || GetBelowLeft() is Fire) &&
            (GetBelowRight() == null || GetBelowRight() is Smoke || GetBelowRight() is Fire))
                Move(X, Y - 1);
        }

        if (Lifetime > 50)
            Die();
    }

    /// <summary>
    /// Attempts to spread to surrounding flammable particles
    /// </summary>
    /// <returns>Returns true if this particle survived</returns>
    private bool Spread()
    {
        foreach (Particle particle in parentMap.GetParticlesAround(X, Y))
        {
            if (particle == null)
                continue;

            if (particle.FireResistance >= 0)
            {
                if (particle.FireResistance == 0)
                {
                    Particle createdFire = new Fire(parentMap);
                    createdFire.HasBeenUpdated = true;

                    particle.Replace(createdFire);

                    if (Globals.Random.Next(2) == 0)
                    {
                        Die();
                        return false;
                    }
                }
                else
                {
                    particle.FireResistance -= Globals.NextFloat(minFireDamage, maxFireDamage);
                    if(particle.FireResistance < 0) particle.FireResistance = 0;
                }
            }
        }

        return true;
    }

    private void Die()
    {
        if (Globals.Random.Next(5) == 0)
            Replace(new Smoke(parentMap));
        else Remove();
    }
}