using Microsoft.Xna.Framework.Graphics;

namespace SnowWanderer {
    internal interface IDrawable {
        public void Draw(SpriteBatch spriteBatch);
        public void Update();
    }
}
