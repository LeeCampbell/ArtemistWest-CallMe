using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Text;
using InTheHand.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace ArtemisWest.CallMe.Shell.Services
{
    public class BluetoothActivatedIdentityListener : IActivatedIdentityListener
    {
        //private static readonly Guid CallMeServiceId  = new Guid("5DFEE4FE-A594-4BFB-B21A-6D7184330669"); //My generated random Id.
        //private static readonly Guid CallMeServiceId = new Guid("00001105-0000-1000-8000-00805F9B34FB");  //Std generic id
        private static readonly Guid CallMeServiceId = new Guid("fa87c0d0-afac-11de-8a39-0800200c9a66"); //Bluetooth chat id

        private readonly BluetoothListener _listener;
        private readonly ILogger _logger;

        public BluetoothActivatedIdentityListener(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.GetLogger();
            _listener = new BluetoothListener(CallMeServiceId);
        }

        public IObservable<IList<string>> IdentitiesActivated(IScheduler scheduler)
        {
            return Observable.Create<IList<string>>(
                o =>
                    {
                        _listener.Start();
                        var encoder = new ASCIIEncoding();
                        try
                        {
                            _logger.Debug("_listener.AcceptBluetoothClient();");
                            var bluetoothClient = _listener.AcceptBluetoothClient();
                            var ns = bluetoothClient.GetStream();
                            //TODO: Should this be a recursive call, or should I just continue on the same scheduler path? -LC
                            return ns.ToObservable(1, scheduler)
                                     .Aggregate(new List<byte>(), 
                                                (acc, cur) => 
                                                { 
                                                    acc.Add(cur);
                                                    return acc;
                                                })
                                     .Select(bytes=>encoder.GetString(bytes.ToArray()))
                                     .Select(data=>data.Split('\n'))
                                     .Subscribe(o);
                        }
                        catch(Exception ex)
                        {
                            o.OnError(ex);
                            return Disposable.Empty;
                        }
                    }).SubscribeOn(scheduler);
        }
    }
}