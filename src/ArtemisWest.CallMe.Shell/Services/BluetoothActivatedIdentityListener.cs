using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Text;
using InTheHand.Net.Sockets;
using System.Reactive.Disposables;
using System.Net.Sockets;

namespace ArtemisWest.CallMe.Shell.Services
{
    public class BluetoothActivatedIdentityListener : IActivatedIdentityListener
    {
        //private static readonly Guid CallMeServiceId  = new Guid("5DFEE4FE-A594-4BFB-B21A-6D7184330669"); //My generated random Id.
        //private static readonly Guid CallMeServiceId = new Guid("00001105-0000-1000-8000-00805F9B34FB");  //Std generic id
        private static readonly Guid CallMeServiceId = new Guid("fa87c0d0-afac-11de-8a39-0800200c9a66"); //Bluetooth chat id

        private readonly BluetoothListener _listener;
        private readonly IObservable<IList<string>> _identitiesActivated;

        public BluetoothActivatedIdentityListener(ISchedulerProvider schedulerProvider)
        {
            _listener = new BluetoothListener(CallMeServiceId);
            _identitiesActivated = Observable.Create<IList<string>>(
                o =>
                    {
                        _listener.Start();
                        var encoder = new ASCIIEncoding();
                        try
                        {
                            Console.WriteLine("_listener.AcceptBluetoothClient();");
                            var bluetoothClient = _listener.AcceptBluetoothClient();
                            var ns = bluetoothClient.GetStream();

                            return ns.ToObservable(1, schedulerProvider.NewThread)
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

                    });
        }

        public IObservable<IList<string>> IdentitiesActivated
        {
            get { return _identitiesActivated; }
        }
    }
}