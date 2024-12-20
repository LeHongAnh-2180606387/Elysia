using UnityEngine;
using Unity.Netcode.Components;
using Unity.VisualScripting;

[DisallowMultipleComponent]
public class ClientNetworkTransform : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}