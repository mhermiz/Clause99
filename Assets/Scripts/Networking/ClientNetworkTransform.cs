using UnityEngine;
using Unity.Netcode.Components;

/// <summary>
/// Client-authoritative NetworkTransform — the client drives its own transform, and that is synchronized to the server and then to other clients.
/// </summary>
[DisallowMultipleComponent]
public class ClientNetworkTransform : NetworkTransform
{
    /// <inheritdoc />
    protected override bool OnIsServerAuthoritative()
    {
        // Override to make it *not* server authoritative, i.e. owner/client authoritative.
        return false;
    }
}
