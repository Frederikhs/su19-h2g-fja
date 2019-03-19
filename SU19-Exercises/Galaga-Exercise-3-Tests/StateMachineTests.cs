using System;
using DIKUArcade;
using NUnit.Framework;
using GalagaGame;
using GalagaGame;
using GalagaGame.GalagaState;
using DIKUArcade.EventBus;
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
using GalagaGame;
using Galaga_Exercise_3;
using Galaga_Exercise_3.GalagaEntities.Enemy;
using Galaga_Exercise_3.MovementStrategy;

namespace Galaga_Testing {
    [TestFixture]
    public class StateMachineTesting {
        private StateMachine stateMachine;
        private GameEventBus<object> eventBus;
        private Window win;
        private Game game;
        [SetUp]
        public void InitiateStateMachine() {
            DIKUArcade.Window.CreateOpenGLContext();
// Here you should:
// (1) Initialize a GalagaBus with proper GameEventTypes
        
        eventBus = GalagaBus.GetBus();
        eventBus.InitializeEventBus(new List<GameEventType> {
            GameEventType.GameStateEvent
            
        });

        stateMachine = new StateMachine();

        eventBus.Subscribe(GameEventType.GameStateEvent, stateMachine);
                
                
                
        }
        [Test]
        public void TestInitialState() {
            Assert.That(stateMachine.ActiveState, Is.InstanceOf<MainMenu>());
        }
        [Test]
        public void TestEventGamePaused() {
            GalagaBus.GetBus().RegisterEvent(
                GameEventFactory<object>.CreateGameEventForAllProcessors(
                    GameEventType.GameStateEvent,
                    this,
                    "CHANGE_STATE",
                    "GAME_PAUSED", ""));
            GalagaBus.GetBus().ProcessEventsSequentially();
            Assert.That(stateMachine.ActiveState, Is.InstanceOf<GamePaused>());
        }
    }
}