﻿using System;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http.Compression;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.Authorization;

namespace FubuMVC.Core
{
    /// <summary>
    /// Marks this input or resource type as a 
    /// known message type for the client to facilitate
    /// data aggregation
    /// </summary>
    public abstract class ClientMessage
    {
        private readonly string _messageName;

        protected ClientMessage()
        {

        }

        protected ClientMessage(string messageName)
        {
            _messageName = messageName;
        }

        public string MessageName()
        {
            return _messageName;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ClientMessageAttribute : Attribute
    {
        private readonly string _messageName;

        public ClientMessageAttribute()
        {
        }

        public ClientMessageAttribute(string messageName)
        {
            _messageName = messageName;
        }

        public string MessageName
        {
            get { return _messageName; }
        }
    }

    public static class BehaviorChainExtensions
    {
        public readonly static HttpContentEncodingFilter DefaultFilter = new HttpContentEncodingFilter(new HttpContentEncoders(new IHttpContentEncoding[] { new GZipHttpContentEncoding(), new DeflateHttpContentEncoding() }));

        public static void ApplyCompression(this BehaviorChain chain, params IHttpContentEncoding[] encodings)
        {
            if (chain.Calls.Any(x => x.HasAttribute<DoNotCompressAttribute>()))
            {
                return;
            }

            if (encodings.Any())
            {
                var filter = new HttpContentEncodingFilter(new HttpContentEncoders(encodings));
                chain.AddFilter(filter);
            }
            else
            {
                chain.AddFilter(DefaultFilter);
            }
        }

        public static bool HandlerTypeIs<T>(this BehaviorChain chain)
        {
            return chain.Calls.Any(x => x.HandlerType == typeof (T));
        }

        public static void WrapWith<T>(this BehaviorChain chain) where T : IActionBehavior
        {
            chain.InsertFirst(Wrapper.For<T>());
        }

        public static bool IsClientMessage(this Type type)
        {
            if (type == null) return false;
            return type.HasAttribute<ClientMessageAttribute>() || type.CanBeCastTo<ClientMessage>();
        }

        public static bool IsAggregatedChain(this BehaviorChain chain)
        {
            return chain.InputType().IsClientMessage() || chain.ResourceType().IsClientMessage();
        }

        public static string GetDefaultMessageName(this Type type)
        {
            return type.Name.SplitPascalCase().Replace(" ", "-").ToLower();
        }

        public static string GetMessageName(this Type type)
        {
            string messageName = null;

            if (type.HasAttribute<ClientMessageAttribute>())
            {
                messageName = type.GetAttribute<ClientMessageAttribute>().MessageName;
            }

            if (type.CanBeCastTo<ClientMessage>())
            {
                messageName = Activator.CreateInstance(type).As<ClientMessage>().MessageName();
            }

            return messageName.IsEmpty()
                ? type.GetDefaultMessageName()
                : messageName;
        }

        public static bool AnyActionHasAttribute<T>(this BehaviorChain chain) where T : Attribute
        {
            return chain.OfType<ActionCall>().Any(x => x.HasAttribute<T>());
        }
    }
}