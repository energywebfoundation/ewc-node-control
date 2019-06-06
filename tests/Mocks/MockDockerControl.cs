using System;
using Docker.DotNet.Models;
using src.Interfaces;

namespace tests.Mocks
{
    public class MockDockerControl : IDockerControl
    {
        private readonly string _inspectReturn;

        public int ApplyChangesCallCount { get; private set; }
        public string SendPathToStack { get; private set; }
        public bool SendRestartOnly { get; private set; }


        public MockDockerControl(string expectedInspectId = "")
        {
            _inspectReturn = expectedInspectId;
        }

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
            return new ImageInspectResponse
            {
                ID = _inspectReturn
            };
        }

        public void DeleteImage(string dockerImage)
        {
        }
    }
}