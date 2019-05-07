using DIKUArcade.EventBus;
using DIKUArcade.State;
using SpaceTaxi_2.SpaceTaxiStates;
using SpaceTaxiGame;

namespace SpaceTaxi_2.SpaceTaxiState {
    public class StateMachine : IGameEventProcessor<object> {
        public IGameState ActiveState { get; private set; }

        public StateMachine() {
            SpaceTaxiBus.GetBus().Subscribe(GameEventType.GameStateEvent, this);
            SpaceTaxiBus.GetBus().Subscribe(GameEventType.InputEvent, this);
            ActiveState = MainMenu.GetInstance();
        }

        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.GameStateEvent) {
                switch (gameEvent.Message) {
                case "CHANGE_STATE":
                    SwitchState(GameStateType.TransformStringToState(gameEvent.Parameter1));
                    break;
                }
            } else if (eventType == GameEventType.InputEvent) {
                ActiveState.HandleKeyEvent(gameEvent.Message, gameEvent.Parameter1);
            }
        }

        private void SwitchState(GameStateType.EnumGameStateType stateType) {
            switch (stateType) {
            case GameStateType.EnumGameStateType.GameRunning:
                ActiveState = GameRunning.GetInstance();
                break;
            case GameStateType.EnumGameStateType.GamePaused:
                ActiveState = GamePaused.GetInstance();
                break;
            case GameStateType.EnumGameStateType.MainMenu:
                ActiveState = MainMenu.GetInstance();
                break;
            }
        }
    }
}