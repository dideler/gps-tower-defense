using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace GPSTD_RM
{
    /// <summary>
    /// This is effectively the exact same as a basic projectile, but
    /// with a different collision effect.
    /// </summary>
    public class SlowProjectile : BasicProjectile
    {
            public SlowProjectile(Game g)
                : base(g)
            {
            }

            public SlowProjectile(Game g, Vector2 position, Vector2 velocity, int attackPower)
                : base(g, position, velocity, attackPower)
            {
                
            }

            /// <summary>
            /// The overrided collision effect causes a slow counter
            /// to be added rather than damage to be caused.
            /// </summary>
            /// <param name="c">The creep on which the effect will be applied.</param>
            protected override void CollisionEffect(Creep c)
            {
                c.slowCounters++;
                Alive = false;
            }
    }
}
