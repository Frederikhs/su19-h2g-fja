using System.Collections.Generic;
using System.IO;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace Galaga_Exercise_1 {
    public class Enemy : Entity {
        
        private Game game;
        private Shape shape;
        public List<Image> enemyStrides;
        public List<Enemy> enemies;
        
        public Enemy(Game game, DynamicShape shape, IBaseImage image)
            : base(shape, image) {
            this.game = game;
            this.shape = shape;
            
        }

        public void AddEnemy(int numberEnemies) {
            float xpos = 0.25f;
            for (int i = 0; i < numberEnemies; i++) {
                enemies.Add(new Enemy(this.game,
                    new DynamicShape(new Vec2F(xpos, 1.0f), new Vec2F(0.1f, 0.1f)),
                    enemyStrides[0]));
                xpos += 0.2f;

            }
        }
    }
}