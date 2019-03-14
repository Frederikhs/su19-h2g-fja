using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using DIKUArcade;
using DIKUArcade.EventBus;
using DIKUArcade.Timers;
using DIKUArcade.Graphics;
using DIKUArcade.Entities;
using DIKUArcade.Math;
using DIKUArcade.Physics;
using Galaga_Exercise_2.GalagaEntities.Enemy;


namespace Galaga_Exercise_2 {

    public class Game : IGameEventProcessor<object> {
        private Window win;
        private Player player;
        private DIKUArcade.Timers.GameTimer gameTimer;
        private GameEventBus<object> eventBus;
        
        
        //ENEMY
        private List<Image> enemyStrides;
        private List<Enemy> enemies;
        private Enemy newEnemy;
        private ImageStride enemyAnimation;
        
        private ISquadron enemyFormation;
        private ZigZagDown Movement;
        
        //PLAYERSHOTS
        public List<PlayerShot> playerShots { get; private set; }
        
        //EXPLOSIONS
        private List<Image> explosionStrides;
        private AnimationContainer explosions;
        private ImageStride explosionStride;
        
        //SCORE
        private Score score;
        
        public Game() {
            //CREATING GAME WINDOW
            win = new Window("Window", 500, 500);
            gameTimer = new GameTimer(60, 60);
            
            //CREATING THE PLAYER
            player = new Player(this,
                new DynamicShape(new Vec2F(0.45f, 0.1f), new Vec2F(0.1f, 0.1f)),
                new Image(Path.Combine("Assets", "Images", "Player.png")));
            
            //CREATING 4 STRIDES OF 1 IMAGE FOR MONSTERS
            enemyStrides = ImageStride.CreateStrides(4,
                Path.Combine("Assets", "Images", "BlueMonster.png"));

            //CREATING NEW ANIMATION BASED ON IMAGE LIST FOR MONSTERS
            enemyAnimation = new ImageStride(80,enemyStrides);
            enemyFormation = new PairsFormation(4);
            enemyFormation.CreateEnemies(enemyStrides);
            
            //Move enemies
            Movement = new ZigZagDown();
            
            
            
            //CREATING LIST FOR ENEMIES TO BE IN
            enemies = new List<Enemy>();
            
            //CREATING EVENTBUS TO LISTEN
            eventBus = new GameEventBus<object>();
            eventBus.InitializeEventBus(new List<GameEventType> {
                GameEventType.PlayerEvent, //player events
                GameEventType.InputEvent, //key press / key release
                GameEventType.WindowEvent //messages to the window
            });
            win.RegisterEventBus(eventBus);
            eventBus.Subscribe(GameEventType.PlayerEvent, player);
            eventBus.Subscribe(GameEventType.InputEvent, this);
            eventBus.Subscribe(GameEventType.WindowEvent, this);

            //CREATING LIST OF PLAYERSHOTS
            playerShots = new List<PlayerShot>();
            
            //ANIMATIONS - 8 STRIDES FOR EXPLOSIONS
            explosionStrides = ImageStride.CreateStrides(8,
                Path.Combine("Assets", "Images", "Explosion.png"));
            explosions = new AnimationContainer(enemyFormation.MaxEnemies);
            explosionStride = new ImageStride(explosionLength / 8, explosionStrides);

            //CREATING SCORE
            score = new Score(new Vec2F(0.45f,-0.12f), new Vec2F(0.2f,0.2f));
        }

        //FOR DETECTING IF GAME IS OVER, IF GAME IS OVER PLAYER FLIES UP AND GAME ENDS
        public bool IsGameOver() {
            if (enemyFormation.Enemies.CountEntities() > 0) {
                return false;
            } else {
                player.Direction(new Vec2F(0.00f, 0.01f));
                return true;
            }    
        }

        public void AddEnemy(int count) {
            //FIRST X POS OF FIRST ENEMY
            float xPos = 0.0625f;
            
            //CREATING COUNT ENEMIES AND ADDING TO enemies
            for (int i = 0; i < count; i++) {
            newEnemy = new Enemy(this,
                new DynamicShape(new Vec2F(xPos, 0.9f), new Vec2F(0.1f, 0.1f)),
                enemyAnimation, new Vec2F(0f,0f));
            enemies.Add(newEnemy);
            //INCREMENTING XPOS WITH 0.10625f
            xPos += 0.10625f;
            }
        }
        
