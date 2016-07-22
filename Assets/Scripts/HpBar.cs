namespace Assets.Scripts
{
    public class HpBar : Bar
    {
        #region Variables
        public Player Player;
        #endregion

        public new void Start () {
            ChangeFullBarValue (Player.Hp);
        }

        public void Update () {
            ChangeValue (Player.Hp);
        }
    }
}
