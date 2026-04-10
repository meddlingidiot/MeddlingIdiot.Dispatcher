namespace Automation.Dispatcher;

public interface IBaseRequest { }

public interface IRequest<out TResponse> : IBaseRequest { }

public interface IRequest : IBaseRequest { }
