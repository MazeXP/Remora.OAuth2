//
//  TokenRevocationRequest.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Net.Http;
using JetBrains.Annotations;
using Remora.OAuth2.Abstractions.OAuthExtensions.TokenRevocation;
using Remora.OAuth2.Extensions;
using Remora.Rest.Core;

namespace Remora.OAuth2.OAuth2Extensions.TokenRevocation;

/// <inheritdoc />
[PublicAPI]
public record TokenRevocationRequest
(
    string Token,
    Optional<string> TokenTypeHint = default,
    Optional<IReadOnlyList<ITokenRevocationRequestExtension>> Extensions = default
) : ITokenRevocationRequest
{
    /// <inheritdoc />
    public HttpRequestMessage ToRequest(Uri revocationEndpoint)
    {
        var parameters = new Dictionary<string, string>
        {
            { "token", this.Token },
            { "token_type_hint", this.TokenTypeHint },
        };

        // ReSharper disable once InvertIf
        if (this.Extensions.IsDefined(out var extensions))
        {
            foreach (var requestExtension in extensions)
            {
                requestExtension.AddParameters(parameters);
            }
        }

        return new HttpRequestMessage(HttpMethod.Post, revocationEndpoint)
        {
            Content = new FormUrlEncodedContent(parameters)
        };
    }
}