        public void IterateShots() {
            foreach (var shot in playerShots) {
                shot.Shape.Move();
                if (shot.Shape.Position.Y > 1.0f) {
                    shot.DeleteEntity();
                }
                foreach (Enemy enemy in enemyFormation.Enemies) {
                    //CREATING DYNAMISK SHAPES
                    var shotDyn = shot.Shape.AsDynamicShape();
                   
                    //CHECKS IF THERES A COLLISION
                    if (CollisionDetection.Aabb(shotDyn, enemy.Shape).Collision) {
                        
                        //DELETES BOTH ENEMY AND SHOT
                        enemy.DeleteEntity();
                        shot.DeleteEntity();
                        
                        //CREATES EXPLOSION AT ENEMY POSITION
                        AddExplosion(enemy.Shape.Position.X,enemy.Shape.Position.Y,0.1f,0.1f);
                        
                        //ADDS A POINT TO THE SCORE
                        score.AddPoint();
                    }
                }
            }
            
            // IF COLLISION HAPPENED REMOVE ENEMY FROM OLD LIST AND CREATE NEW LIST
            var newEnemies = new List<Enemy>();
                        
            foreach (Enemy enemy in enemyFormation.Enemies) {
                if (!enemy.IsDeleted()) {
                    newEnemies.Add(enemy);
                }
            }
            
            enemyFormation.Enemies.ClearContainer();
            foreach (Enemy enemy in newEnemies) {
                if (!enemy.IsDeleted()) {
                    enemyFormation.Enemies.AddDynamicEntity(enemy);
                }
            }
            
            // IF COLLISION HAPPENED REMOVE PLAYERSHOT FROM OLD LIST AND CREATE NEW LIST
            List<PlayerShot> newPlayerShots = new List<PlayerShot>();
            foreach (PlayerShot aShot in playerShots) {
                if (!aShot.IsDeleted()) {
                    newPlayerShots.Add(aShot);
                } }
            playerShots = newPlayerShots;
        }
        
        //FUNCTION FOR DISPLAYING EXPLOSION
        private int explosionLength = 500;
        public void AddExplosion(float posX, float posY,
            float extentX, float extentY) {
            explosions.AddAnimation(
                new StationaryShape(posX, posY, extentX, extentY), explosionLength,
                explosionStride);
        }


        public void GameLoop() {
            while (win.IsRunning()) {
                gameTimer.MeasureTime();
                while (gameTimer.ShouldUpdate()) {
                    win.PollEvents();

                    //LISTEN FOR EVENTS
                    eventBus.ProcessEvents();
                    
                    //CHECK IF PLAYER HAS MOVEN
                    Movement.MoveEnemies(enemyFormation.Enemies);
                    player.Move();
                    //ANIMATE SHOTS
                    IterateShots();
                    
                  
                }
                

                if (gameTimer.ShouldRender()) {
                    win.Clear();
                    player.RenderEntity();
                    
                    //RENDER EACH ENEMY IN LIST enemies
//                    foreach (var anEnemy in enemyFormation) {
//                        anEnemy.RenderEntity();
//                    }
                    enemyFormation.Enemies.RenderEntities();

                    //RENDER EACH SHOT
                    foreach (var aShot in playerShots) {
                        aShot.RenderEntity();
                    }
                    //RENDER EXPLOSIONS
                    explosions.RenderAnimations();

                    //WHILE GAME IS RUNNING SHOW SCORE, THEN SHOW GAME OVER
                    if (!IsGameOver()) {
                        score.RenderScore();                        
                    } else {
                        score.GameOver();
                    }
                    
                    win.SwapBuffers();
                }

                if (gameTimer.ShouldReset()) {
                    // 1 second has passed - display last captured ups and fps
                    win.Title = "Galaga | UPS: " + gameTimer.CapturedUpdates +
                                ", FPS: " + gameTimer.CapturedFrames;
                }

            }
        }


        // Key methods
        // TODO: Tilføj is game over
        public void KeyPress(string key) {
            switch (key) {
            case "KEY_ESCAPE":
                eventBus.RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.WindowEvent, this, "CLOSE_WINDOW", "", ""));
                break;
            case "KEY_LEFT":
                eventBus.RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "move_left", "", ""));
                break;
            case "KEY_RIGHT":
                eventBus.RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "move_right", "", ""));
                break;
            case "KEY_SPACE":
                eventBus.RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "shoot", "", ""));
                break;
            }
        }

        public void KeyRelease(string key) {
            switch (key) {
            case "KEY_LEFT":
                eventBus.RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "stop_left", "", ""));
                break;
                
            case "KEY_RIGHT":
                eventBus.RegisterEvent(
                    GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.PlayerEvent, this, "stop_right", "", ""));
                break;
            default:
                break;
            }
        }


        // Process events
        public void ProcessEvent(GameEventType eventType,
            GameEvent<object> gameEvent) {
            if (eventType == GameEventType.WindowEvent) {
                switch (gameEvent.Message) {
                case "CLOSE_WINDOW":
                    win.CloseWindow();
                    break;
                default:
                    break;
                }
            } else if (eventType == GameEventType.InputEvent) {
                switch (gameEvent.Parameter1) {
                case "KEY_PRESS":
                    KeyPress(gameEvent.Message);
                    break;
                case "KEY_RELEASE":
                    KeyRelease(gameEvent.Message);
                    break;
                }
            }
        }

        public EntityContainer<Enemy> Enemies { get; }
        public int MaxEnemies { get; }
        public void CreateEnemies(List<Image> enemyStrides) {
            throw new NotImplementedException();
        }
    }
}