// vis2k: GUILayout instead of spacey += ...; removed Update hotkeys to avoid
// confusion if someone accidentally presses one.
using UnityEngine;
using DapperLabs.Flow.Sdk;
using DapperLabs.Flow.Sdk.DevWallet;

namespace Mirror
{
    /// <summary>Shows NetworkManager controls in a GUI at runtime.</summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/Network Manager HUD")]
    [RequireComponent(typeof(NetworkManager))]
    [HelpURL("https://mirror-networking.gitbook.io/docs/components/network-manager-hud")]
    public class NetworkGUIStart : MonoBehaviour
    {
        NetworkManager manager;

        public int offsetX;
        public int offsetY;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();
            FlowConfig flowConfig = new FlowConfig();
            flowConfig.NetworkUrl = "https://rest-testnet.onflow.org/v1";

            flowConfig.Protocol = FlowConfig.NetworkProtocol.HTTP;
            FlowSDK.Init(flowConfig);
            FlowSDK.RegisterWalletProvider(ScriptableObject.CreateInstance<DevWalletProvider>());

        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 250, 9999));
            if (!NetworkClient.isConnected && !NetworkServer.active)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            // client ready
            if (NetworkClient.isConnected && !NetworkClient.ready)
            {
                if (GUILayout.Button("Client Ready"))
                {
                    NetworkClient.Ready();
                    if (NetworkClient.localPlayer == null)
                    {
                        NetworkClient.AddPlayer();
                    }
                }
            }

            StopButtons();

            GUILayout.EndArea();
        }

        void StartButtons()
        {
            if (!NetworkClient.active)
            {
                // Server + Client
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    if (GUILayout.Button("Host (Server + Client)"))
                    {
                        FlowSDK.GetWalletProvider().Authenticate("", (string authAccount) =>
                        {
                            Debug.Log($"Authenticated: {authAccount}");
                            manager.StartHost();
                        }, () =>
                        {
                            Debug.Log("Authentication failed, aborting transaction.");
                        });
                    }
                }

                // Client + IP
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Client"))
                {
                    FlowSDK.GetWalletProvider().Authenticate("", (string authAccount) =>
                    {
                        Debug.Log($"Authenticated: {authAccount}");
                        manager.StartClient();
                    }, () =>
                    {
                        Debug.Log("Authentication failed, aborting transaction.");
                    });
                    
                }
                // This updates networkAddress every frame from the TextField
                manager.networkAddress = GUILayout.TextField(manager.networkAddress);
                GUILayout.EndHorizontal();

                // Server Only
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    // cant be a server in webgl build
                    GUILayout.Box("(  WebGL cannot be server  )");
                }
                else
                {
                    if (GUILayout.Button("Server Only")) manager.StartServer();
                }
            }
            else
            {
                // Connecting
                GUILayout.Label($"Connecting to {manager.networkAddress}..");
                if (GUILayout.Button("Cancel Connection Attempt"))
                {
                    manager.StopClient();
                }
            }
        }

        void StatusLabels()
        {
            // host mode
            // display separately because this always confused people:
            //   Server: ...
            //   Client: ...
            if (NetworkServer.active && NetworkClient.active)
            {
                GUILayout.Label($"<b>Host</b>: running via {Transport.active}");
            }
            // server only
            else if (NetworkServer.active)
            {
                GUILayout.Label($"<b>Server</b>: running via {Transport.active}");
            }
            // client only
            else if (NetworkClient.isConnected)
            {
                GUILayout.Label($"<b>Client</b>: connected to {manager.networkAddress} via {Transport.active}");
            }
        }

        void StopButtons()
        {
            // stop host if host mode
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Stop Host"))
                {
                    manager.StopHost();
                }
                if (GUILayout.Button("Stop Client"))
                {
                    manager.StopClient();
                }
                GUILayout.EndHorizontal();
            }
            // stop client if client-only
            else if (NetworkClient.isConnected)
            {
                if (GUILayout.Button("Stop Client"))
                {
                    manager.StopClient();
                }
            }
            // stop server if server-only
            else if (NetworkServer.active)
            {
                if (GUILayout.Button("Stop Server"))
                {
                    manager.StopServer();
                }
            }
        }
    }
}
