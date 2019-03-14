using System;
using System.Threading.Tasks;
using Docker.DotNet.Models;

namespace src.Interfaces
{
    /// <summary>
    /// Describes how to apply changes on the docker compose tack 
    /// </summary>
    public interface IDockerComposeControl
    {
        /// <summary>
        /// Applies changes to the docker compose stack
        /// </summary>
        /// <param name="pathToStack">Absolute path to the directory that contains the `docker-compose.yml file</param>
        /// <param name="restartOnly">Should the stack only be restarted instead of being re-created?</param>
        void ApplyChangesToStack(string pathToStack, bool restartOnly);

        /// <summary>
        /// Let the docker deamon pull the image
        /// </summary>
        /// <param name="imagesCreateParameters"></param>
        /// <param name="authConfig"></param>
        /// <param name="progress"></param>
        void PullImage(ImagesCreateParameters imagesCreateParameters, AuthConfig authConfig, Progress<JSONMessage> progress);

        ImageInspectResponse InspectImage(string dockerImage);
        void DeleteImage(string dockerImage);
    }
}