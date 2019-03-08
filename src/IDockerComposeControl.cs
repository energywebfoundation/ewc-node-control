namespace src
{
    public interface IDockerComposeControl
    {
        void ApplyChangesToStack(string pathToStack, bool restartOnly);
    }
}