namespace src
{
    public class DockerControl
    {

        private string _dockerSocket;
        
        public DockerControl(string sock)
        {
            _dockerSocket = sock;
        }


        public void UpdateContainer(string name, string newImage)
        {
            
        }
    }
}