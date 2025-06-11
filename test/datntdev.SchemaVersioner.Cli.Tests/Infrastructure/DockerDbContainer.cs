using Docker.DotNet;
using Docker.DotNet.Models;
using System.Data;
using System.Runtime.InteropServices;

namespace datntdev.SchemaVersioner.Cli.Tests.Infrastructure
{
    public abstract class DockerDbContainer : IAsyncLifetime
    {
        private readonly DockerClient _client = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient()
            : new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();

        public abstract string ConnectionString { get; }
        public abstract IDbConnection DbConnection { get; protected set; }

        public abstract string ContainerName { get; }
        public abstract string ContainerImage { get; }
        public abstract string ContainerExposePort { get; }
        public abstract string ContainerHostPort { get; }
        public abstract string[] ContainerEnv { get; }

        public async Task BuildAndStart()
        {
            var container = (await _client.Containers.ListContainersAsync(new ContainersListParameters { All = true }))
                .FirstOrDefault(x => x.Names.Any(n => n.Equals($"/{ContainerName}", StringComparison.OrdinalIgnoreCase)));

            if (container != null)
            {
                await _client.Containers.RemoveContainerAsync(container.ID, new() { Force = true, RemoveVolumes = true });
            }

            await _client.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = ContainerImage }, null, new Progress<JSONMessage>());

            var newContainer = await _client.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = ContainerImage,
                Name = ContainerName,
                Env = ContainerEnv,
                ExposedPorts = new Dictionary<string, EmptyStruct> { { ContainerExposePort, new EmptyStruct() } },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        { ContainerExposePort, new List<PortBinding> { new() { HostIP = "localhost", HostPort = ContainerHostPort } } }
                    }
                }
            });

            await _client.Containers.StartContainerAsync(newContainer.ID, null);
        }

        public virtual async Task WaitConnection()
        {
            var timeout = TimeSpan.FromSeconds(30);
            var startTime = DateTime.UtcNow;
            while (DateTime.UtcNow - startTime < timeout)
            {
                try
                {
                    DbConnection.Open();
                    return; // Connection successful
                }
                catch (Exception)
                {
                    await Task.Delay(1000); // Wait before retrying
                }
            }
            throw new TimeoutException($"Could not connect to the database within {timeout.TotalSeconds} seconds.");
        }

        public async Task InitializeAsync()
        {
            await BuildAndStart();
            await WaitConnection();
        }

        public Task DisposeAsync()
        {
            DbConnection?.Dispose();
            return Task.CompletedTask;
        }
    }
}
