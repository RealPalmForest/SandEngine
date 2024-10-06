using Microsoft.Xna.Framework;
using SandEngine.AbstractParticles;

namespace SandEngine.Particles;

public class Wood : StableParticle
{
    public Wood(GameMap parentMap) : base(parentMap)
    {
        Color = Color.SaddleBrown;
    }
}