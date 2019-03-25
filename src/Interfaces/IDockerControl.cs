using System;
using System.Threading.Tasks;
using Docker.DotNet.Models;

namespace src.Interfaces
{
    /// <summary>
    /// Describes how to apply changes on the docker compose stack and how to talks to the docker engine 
    /// </summary>
    public interface IDockerControl
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

        /// <summary>
        /// Inspect the properties of a given image
        /// </summary>
        /// <param name="dockerImage">Docker image to inspect (eg. parity/parity:v2.3.3)</param>
        /// <returns>INspection results</returns>
        ImageInspectResponse InspectImage(string dockerImage);
        
        /// <summary>
        /// Remove an image from the local docker engine
        /// </summary>
        /// <param name="dockerImage">Docker image to remove (eg. parity/parity:v2.3.3)</param>
        void DeleteImage(string dockerImage);
    }
}