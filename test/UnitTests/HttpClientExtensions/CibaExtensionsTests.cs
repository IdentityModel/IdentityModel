// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FluentAssertions;
using IdentityModel.Client;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IdentityModel.UnitTests
{
    public class CibaExtensionsTests
    {
        private const string Endpoint = "http://server/backchannel";

        [Fact]
        public async Task Http_request_should_have_correct_format()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");

            var client = new HttpClient(handler);
            var request = new BackchannelAuthenticationRequest
            {
                Address = Endpoint,
                ClientId = "client",
                
                Scope = "scope",
                AcrValues = "acr_values",
                BindingMessage = "binding_message",
                ClientNotificationToken = "client_notification_token",
                UserCode = "user_code",
                
                RequestedExpiry = 1,
                
                IdTokenHint = "id_token_hint",
                LoginHintToken = "login_hint_token",
                LoginHint = "login_hint",
                
                Resource =
                {
                    "resource1",
                    "resource2"
                }
            };

            request.Headers.Add("custom", "custom");
            request.Properties.Add("custom", "custom");

            var response = await client.RequestBackchannelAuthenticationAsync(request);

            var httpRequest = handler.Request;

            httpRequest.Method.Should().Be(HttpMethod.Post);
            httpRequest.RequestUri.Should().Be(new Uri(Endpoint));
            httpRequest.Content.Should().NotBeNull();

            var headers = httpRequest.Headers;
            headers.Count().Should().Be(3);
            headers.Should().Contain(h => h.Key == "custom" && h.Value.First() == "custom");

            var properties = httpRequest.Properties;
            properties.Count.Should().Be(1);

            var prop = properties.First();
            prop.Key.Should().Be("custom");
            ((string)prop.Value).Should().Be("custom");
            
            var fields = QueryHelpers.ParseQuery(handler.Body);
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.Scope, out var scope).Should().BeTrue();
            scope.First().Should().Be("scope");
            
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.AcrValues, out var acr_values).Should().BeTrue();
            acr_values.First().Should().Be("acr_values");
            
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.BindingMessage, out var binding_message).Should().BeTrue();
            binding_message.First().Should().Be("binding_message");
            
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.ClientNotificationToken, out var client_notification_token).Should().BeTrue();
            client_notification_token.First().Should().Be("client_notification_token");
            
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.UserCode, out var user_code).Should().BeTrue();
            user_code.First().Should().Be("user_code");
            
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.RequestedExpiry, out var request_expiry).Should().BeTrue();
            int.Parse(request_expiry.First()).Should().Be(1);
            
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.IdTokenHint, out var id_token_hint).Should().BeTrue();
            id_token_hint.First().Should().Be("id_token_hint");
            
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.LoginHintToken, out var login_hint_token).Should().BeTrue();
            login_hint_token.First().Should().Be("login_hint_token");
            
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.LoginHint, out var login_hint).Should().BeTrue();
            login_hint.First().Should().Be("login_hint");
            
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.Resource, out var resource).Should().BeTrue();
            resource.Count.Should().Be(2);
            resource.First().Should().Be("resource1");
            resource.Skip(1).First().Should().Be("resource2");
        }
        
        [Fact]
        public async Task Http_request_with_request_object_should_have_correct_format()
        {
            var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");

            var client = new HttpClient(handler);
            var request = new BackchannelAuthenticationRequest
            {
                Address = Endpoint,
                RequestObject = "request",
                
                ClientId = "client",
                
                Scope = "scope",
                AcrValues = "acr_values",
                BindingMessage = "binding_message",
                ClientNotificationToken = "client_notification_token",
                UserCode = "user_code",
                
                RequestedExpiry = 1,
                
                IdTokenHint = "id_token_hint",
                LoginHintToken = "login_hint_token",
                LoginHint = "login_hint",
                
                Resource =
                {
                    "resource1",
                    "resource2"
                }
            };

            request.Headers.Add("custom", "custom");
            request.Properties.Add("custom", "custom");

            var response = await client.RequestBackchannelAuthenticationAsync(request);

            var httpRequest = handler.Request;

            httpRequest.Method.Should().Be(HttpMethod.Post);
            httpRequest.RequestUri.Should().Be(new Uri(Endpoint));
            httpRequest.Content.Should().NotBeNull();

            var headers = httpRequest.Headers;
            headers.Count().Should().Be(3);
            headers.Should().Contain(h => h.Key == "custom" && h.Value.First() == "custom");

            var properties = httpRequest.Properties;
            properties.Count.Should().Be(1);

            var prop = properties.First();
            prop.Key.Should().Be("custom");
            ((string)prop.Value).Should().Be("custom");
            
            var fields = QueryHelpers.ParseQuery(handler.Body);
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.Scope, out var scope).Should().BeFalse();
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.AcrValues, out var _).Should().BeFalse();
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.BindingMessage, out _).Should().BeFalse();
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.ClientNotificationToken, out _).Should().BeFalse();
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.UserCode, out _).Should().BeFalse();
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.RequestedExpiry, out _).Should().BeFalse();
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.IdTokenHint, out _).Should().BeFalse();
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.LoginHintToken, out _).Should().BeFalse();
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.LoginHint, out _).Should().BeFalse();
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.Resource, out _).Should().BeFalse();
            
            fields.TryGetValue(OidcConstants.BackchannelAuthenticationRequest.Request, out var ro).Should().BeTrue();
            ro.First().Should().Be("request");
        }
        
        [Fact]
        public async Task Valid_protocol_response_should_be_handled_correctly()
        {
            var document = File.ReadAllText(FileName.Create("success_ciba_response.json"));
            var handler = new NetworkHandler(document, HttpStatusCode.OK);
        
            var client = new HttpClient(handler);
            var response = await client.RequestBackchannelAuthenticationAsync(new BackchannelAuthenticationRequest
            {
                Address = Endpoint,
                ClientId = "client",
                Scope = "scope"
            });
        
            response.IsError.Should().BeFalse();
            response.ErrorType.Should().Be(ResponseErrorType.None);
            response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
        
            response.AuthenticationRequestId.Should().Be("1c266114-a1be-4252-8ad1-04986c5b9ac1");
            response.ExpiresIn.Should().Be(120);
            response.Interval.Should().Be(2);
        }
        
        //
        // [Fact]
        // public async Task Valid_protocol_error_should_be_handled_correctly()
        // {
        //     var document = File.ReadAllText(FileName.Create("failure_device_authorization_response.json"));
        //     var handler = new NetworkHandler(document, HttpStatusCode.BadRequest);
        //
        //     var client = new HttpClient(handler);
        //     var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
        //     {
        //         Address = Endpoint,
        //         ClientId = "client"
        //     });
        //
        //     response.IsError.Should().BeTrue();
        //     response.ErrorType.Should().Be(ResponseErrorType.Protocol);
        //     response.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
        //     response.Error.Should().Be("error");
        //     response.ErrorDescription.Should().Be("error_description");
        //     response.TryGet("custom").Should().Be("custom");
        // }
        //
        // [Fact]
        // public async Task Malformed_response_document_should_be_handled_correctly()
        // {
        //     var document = "invalid";
        //     var handler = new NetworkHandler(document, HttpStatusCode.OK);
        //
        //     var client = new HttpClient(handler);
        //     var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
        //     {
        //         Address = Endpoint,
        //         ClientId = "client"
        //     });
        //
        //     response.IsError.Should().BeTrue();
        //     response.ErrorType.Should().Be(ResponseErrorType.Exception);
        //     response.Raw.Should().Be("invalid");
        //     response.Exception.Should().NotBeNull();
        // }
        //
        // [Fact]
        // public async Task Exception_should_be_handled_correctly()
        // {
        //     var handler = new NetworkHandler(new Exception("exception"));
        //
        //     var client = new HttpClient(handler);
        //     var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
        //     {
        //         Address = Endpoint,
        //         ClientId = "client"
        //     });
        //
        //     response.IsError.Should().BeTrue();
        //     response.ErrorType.Should().Be(ResponseErrorType.Exception);
        //     response.Error.Should().Be("exception");
        //     response.Exception.Should().NotBeNull();
        // }
        //
        // [Fact]
        // public async Task Http_error_should_be_handled_correctly()
        // {
        //     var handler = new NetworkHandler(HttpStatusCode.NotFound, "not found");
        //
        //     var client = new HttpClient(handler);
        //     var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
        //     {
        //         Address = Endpoint,
        //         ClientId = "client"
        //     });
        //
        //     response.IsError.Should().BeTrue();
        //     response.ErrorType.Should().Be(ResponseErrorType.Http);
        //     response.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        //     response.Error.Should().Be("not found");
        // }
        //
        // [Fact]
        // public async Task Http_error_with_non_json_content_should_be_handled_correctly()
        // {
        //     var handler = new NetworkHandler("not_json", HttpStatusCode.Unauthorized);
        //
        //     var client = new HttpClient(handler);
        //     var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
        //     {
        //         Address = Endpoint,
        //         ClientId = "client"
        //     });
        //
        //     response.IsError.Should().BeTrue();
        //     response.ErrorType.Should().Be(ResponseErrorType.Http);
        //     response.HttpStatusCode.Should().Be(HttpStatusCode.Unauthorized);
        //     response.Error.Should().Be("Unauthorized");
        //     response.Raw.Should().Be("not_json");
        // }
        //
        // [Fact]
        // public async Task Http_error_with_json_content_should_be_handled_correctly()
        // {
        //     var content = new
        //     {
        //         foo = "foo",
        //         bar = "bar"
        //     };
        //
        //     var handler = new NetworkHandler(JsonSerializer.Serialize(content), HttpStatusCode.Unauthorized);
        //
        //     var client = new HttpClient(handler);
        //     var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
        //     {
        //         Address = Endpoint,
        //         ClientId = "client"
        //     });
        //
        //     response.IsError.Should().BeTrue();
        //     response.ErrorType.Should().Be(ResponseErrorType.Http);
        //     response.HttpStatusCode.Should().Be(HttpStatusCode.Unauthorized);
        //     response.Error.Should().Be("Unauthorized");
        //
        //     response.Json.TryGetString("foo").Should().Be("foo");
        //     response.Json.TryGetString("bar").Should().Be("bar");
        // }
    }
}