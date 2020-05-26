using System.Collections.Generic;
using System.Linq;

namespace Exporter.Services
{

    public interface IService
    {
        string ServiceName { get; }
    }

    public abstract class Service
    {
        public string ServiceName => this.GetType().GetInterfaces().First().Name;
    }

    public interface IServices
    {
        T Get<T>() where T : IService;
        IService GetByName(string serviceName);
        IServices Register<T>(T service) where T : IService;
    }

    public class Services : IServices
    {
        Dictionary<string, IService> services = new Dictionary<string, IService>();

        public T Get<T>() where T : IService
        {
            return (T)services.Values.First(s => s is T);
        }

        public IService GetByName(string serviceName)
        {
            return services.FirstOrDefault(kvp => kvp.Key == serviceName).Value;
        }

        public IServices Register<T>(T service) where T : IService
        {
            services[service.ServiceName] = service;
            return this;
        }

    }

}
