.. _refRequestUrl:
Creating Request URLs
=====================
The ``RequestUrl`` class is a helper for creating URLs with query string parameters, e.g.::

    var ru = new RequestUrl("https://server/endpoint");
    
    // produces https://server/endpoint?foo=foo&bar=bar
    var url = ru.Create(new 
        {
            foo: "foo",
            bar: "bar"
        });

As a parameter to the ``Create`` method you can either pass in an object, or a string dictionary.
In both cases the properties/values will be serialized to key/value pairs.

.. note:: All values will be URL encoded.

``RequestUrl`` is mostly useful when you create extension methods for modelling domain specific URL structures.
For examples see the :ref:`Authorize Endpoint <refAuthorize>` and :ref:`EndSession Endpoint <refEndSession>`.


