using DIKUArcade.EventBus;
using DIKUArcade.State;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using GalagaGame.GalagaState;
using Galaga_Exercise_3;
using Galaga_Exercise_3.GalagaStates;


namespace GalagaGame.GalagaState {
    public class StateMachine : IGameEventProcessor<object> {
        public IGameState ActiveState { get; private set; }

        public StateMachine() {
            GalagaBus.GetBus().Subscribe(GameEventType.GameStateEvent, this);
            GalagaBus.GetBus().Subscribe(GameEventType.InputEvent, this);
            ActiveState = MainMenu.GetInstance();
        }

        private void SwitchState(GameStateType.EnumGameStateType stateType) {
            switch (stateType) {
                case GameStateType.EnumGameStateType.GameRunning:
                    ActiveState = GameRunning.GetInstance();
                    break;
                /*case GameStateType.EnumGameStateType.GamePaused:
                    ActiveState = GamePaused.GetInstance();
                    break;
                */case GameStateType.EnumGameStateType.MainMenu:
                    ActiveState = MainMenu.GetInstance();
                    break;
                    
            }

            // vores kode her
            //
            //
        }

        /*public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            throw new System.NotImplementedException();
        }*/
        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.GameStateEvent) {
                switch (gameEvent.Message) {
                case "CHANGE_STATE":
                    SwitchState(GameStateType.TransformStringToState(gameEvent.Parameter1));
                    break;



                }
            } else if (eventType == GameEventType.InputEvent) {
                ActiveState.HandleKeyEvent(gameEvent.Message,gameEvent.Parameter1);
                    
                    
                }
            }
        }
    }

