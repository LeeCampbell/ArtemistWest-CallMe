﻿using System;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace ArtemisWest.CallMe.Google
{
    [Module(ModuleName = "GoogleModule")]
    public class GoogleModule : IModule
    {
        public GoogleModule(IUnityContainer container)
        {
            Console.WriteLine("GoogleModule()");
            container.RegisterType<Contract.IProvider, GoogleProvider>();
        }
        public void Initialize()
        {
            //Register the providers. The thing that allows the user to choose google as an auth and selecte the services we request.
            //Show the available providers (and their state)
            //  Clicking on them starts the login process (Modal) so you cant try to do 2 at once
            
            //When making a service request, it should be passed via the serivce/repo that can communicate to thte Auth model that Auth has lapsed. 
            //  It should then disable the Provider and it's child services
            Console.WriteLine("GoogleModule.Initialize()");



        }
    }
}