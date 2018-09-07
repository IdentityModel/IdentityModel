// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;

namespace IdentityModel.Internal
{
    class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<Task<T>> taskFactory) :
            base(() => Task.Factory.StartNew(taskFactory).Unwrap())
        { }

        //TODO: at some point allow this
        //public AsyncLazy(Func<Task<T>> taskFactory, LazyThreadSafetyMode mode) :
        //    base(() => Task.Factory.StartNew(taskFactory).Unwrap(), mode)
        //{ }
    }
}
