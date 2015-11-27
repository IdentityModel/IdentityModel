/*
 * Copyright 2014, 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
 
namespace IdentityModel.Constants
{
    // see https://tools.ietf.org/html/draft-jones-oauth-amr-values-01
    public static class AuthenticationMethods
    {
        public const string Password                        = "pwd";
        public const string ProofOfPossion                  = "pop";
        public const string OneTimePassword                 = "otp";
        public const string FingerprintBiometric            = "fpt";
        public const string RetinaScanBiometric             = "eye";
        public const string VoiceBiometric                  = "vbm";
        public const string ConfirmationByTelephone         = "tel";
        public const string ConfirmationBySms               = "sms";
        public const string KnowledgeBasedAuthentication    = "kba";
        public const string WindowsIntegratedAuthentication = "wia";
        public const string MultiFactorAuthentication       = "mfa";
        public const string UserPresenceTest                = "user";
        public const string RiskBasedAuthentication         = "risk";
        public const string MultipleChannelAuthentication   = "mfa";
    }
}