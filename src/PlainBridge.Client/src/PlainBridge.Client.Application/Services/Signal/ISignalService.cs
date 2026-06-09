namespace PlainBridge.Client.Application.Services.Signal;

public interface ISignalService
{
    void Set();
    void WaitOne();
}