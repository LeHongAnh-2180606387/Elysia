using Systems.Account.Enum;

namespace Systems.Account.Model
{
    public class SignInResult
    {
        private static AccountType accountType = AccountType.NotSignIn;
        private static string idPlayer = string.Empty;
        public static AccountType AccountType
        {
            get => accountType;
            set => accountType = value;
        }
        public static string IdPlayer
        {
            get => idPlayer;
            set => idPlayer = value;
        }
    }
}