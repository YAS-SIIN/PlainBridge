using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainBridge.Client.Application.Services.Signal;

public class SignalService : ISignalService
{
    private ManualResetEvent mre = new ManualResetEvent(false);

    public void WaitOne() => mre.WaitOne();

    public void Set() => mre.Set();
}