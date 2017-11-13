// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;

namespace IdentityModel.Internal
{
    internal static class EventExtensions
    {
        public static void FireAndForget<T>(this EventHandler<T> theEvent, T args)
        {
            if (theEvent != null)
            {
                Task.Run(() =>
                {
                    foreach (var del in theEvent.GetInvocationList())
                    {
                        try
                        {
                            del.DynamicInvoke(args);
                        }
                        catch { }
                    }
                }).ConfigureAwait(false);
            }
            
        }
    }
}