using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleIdentityTest
{
    public class CustomIdentity : IIdentity
    {
        public const string AnonymousAuthenticationType = "AnonymousAuthenticationType";

        public static CustomIdentity GetAnonymousIndentity()
        {
            return new CustomIdentity("AnonymousUser", AnonymousAuthenticationType);
        }

        public CustomIdentity(string name, string authenticationType)
        {
            Name = name;
            AuthenticationType = authenticationType;
        }

        public string Name { get; }
        public string AuthenticationType { get; }

        public bool IsAuthenticated
        {
            get { return !string.IsNullOrEmpty(Name) && AuthenticationType != CustomIdentity.AnonymousAuthenticationType; }
        }
    }

    public class MyCustomPrincipal : MarshalByRefObject, IPrincipal
    {
        public static MyCustomPrincipal GetAnonymousPrincipal() 
        {
            return new MyCustomPrincipal(CustomIdentity.GetAnonymousIndentity(), new string[] { });
        }

        public MyCustomPrincipal(CustomIdentity identity, string[] roles)
        {
            Identity = identity;
            if (roles != null) 
                Roles.AddRange(roles);
        }

        public bool IsInRole(string role)
        {
            if (Roles.Contains(role))
                return true;
            return false;

        }

        public IIdentity Identity { get; }
        public List<string> Roles { get; set; } = new List<string>();
    }
    
    [TestClass]
    public class TestClass
    {
        [AssemblyInitialize]
        public static void InitializeAuthentication(TestContext testContext)
        {
            Thread.CurrentPrincipal = MyCustomPrincipal.GetAnonymousPrincipal();
        }

        [TestMethod]
        public void Test1()
        {
            Assert.IsInstanceOfType(Thread.CurrentPrincipal, typeof(MyCustomPrincipal));
        }
        
        [TestMethod]
        public void Test2()
        {
            Assert.IsInstanceOfType(Thread.CurrentPrincipal, typeof(MyCustomPrincipal));
        }
    }
}