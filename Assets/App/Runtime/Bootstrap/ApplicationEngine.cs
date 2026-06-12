using System;
using UnityRequestQueue.Runtime.Audio;
using UnityRequestQueue.Runtime.AssetManagement;
using UnityRequestQueue.Runtime.Network;
using UnityRequestQueue.Runtime.Pooling;
using UnityRequestQueue.Runtime.Presentation;
using UnityRequestQueue.Runtime.Sections;
using UnityEngine;
using Zenject;

namespace UnityRequestQueue.Runtime.Bootstrap
{
    public sealed class ApplicationEngine : IDisposable
    {
        private const string RootName = "APP_ENGINE";

        private static ApplicationEngine s_current;

        private GameObject _root;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Start()
        {
            var engine = new ApplicationEngine();
            s_current = engine;

            try
            {
                engine.Initialize();
                UnityEngine.Application.quitting += Stop;
            }
            catch
            {
                s_current = null;
                engine.Dispose();
                throw;
            }
        }

        private static void Stop()
        {
            UnityEngine.Application.quitting -= Stop;
            s_current?.Dispose();
            s_current = null;
        }

        private void Initialize()
        {
            var container = new DiContainer();
            var root = new GameObject(RootName);
            UnityEngine.Object.DontDestroyOnLoad(root);

            try
            {
                var builder = new AppModuleBuilder(container);
                RegisterModules(builder);

                _root = root;

                CreateEventLoop(container, root);
            }
            catch
            {
                _root = null;
                DestroyRoot(root);
                throw;
            }
        }

        private void RegisterModules(IAppModuleBuilder builder)
        {
            builder.RegisterModule(new CoreModule());
            builder.RegisterModule(new NetworkModule());
            builder.RegisterModule(new AssetModule());
            builder.RegisterModule(new AudioModule());
            builder.RegisterModule(new SectionsModule());
            builder.RegisterModule(new PoolingModule());
            builder.RegisterModule(new PresentationModule());
        }

        private static void CreateEventLoop(DiContainer container, GameObject root)
        {
            var eventLoopObject = new GameObject("EventLoop");
            eventLoopObject.transform.SetParent(root.transform, false);

            var eventLoop = eventLoopObject.AddComponent<ApplicationEventLoop>();
            container.Inject(eventLoop);
        }

        public void Dispose()
        {
            var root = _root;

            _root = null;

            DestroyRoot(root);
        }

        private static void DestroyRoot(GameObject root)
        {
            if (root == null)
            {
                return;
            }

            if (UnityEngine.Application.isPlaying)
            {
                UnityEngine.Object.Destroy(root);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }
    }
}
