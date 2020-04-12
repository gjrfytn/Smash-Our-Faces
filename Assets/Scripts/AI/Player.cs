namespace Sof.AI
{
    public class Player : Model.Game.IPlayer
    {
        public interface IGameLoop
        {
            event System.Action Ticked;
        }

        private bool _ShouldAct;

        public event System.Action Acted;

        public Player(IGameLoop gameLoop)
        {
            gameLoop.Ticked += GameLoop_Ticked;
        }

        public void Act()
        {
            _ShouldAct = true;
        }

        private void GameLoop_Ticked()
        {
            if (_ShouldAct)
            {
                _ShouldAct = false;
                Acted?.Invoke();
            }
        }
    }
}
