Fluent API for the X.509 Certificate Store
==========================================
A common place to store X.509 certificates is the Windows X.509 certificate store.
The raw APIs for the store are a bit arcane (and also slightly changed between .NET Framework and .NET Core).

The ``X509`` class is a simplified API to load certificates from the store. The following code loads a
certificate by name from the personal machine store::

    var cert = X509
                 .LocalMachine
                 .My
                 .SubjectDistinguishedName
                 .Find("CN=sts")
                 .FirstOrDefault();

You can load certs from the machine or user store and from ``My``, ``AddressBook``, ``TrustedPeople``,
``CertificateAuthority`` and ``TrustedPublisher`` respectively. You can search for subject name, thumbprint, issuer name or serial number.
