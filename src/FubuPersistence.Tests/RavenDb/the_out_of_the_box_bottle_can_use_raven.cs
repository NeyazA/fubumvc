﻿using System;
using System.Diagnostics;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using FubuPersistence.RavenDb;
using FubuTestingSupport;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Document;
using Raven.Database.Server;
using StructureMap;

namespace FubuPersistence.Tests.RavenDb
{
    [TestFixture]
    public class the_out_of_the_box_bottle_can_use_raven
    {
        [Test]
        public void raven_is_available_out_of_the_box()
        {
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8080);

            var container = new Container();
            using (var application = FubuApplication.DefaultPolicies().StructureMap(container).Bootstrap())
            {
                container.GetInstance<IDocumentStore>().ShouldNotBeNull();
            }
        }

        [Test]
        public void can_run_silverlight_from_embedded()
        {
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8080);

            var container = new Container();
            var settings = RavenDbSettings.InMemory();
            //settings.Url = "http://localhost:8080";
            settings.UseEmbeddedHttpServer = true;
            container.Inject(settings);

            var store = settings.Create();
            store.Initialize();

            Debug.WriteLine(store.Url);

            //Process.Start("http://localhost:8080");

            var store2 = new DocumentStore
            {
                Url = "http://localhost:8080",
                DefaultDatabase = "Portal"
            };

            store2.Initialize();

            var entity1 = new FakeEntity {Id = Guid.NewGuid()};

            using (var session = store2.OpenSession())
            {
                session.Store(entity1);
                session.SaveChanges();
            }

            using (var session2 = store2.OpenSession())
            {
                session2.Query<FakeEntity>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                    .Any().ShouldBeTrue();
            }
        }
    }

    public class FakeEntity : Entity
    {
    }

    public class SampleEntity : Entity
    {
        public string Name { get; set; }
    }
}