using System;
using Docker.DotNet.Models;
using src.Interfaces;

namespace tests.Mocks
{
    public class MockDockerControl : IDockerControl
    {
        public int ApplyChangesCallCount { get; set; } = 0;
        public string SendPathToStack { get; set; }
        public bool SendRestartOnly { get; set; }
        
        public void ApplyChangesToStack(string pathToStack, bool restartOnly)
        {
            SendRestartOnly = restartOnly;
            SendPathToStack = pathToStack;
            ApplyChangesCallCount++;
        }

        public void PullImage(ImagesCreateParameters imagesCreateParameters, AuthConfig authConfig, Progress<JSONMessage> progress)
        {
            
        }

        public ImageInspectResponse InspectImage(string dockerImage)
        {
            throw new NotImplementedException();
        }

        public void DeleteImage(string dockerImage)
        {
            throw new NotImplementedException();
        }
    }
}